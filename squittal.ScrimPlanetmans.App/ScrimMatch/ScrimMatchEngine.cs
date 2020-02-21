using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Events;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimMatchEngine : IScrimMatchEngine
    {
        private readonly IScrimTeamsManager _teamsManager;
        private readonly IWebsocketMonitor _wsMonitor;
        private readonly ILogger<ScrimMatchEngine> _logger;

        private readonly IStatefulTimer _timer;

        //public Team Team1;
        //public Team Team2;

        public MatchConfiguration MatchConfiguration { get; set; }

        private Dictionary<int, Team> _ordinalTeamMap = new Dictionary<int, Team>();

        private List<string> _allCharacterIds = new List<string>();

        private bool _isRunning = false;

        private int _currentRound = 0;

        private int _roundSecondsMax = 900;
        private int _roundSecondsRemaining;
        private MatchTimerTickMessage _latestTimerTickMessage;

        //private int _roundSecondsElapsed = 0;

        

        public ScrimMatchEngine(IScrimTeamsManager teamsManager, IWebsocketMonitor wsMonitor, IStatefulTimer timer, ILogger<ScrimMatchEngine> logger)
        {
            _teamsManager = teamsManager;
            _wsMonitor = wsMonitor;
            _timer = timer;
            _logger = logger;

            _timer.RaiseMatchTimerTickEvent += OnMatchTimerTick;

            MatchConfiguration = new MatchConfiguration();
            //Team1 = _teamsManager.GetTeamOne();
            //Team2 = _teamsManager.GetTeamTwo();
        }

        
        public void Start()
        {
            if (_isRunning)
            {
                return;
            }
            
            if (_currentRound == 0)
            {
                // TODO: InitializeNewMatch
            }

            InitializeNewRound();

            StartRound();
        }


        public void ClearMatch()
        {
            _wsMonitor.DisableScoring();
            _wsMonitor.RemoveAllCharacterSubscriptions();
            
            MatchConfiguration = new MatchConfiguration();

            _roundSecondsMax = MatchConfiguration.RoundSecondsTotal;

            _currentRound = 0;

            _latestTimerTickMessage = null;
            // TODO: empty Teams, reset points, etc.

        }

        public void ConfigureMatch(MatchConfiguration configuration)
        {
            MatchConfiguration = configuration;

            _roundSecondsMax = MatchConfiguration.RoundSecondsTotal;

        }

        public void EndRound()
        {
            _isRunning = false;

            _wsMonitor.DisableScoring();

            // Stop the timer if forcing the round to end (as opposed to timer reaching 0)
            if (GetLatestTimerTickMessage().MatchTimerStatus.State != MatchTimerState.Stopped)
            {
                _timer.Stop(); // TODO: change this to Halt, if Halt ends up getting implemented
            }
        }

        public void InitializeNewMatch()
        {
            throw new NotImplementedException();
        }

        public void InitializeNewRound()
        {
            _currentRound += 1;
            
            _roundSecondsRemaining = _roundSecondsMax;
            //_roundSecondsElapsed = 0;

            _timer.Configure(TimeSpan.FromSeconds(_roundSecondsMax));

        }

        public void StartRound()
        {
            _timer.Start();
            _wsMonitor.EnableScoring();
        }

        public void PauseRound()
        {
            _wsMonitor.DisableScoring();
            _timer.Pause();
        }

        public void ResetRound()
        {
            _timer.Reset(); // TODO: actually implement StatefulTimer.Reset
            _wsMonitor.DisableScoring();
        }

        public void ResumeRound()
        {
            _timer.Resume();
            _wsMonitor.EnableScoring();
        }

        

        public void SubmitPlayersList()
        {
            _wsMonitor.AddCharacterSubscriptions(_teamsManager.GetAllPlayerIds());
        }

        private void OnMatchTimerTick(object sender, MatchTimerTickEventArgs e)
        {
            var message = e.Message;

            SetLatestTimerTickMessage(e.Message);

            var status = message.MatchTimerStatus;
            var info = message.Info;

            var state = status.State;

            if (state == MatchTimerState.Stopped && _isRunning)
            {
                EndRound();
                return;
            }
        }

        public MatchTimerTickMessage GetLatestTimerTickMessage()
        {
            return _latestTimerTickMessage;
        }

        private void SetLatestTimerTickMessage(MatchTimerTickMessage value)
        {
            _latestTimerTickMessage = value;
        }

    }
}
