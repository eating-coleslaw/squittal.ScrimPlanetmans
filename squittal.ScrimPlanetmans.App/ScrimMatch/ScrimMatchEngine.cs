using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Models.ScrimEngine;
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
        private readonly IScrimPlayersManager _playersManager;
        private readonly IWebsocketMonitor _wsMonitor;
        private readonly ILogger<ScrimMatchEngine> _logger;

        private readonly IStatefulTimer _timer;

        public Team Team1;
        public Team Team2;

        public MatchConfiguration MatchConfiguration;

        private Dictionary<int, Team> _ordinalTeamMap = new Dictionary<int, Team>();

        private List<string> _allCharacterIds = new List<string>();


        private int _roundSecondsMax = 900;
        private int _roundSecondsRemaining;
        private int _roundSecondsElapsed = 0;

        public ScrimMatchEngine(IScrimPlayersManager playersManager, IScrimTeamsManager teamsManager, IWebsocketMonitor wsMonitor, IStatefulTimer timer, ILogger<ScrimMatchEngine> logger)
        {
            _playersManager = playersManager;
            _teamsManager = teamsManager;
            _wsMonitor = wsMonitor;
            _timer = timer;
            _logger = logger;

            Team1 = _teamsManager.GetTeamOne();
            Team2 = _teamsManager.GetTeamTwo();
        }

        public void ClearMatch()
        {
            throw new NotImplementedException();
        }

        public void ConfigureMatch()
        {
            throw new NotImplementedException();
        }

        public void EndRound()
        {
            throw new NotImplementedException();
        }

        public void InitializeNewMatch()
        {
            throw new NotImplementedException();
        }

        public void InitializeNewRound()
        {
            _roundSecondsRemaining = _roundSecondsMax;
            _roundSecondsElapsed = 0;

            _timer.Configure(TimeSpan.FromSeconds(_roundSecondsMax));

        }

        public void PauseRound()
        {
            throw new NotImplementedException();
        }

        public void ResetRound()
        {
            throw new NotImplementedException();
        }

        public void ResumeRound()
        {
            throw new NotImplementedException();
        }

        public void StartRound()
        {

        }

        
    }
}
