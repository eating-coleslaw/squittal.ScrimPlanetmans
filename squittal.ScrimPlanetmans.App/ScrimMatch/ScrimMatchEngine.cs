using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimMatchEngine : IScrimMatchEngine
    {
        private readonly IScrimTeamsManager _teamsManager;
        private readonly IWebsocketMonitor _wsMonitor;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly IScrimMatchDataService _matchDataService;
        private readonly IScrimRulesetManager _rulesetManager;
        private readonly IScrimMatchScorer _matchScorer;
        private readonly ILogger<ScrimMatchEngine> _logger;

        private readonly IStatefulTimer _timer;
        private readonly IPeriodicPointsTimer _periodicTimer;

        public MatchConfiguration MatchConfiguration { get; set; } = new MatchConfiguration();
        public Ruleset MatchRuleset { get; private set; }

        public int CurrentSeriesMatch { get; private set; } = 0;
        private int? FacilityControlTeamOrdinal { get; set; }


        private bool _isRunning = false;

        private int _currentRound = 0;

        private MatchTimerTickMessage _latestTimerTickMessage;

        private MatchState _matchState = MatchState.Uninitialized;

        private DateTime _matchStartTime;


        public ScrimMatchEngine(
            IScrimTeamsManager teamsManager,
            IWebsocketMonitor wsMonitor,
            IStatefulTimer timer,
            IPeriodicPointsTimer periodicTimer,
            IScrimMatchDataService matchDataService,
            IScrimMessageBroadcastService messageService,
            IScrimRulesetManager rulesetManager,
            IScrimMatchScorer matchScorer,
            ILogger<ScrimMatchEngine> logger)
        {
            _teamsManager = teamsManager;
            _wsMonitor = wsMonitor;
            _timer = timer;
            _periodicTimer = periodicTimer;
            _messageService = messageService;
            _matchDataService = matchDataService;
            _rulesetManager = rulesetManager;
            _matchScorer = matchScorer;

            _logger = logger;

            _messageService.RaiseMatchTimerTickEvent += OnMatchTimerTick;
            _messageService.RaisePeriodicPointsTimerTickEvent += OnPeriodiocPointsTimerTick;

            _messageService.RaiseTeamOutfitChangeEvent += OnTeamOutfitChangeEvent;
            _messageService.RaiseTeamPlayerChangeEvent += OnTeamPlayerChangeEvent;

            _messageService.RaiseScrimFacilityControlActionEvent += OnFacilityControlEvent;

            _messageService.RaiseEndRoundCheckerMessage += async (s, e) => await OnEndRoundCheckerMessage(s, e);
        }


        public async Task Start()
        {
            if (_isRunning)
            {
                return;
            }

            if (_currentRound == 0)
            {
                await InitializeNewMatch();
            }

            await InitializeNewRound();

            StartRound();
        }


        public async Task ClearMatch(bool isRematch)
        {
            if (_isRunning)
            {
                await EndRound();
            }

            _wsMonitor.DisableScoring();
            if (!isRematch)
            {
                _wsMonitor.RemoveAllCharacterSubscriptions();
            }
            _messageService.DisableLogging();

            var previousWorldId = MatchConfiguration.WorldIdString;
            var previousIsManualWorldId = MatchConfiguration.IsManualWorldId;

            var previousEndRoundOnFacilityCapture = MatchConfiguration.EndRoundOnFacilityCapture;
            var previousIsManualEndRoundOnFacilityCapture = MatchConfiguration.EndRoundOnFacilityCapture;

            var previousTargetPointValue = MatchConfiguration.TargetPointValue;
            var previousIsManualTargetPointValue = MatchConfiguration.IsManualTargetPointValue;

            var previousInitialPoints = MatchConfiguration.InitialPoints;
            var previousIsManualInitialPoints = MatchConfiguration.IsManualInitialPoints;

            var previousPeriodicFacilityControlPoints = MatchConfiguration.PeriodicFacilityControlPoints;
            var previousIsManualPeriodicFacilityControlPoints = MatchConfiguration.IsManualPeriodicFacilityControlPoints;

            var previousPeriodicFacilityControlInterval = MatchConfiguration.PeriodicFacilityControlInterval;
            var previousIsManualPeriodicFacilityControlInterval = MatchConfiguration.IsManualPeriodicFacilityControlInterval;

            var activeRuleset = await _rulesetManager.GetActiveRulesetAsync();
            MatchConfiguration = new MatchConfiguration(activeRuleset);

            if (isRematch)
            {
                MatchConfiguration.TrySetWorldId(previousWorldId, previousIsManualWorldId);
                MatchConfiguration.TrySetEndRoundOnFacilityCapture(previousEndRoundOnFacilityCapture, previousIsManualEndRoundOnFacilityCapture);
                
                MatchConfiguration.TrySetTargetPointValue(previousTargetPointValue, previousIsManualTargetPointValue);
                MatchConfiguration.TrySetInitialPoints(previousInitialPoints, previousIsManualInitialPoints);
                MatchConfiguration.TrySetPeriodicFacilityControlPoints(previousPeriodicFacilityControlPoints, previousIsManualPeriodicFacilityControlPoints);
                MatchConfiguration.TrySetPeriodicFacilityControlInterval(previousPeriodicFacilityControlInterval, previousIsManualPeriodicFacilityControlInterval);
            }
            else
            {
                MatchConfiguration.TrySetEndRoundOnFacilityCapture(activeRuleset.DefaultEndRoundOnFacilityCapture, false);

                MatchConfiguration.TrySetTargetPointValue(activeRuleset.TargetPointValue, false);
                MatchConfiguration.TrySetInitialPoints(activeRuleset.InitialPoints, false);
                MatchConfiguration.TrySetPeriodicFacilityControlPoints(activeRuleset.PeriodicFacilityControlPoints, false);
                MatchConfiguration.TrySetPeriodicFacilityControlInterval(activeRuleset.PeriodicFacilityControlInterval, false);
            }

            _matchState = MatchState.Uninitialized;
            _currentRound = 0;

            _matchDataService.CurrentMatchRound = _currentRound;
            _matchDataService.CurrentMatchId = string.Empty;

            _latestTimerTickMessage = null;

            if (isRematch)
            {
                _teamsManager.UpdateAllTeamsMatchSeriesResults(CurrentSeriesMatch);
                _teamsManager.ResetAllTeamsMatchData();
            }
            else
            {
                CurrentSeriesMatch = 0;

                _teamsManager.ClearAllTeams();
            }

            SendMatchStateUpdateMessage();
            SendMatchConfigurationUpdateMessage();
        }

        public void ConfigureMatch(MatchConfiguration configuration)
        {
            MatchConfiguration = configuration;

            _wsMonitor.SetFacilitySubscription(MatchConfiguration.FacilityId);
            _wsMonitor.SetWorldSubscription(MatchConfiguration.WorldId);

            SendMatchConfigurationUpdateMessage();
        }

        private bool TrySetMatchRuleset(Ruleset matchRuleset)
        {
            if (CanChangeRuleset())
            {
                MatchRuleset = matchRuleset;
                _matchDataService.CurrentMatchRulesetId = matchRuleset.Id;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task EndRound()
        {
            _isRunning = false;
            _matchState = MatchState.Stopped;

            _wsMonitor.DisableScoring();

            // Stop the timer if forcing the round to end (as opposed to timer reaching 0)
            if (GetLatestTimerTickMessage().State != TimerState.Stopped)
            {
                _timer.Halt();
            }

            if (MatchConfiguration.EnablePeriodicFacilityControlRewards)
            {
                _periodicTimer.Halt();
            }

            await _teamsManager.SaveRoundEndScores(_currentRound);

            // TODO: Save Match Round results & metadata

            _messageService.BroadcastSimpleMessage($"Round {_currentRound} ended; scoring diabled");

            SendMatchStateUpdateMessage();

            _messageService.DisableLogging();
        }

        public async Task InitializeNewMatch()
        {
            TrySetMatchRuleset(_rulesetManager.ActiveRuleset);

            _matchStartTime = DateTime.UtcNow;
            FacilityControlTeamOrdinal = null;

            CurrentSeriesMatch++;

            if (MatchConfiguration.SaveLogFiles == true)
            {
                var matchId = BuildMatchId();

                _messageService.SetLogFileName($"{matchId}.txt");

                var scrimMatch = new Data.Models.ScrimMatch
                {
                    Id = matchId,
                    StartTime = _matchStartTime,
                    Title = MatchConfiguration.Title,
                    RulesetId = MatchRuleset.Id
                };

                await _matchDataService.SaveToCurrentMatch(scrimMatch);
            }
        }

        private string BuildMatchId()
        {
            var matchId = _matchStartTime.ToString("yyyyMMddTHHmmss");

            for (var i = 1; i <= 3; i++)
            {
                var alias = _teamsManager.GetTeamAliasDisplay(i);

                if (string.IsNullOrWhiteSpace(alias))
                {
                    continue;
                }

                matchId = $"{matchId}_{alias}";
            }

            return matchId;
        }

        public async Task InitializeNewRound()
        {
            _currentRound += 1;

            _matchDataService.CurrentMatchRound = _currentRound;

            FacilityControlTeamOrdinal = null;

            _timer.Configure(TimeSpan.FromSeconds(MatchConfiguration.RoundSecondsTotal));
            
            if (MatchConfiguration.EnablePeriodicFacilityControlRewards && MatchConfiguration.PeriodicFacilityControlInterval.HasValue)
            {
                _periodicTimer.Configure(TimeSpan.FromSeconds(MatchConfiguration.PeriodicFacilityControlInterval.Value));
            }

            await _matchDataService.SaveCurrentMatchRoundConfiguration(MatchConfiguration);
        }

        public void StartRound()
        {
            _isRunning = true;
            _matchState = MatchState.Running;

            if (MatchConfiguration.SaveLogFiles)
            {
                _messageService.EnableLogging();
            }

            _timer.Start();
            _wsMonitor.EnableScoring();

            SendMatchStateUpdateMessage();
        }

        public void PauseRound()
        {
            _isRunning = false;
            _matchState = MatchState.Paused;

            _wsMonitor.DisableScoring();

            _timer.Pause();
            if (MatchConfiguration.EnablePeriodicFacilityControlRewards)
            {
                _periodicTimer.Pause();
            }

            SendMatchStateUpdateMessage();

            _messageService.DisableLogging();
        }

        public async Task ResetRound()
        {
            if (_currentRound == 0)
            {
                return;
            }
            
            _wsMonitor.DisableScoring();
            
            _timer.Reset();
            if (MatchConfiguration.EnablePeriodicFacilityControlRewards)
            {
                _periodicTimer.Reset();
            }


            await _teamsManager.RollBackAllTeamStats(_currentRound);

            await _matchDataService.RemoveMatchRoundConfiguration(_currentRound);

            _currentRound -= 1;

            if (_currentRound == 0)
            {
                _matchState = MatchState.Uninitialized;
                _latestTimerTickMessage = null;
            }

            FacilityControlTeamOrdinal = null;

            _matchDataService.CurrentMatchRound = _currentRound;

            SendMatchStateUpdateMessage();

            _messageService.DisableLogging();
        }

        public void ResumeRound()
        {
            _isRunning = true;
            _matchState = MatchState.Running;

            if (MatchConfiguration.SaveLogFiles)
            {
                _messageService.EnableLogging();
            }

            _timer.Resume();
            if (MatchConfiguration.EnablePeriodicFacilityControlRewards)
            {
                _periodicTimer.Resume();
            }

            _wsMonitor.EnableScoring();

            SendMatchStateUpdateMessage();
        }

        public void SubmitPlayersList()
        {
            _wsMonitor.AddCharacterSubscriptions(_teamsManager.GetAllPlayerIds());
        }

        private async Task OnEndRoundCheckerMessage(object sender, ScrimMessageEventArgs<EndRoundCheckerMessage> e)
        {
            if (_isRunning)
            {
                await EndRound();
            }
        }

        private void OnMatchTimerTick(object sender, ScrimMessageEventArgs<MatchTimerTickMessage> e)
        {
            SetLatestTimerTickMessage(e.Message);
        }

        private void OnPeriodiocPointsTimerTick(object sender, ScrimMessageEventArgs<PeriodicPointsTimerStateMessage> e)
        {
            if (!MatchConfiguration.EnablePeriodicFacilityControlRewards || !FacilityControlTeamOrdinal.HasValue)
            {
                return;
            }

            var controllingTeamOrdinal = FacilityControlTeamOrdinal.Value;

            _matchScorer.ScorePeriodicFacilityControlTick(controllingTeamOrdinal);
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public int GetCurrentRound()
        {
            return _currentRound;
        }

        public MatchState GetMatchState()
        {
            return _matchState;
        }

        public string GetMatchId()
        {
            return _matchDataService.CurrentMatchId;
        }

        public MatchTimerTickMessage GetLatestTimerTickMessage()
        {
            return _latestTimerTickMessage;
        }

        private bool CanChangeRuleset()
        {
            return (_currentRound == 0 && _matchState == MatchState.Uninitialized && !_isRunning);
        }

        private void SetLatestTimerTickMessage(MatchTimerTickMessage value)
        {
            _latestTimerTickMessage = value;
        }

        private void OnTeamOutfitChangeEvent(object sender, ScrimMessageEventArgs<TeamOutfitChangeMessage> e)
        {
            if (MatchConfiguration.IsManualWorldId)
            {
                return;
            }

            int? worldId;

            var message = e.Message;
            var changeType = message.ChangeType;
            bool isRollBack = false;

            if (changeType == TeamChangeType.Add)
            {
                worldId = e.Message.Outfit.WorldId;
            }
            else
            {
                worldId = _teamsManager.GetNextWorldId(MatchConfiguration.WorldId);
                isRollBack = true;
            }

            if (worldId == null)
            {
                MatchConfiguration.ResetWorldId();
                SendMatchConfigurationUpdateMessage();
            }
            else if (MatchConfiguration.TrySetWorldId((int)worldId, false, isRollBack))
            {
                SendMatchConfigurationUpdateMessage();
            }
        }

        private void OnTeamPlayerChangeEvent(object sender, ScrimMessageEventArgs<TeamPlayerChangeMessage> e)
        {
            if (MatchConfiguration.IsManualWorldId)
            {
                return;
            }

            var message = e.Message;
            var changeType = message.ChangeType;
            var player = message.Player;

            // Handle outfit removals via Team Outfit Change events
            if (!player.IsOutfitless)
            {
                return;
            }

            int? worldId;
            bool isRollBack = false;

            if (changeType == TeamPlayerChangeType.Add)
            {
                worldId = player.WorldId;
            }
            else
            {
                worldId = _teamsManager.GetNextWorldId(MatchConfiguration.WorldId);
                isRollBack = true;
            }

            if (worldId == null)
            {
                MatchConfiguration.ResetWorldId();
                SendMatchConfigurationUpdateMessage();
            }
            else if (MatchConfiguration.TrySetWorldId((int)worldId, false, isRollBack))
            {
                SendMatchConfigurationUpdateMessage();
            }
        }

        private void OnFacilityControlEvent(object sender, ScrimMessageEventArgs<ScrimFacilityControlActionEventMessage> e)
        {
            if (!_isRunning)
            {
                return;
            }
            
            if (!MatchConfiguration.EnablePeriodicFacilityControlRewards)
            {
                return;
            }

            var controlEvent = e.Message.FacilityControl;
            var eventFacilityId = controlEvent.FacilityId;
            var eventWorldId = controlEvent.WorldId;

            if (eventFacilityId == MatchConfiguration.FacilityId && eventWorldId == MatchConfiguration.WorldId)
            {
                if (_periodicTimer.CanStart())
                {
                    _periodicTimer.Start();
                }
                else
                {
                    _periodicTimer.Restart();
                }
                FacilityControlTeamOrdinal = controlEvent.ControllingTeamOrdinal;
            }
        }
        
        private void SendMatchStateUpdateMessage()
        {
            _messageService.BroadcastMatchStateUpdateMessage(new MatchStateUpdateMessage(_matchState, _currentRound, DateTime.UtcNow, MatchConfiguration.Title, _matchDataService.CurrentMatchId));
        }

        private void SendMatchConfigurationUpdateMessage()
        {
            _messageService.BroadcastMatchConfigurationUpdateMessage(new MatchConfigurationUpdateMessage(MatchConfiguration));
        }
    }
}
