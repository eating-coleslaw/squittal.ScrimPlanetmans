using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Threading;
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
        private readonly IScrimRoundEndCheckerService _roundEndChecker;

        public MatchConfiguration MatchConfiguration { get; set; } = new MatchConfiguration();
        public Ruleset MatchRuleset { get; private set; }

        public int CurrentSeriesMatch { get; private set; } = 0;
        private int? FacilityControlTeamOrdinal { get; set; }


        private bool _isRunning = false;

        private int _currentRound = 0;

        private MatchTimerTickMessage _latestTimerTickMessage;
        private PeriodicPointsTimerStateMessage _latestPeriodicPointsTimerTickMessage;
        private ScrimFacilityControlActionEventMessage _latestFacilityControlMessage;

        private MatchState _matchState = MatchState.Uninitialized;

        private DateTime _matchStartTime;

        private readonly AutoResetEvent _captureAutoEvent = new AutoResetEvent(true);

        public ScrimMatchEngine(
            IScrimTeamsManager teamsManager,
            IWebsocketMonitor wsMonitor,
            IStatefulTimer timer,
            IPeriodicPointsTimer periodicTimer,
            IScrimMatchDataService matchDataService,
            IScrimMessageBroadcastService messageService,
            IScrimRulesetManager rulesetManager,
            IScrimMatchScorer matchScorer,
            IScrimRoundEndCheckerService roundEndChecker,
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
            _roundEndChecker = roundEndChecker;

            _logger = logger;

            _messageService.RaiseMatchTimerTickEvent += OnMatchTimerTick;
            _messageService.RaisePeriodicPointsTimerTickEvent += async (s, e) => await OnPeriodiocPointsTimerTick(s, e);

            _messageService.RaiseTeamOutfitChangeEvent += OnTeamOutfitChangeEvent;
            _messageService.RaiseTeamPlayerChangeEvent += OnTeamPlayerChangeEvent;

            _messageService.RaiseScrimFacilityControlActionEvent += OnFacilityControlEvent;

            _messageService.RaiseEndRoundCheckerMessage += async (s, e) => await OnEndRoundCheckerMessage(s, e);

            _roundEndChecker.Disable();
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
            MatchConfiguration.CopyValues(configuration);

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
            _logger.LogInformation($"Ending Round {_currentRound}");

            _isRunning = false;
            _matchState = MatchState.Stopped;

            _roundEndChecker.Disable();

            _wsMonitor.DisableScoring();

            // Stop the timer if forcing the round to end (as opposed to timer reaching 0)
            if (GetLatestTimerTickMessage().State != TimerState.Stopped)
            {
                _logger.LogInformation($"Trying to Halt Match Timer");
                _timer.Halt();
                _logger.LogInformation($"Halted Match Timer");
            }

            if (MatchConfiguration.EnablePeriodicFacilityControlRewards)
            {
                _logger.LogInformation($"Trying to Halt Periodic Timer");
                _periodicTimer.Halt();
                _logger.LogInformation($"Halted Periodic Timer");
            }

            _logger.LogInformation($"Saving team scores for round {_currentRound}");
            await _teamsManager.SaveRoundEndScores(_currentRound);

            // TODO: Save Match Round results & metadata

            _logger.LogInformation($"Round {_currentRound} ended; scoring disabled");

            #pragma warning disable CS4014
            Task.Run(() =>
            {
                _messageService.BroadcastSimpleMessage($"Round {_currentRound} ended; scoring disabled");
            }).ConfigureAwait(false);
            #pragma warning restore CS4014

            SendMatchStateUpdateMessage();

            _messageService.DisableLogging();
        }

        public async Task InitializeNewMatch()
        {
            TrySetMatchRuleset(_rulesetManager.ActiveRuleset);

            _matchStartTime = DateTime.UtcNow;
            FacilityControlTeamOrdinal = null;
            _latestFacilityControlMessage = null;
            _latestPeriodicPointsTimerTickMessage = null;

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
            _latestFacilityControlMessage = null;
            _latestPeriodicPointsTimerTickMessage = null;

            if (MatchConfiguration.EnableRoundTimeLimit)
            {
                _timer.Configure(TimeSpan.FromSeconds(MatchConfiguration.RoundSecondsTotal));
            }
            else
            {
                _timer.Configure(null);
            }
            
            if (MatchConfiguration.EnablePeriodicFacilityControlRewards && MatchConfiguration.PeriodicFacilityControlInterval.HasValue)
            {
                _periodicTimer.Configure(TimeSpan.FromSeconds(MatchConfiguration.PeriodicFacilityControlInterval.Value));
            }

            _teamsManager.ClearPlayerLastKilledByMap();

            await _matchDataService.SaveCurrentMatchRoundConfiguration(MatchConfiguration);
        }

        public void StartRound()
        {
            _isRunning = true;
            _matchState = MatchState.Running;

            _roundEndChecker.Enable();

            if (MatchConfiguration.SaveLogFiles)
            {
                _messageService.EnableLogging();
            }

            _timer.Start();
            _wsMonitor.EnableScoring();

            SendMatchStateUpdateMessage();

            Console.WriteLine($"Match Configuration Settings:\n            Title: {MatchConfiguration.Title} (IsManual={MatchConfiguration.IsManualTitle})\n            Round Length: {MatchConfiguration.RoundSecondsTotal} (IsManual={MatchConfiguration.IsManualRoundSecondsTotal})\n            Point Target: {MatchConfiguration.TargetPointValue} (IsManual={MatchConfiguration.IsManualTargetPointValue})\n            Periodic Control Points: {MatchConfiguration.PeriodicFacilityControlPoints} (IsManual={MatchConfiguration.IsManualPeriodicFacilityControlPoints})\n            Periodic Control Interval: {MatchConfiguration.PeriodicFacilityControlInterval} (IsManual={MatchConfiguration.IsManualPeriodicFacilityControlInterval})\n            World ID: {MatchConfiguration.WorldIdString} (IsManual={MatchConfiguration.IsManualWorldId})\n            Facility ID: {MatchConfiguration.FacilityIdString}\n            End Round on Capture?: {MatchConfiguration.EndRoundOnFacilityCapture} (IsManual={MatchConfiguration.IsManualEndRoundOnFacilityCapture})");
        }

        public void PauseRound()
        {
            _isRunning = false;
            _matchState = MatchState.Paused;

            _roundEndChecker.Disable();

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

            _roundEndChecker.Disable();

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
            _latestFacilityControlMessage = null;
            _latestPeriodicPointsTimerTickMessage = null;

            _matchDataService.CurrentMatchRound = _currentRound;

            SendMatchStateUpdateMessage();

            _messageService.DisableLogging();
        }

        public void ResumeRound()
        {
            _isRunning = true;
            _matchState = MatchState.Running;

            _roundEndChecker.Enable();

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

        private void OnMatchTimerTick(object sender, ScrimMessageEventArgs<MatchTimerTickMessage> e)
        {
            
            _latestTimerTickMessage = e.Message;
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

        public PeriodicPointsTimerStateMessage GetLatestPeriodicPointsTimerTickMessage()
        {
            return _latestPeriodicPointsTimerTickMessage;
        }

        public ScrimFacilityControlActionEventMessage GetLatestFacilityControlMessage()
        {
            return _latestFacilityControlMessage;
        }

        public int? GetFacilityControlTeamOrdinal()
        {
            return FacilityControlTeamOrdinal;
        }

        private bool CanChangeRuleset()
        {
            return (_currentRound == 0 && _matchState == MatchState.Uninitialized && !_isRunning);
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
            else if (changeType == TeamChangeType.Remove)
            {
                worldId = _teamsManager.GetNextWorldId(MatchConfiguration.WorldId);
                isRollBack = true;
            }
            else
            {
                return;
            }

            if (worldId == null)
            {
                //Console.WriteLine($"ScrimMatchEngine: Resetting World ID from Outfit Change ({MatchConfiguration})!");
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


            // Handle outfit additions/removals via Team Outfit Change events
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
                //Console.WriteLine($"ScrimMatchEngine: Resetting World ID from Player Change!");
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
                //_logger.LogInformation($"PeriodicFacilityControlRewards not enabled");
                return;
            }

            var controlEvent = e.Message.FacilityControl;
            var eventFacilityId = controlEvent.FacilityId;
            var eventWorldId = controlEvent.WorldId;

            if (eventFacilityId == MatchConfiguration.FacilityId && eventWorldId == MatchConfiguration.WorldId)
            {
                _captureAutoEvent.WaitOne();

                _latestFacilityControlMessage = e.Message;

                FacilityControlTeamOrdinal = controlEvent.ControllingTeamOrdinal;
                _logger.LogInformation($"FacilityControlTeamOrdinal: {controlEvent.ControllingTeamOrdinal}");

                if (_periodicTimer.CanStart())
                {
                    _periodicTimer.Start();
                    _logger.LogInformation($"PeriodicTimer started for team {FacilityControlTeamOrdinal}");
                }
                else
                {
                    _periodicTimer.Restart();
                    _logger.LogInformation($"PeriodicTimer restarted for team {FacilityControlTeamOrdinal}");
                }

                _captureAutoEvent.Set();
            }
        }

        #pragma warning disable CS1998
        private async Task OnPeriodiocPointsTimerTick(object sender, ScrimMessageEventArgs<PeriodicPointsTimerStateMessage> e)
        {
            _logger.LogInformation($"Received PeriodicPointsTimerStateMessage");

            _latestPeriodicPointsTimerTickMessage = e.Message;


            if (e.Message.PeriodElapsed && MatchConfiguration.EnablePeriodicFacilityControlRewards && FacilityControlTeamOrdinal.HasValue && _isRunning)
            {
                #pragma warning disable CS4014
                Task.Run(() =>
                {
                    ProcessPeriodicPointsTick(e.Message);

                }).ConfigureAwait(false);
                #pragma warning restore CS4014
            }

        }
        #pragma warning restore CS1998

        private async Task ProcessPeriodicPointsTick(PeriodicPointsTimerStateMessage payload)
        {
            _logger.LogInformation($"Processing PeriodicPointsTimer tick");

            if (!_isRunning)
            {
                _logger.LogInformation($"Failed to process PeriodicPointsTimer tick: match is not running");
                return;
            }

            var timestamp = DateTime.Now;

            var controllingTeamOrdinal = FacilityControlTeamOrdinal.Value;

            try
            {
                var points = _matchScorer.ScorePeriodicFacilityControlTick(controllingTeamOrdinal);

                if (!points.HasValue)
                {
                    _logger.LogInformation($"Failed to score PeriodicPointsTimer tick: ScrimMatchScorer returned no points value");
                    return;
                }

                _logger.LogInformation($"Scored PeriodicPointsTimer tick: {points.Value} points");


                var periodicTickModel = new ScrimPeriodicControlTick()
                {
                    ScrimMatchId = GetMatchId(),
                    Timestamp = timestamp,
                    ScrimMatchRound = GetCurrentRound(),
                    TeamOrdinal = controllingTeamOrdinal,
                    Points = points.Value
                };

                await _matchDataService.SaveScrimPeriodicControlTick(periodicTickModel);

                _logger.LogInformation($"ScrimPeriodicControlTick saved to Db: Round {periodicTickModel.ScrimMatchRound}, Team {controllingTeamOrdinal}, {points.Value} points");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        private async Task OnEndRoundCheckerMessage(object sender, ScrimMessageEventArgs<EndRoundCheckerMessage> e)
        {
            _logger.LogInformation($"OnEndRoundCheckerMessage received: {Enum.GetName(typeof(EndRoundReason), e.Message.EndRoundReason)}");

            if (_isRunning)
            {

                await EndRound();
            }
        }

        #region Outbound Messages
        private void SendMatchStateUpdateMessage()
        {
            _messageService.BroadcastMatchStateUpdateMessage(new MatchStateUpdateMessage(_matchState, _currentRound, DateTime.UtcNow, MatchConfiguration.Title, _matchDataService.CurrentMatchId));
        }

        private void SendMatchConfigurationUpdateMessage()
        {
            _messageService.BroadcastMatchConfigurationUpdateMessage(new MatchConfigurationUpdateMessage(MatchConfiguration));
        }
        #endregion Outbound Messages
    }
}
