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
    public class ScrimMatchEngine : IHostedService
    {
        private readonly IScrimTeamsManager _teamsManager;
        private readonly IScrimPlayersManager _playersManager;
        private readonly IWebsocketMonitor _wsMonitor;
        private readonly ILogger<ScrimMatchEngine> _logger;

        public Team Team1;
        public Team Team2;

        public MatchConfiguration MatchConfiguration;

        private Dictionary<int, Team> _ordinalTeamMap = new Dictionary<int, Team>();

        private List<string> _allCharacterIds = new List<string>();


        private int _roundSecondsMax = 900;
        private int _roundSecondsRemaining;
        private int _roundSecondsElapsed = 0;

        public ScrimMatchEngine(IScrimPlayersManager playersManager, IScrimTeamsManager teamsManager, IWebsocketMonitor wsMonitor, ILogger<ScrimMatchEngine> logger)
        {
            _playersManager = playersManager;
            _teamsManager = teamsManager;
            _wsMonitor = wsMonitor;
            _logger = logger;

            Team1 = _teamsManager.GetTeamOne();
            Team2 = _teamsManager.GetTeamTwo();
        }

        public void InitializeNewRound()
        {
            _roundSecondsRemaining = _roundSecondsMax;
            _roundSecondsElapsed = 0;

        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Scrim Match Engine");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Scrim Match Engine");
            return Task.CompletedTask;
        }

        
    }
}
