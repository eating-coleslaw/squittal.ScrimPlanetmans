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


        public ScrimMatchEngine(IScrimPlayersManager playersManager, IScrimTeamsManager teamsManager, IWebsocketMonitor wsMonitor, ILogger<ScrimMatchEngine> logger)
        {
            _playersManager = playersManager;
            _teamsManager = teamsManager;
            _wsMonitor = wsMonitor;
            _logger = logger;

            Team1 = new Team("tm1", "Team 1", 1);
            _ordinalTeamMap.Add(1, Team1);

            Team2 = new Team("tm2", "Team 2", 2);
            _ordinalTeamMap.Add(2, Team2);
        }

        public void UpdateTeamAlias(int teamOrdinal, string alias)
        {
            _ordinalTeamMap[teamOrdinal].Alias = alias;
        }

        public void SubmitPlayersList()
        {
            _wsMonitor.AddCharacterSubscriptions(_allCharacterIds);
        }

        // Returns whether specified character was added to the specified team
        public async Task<bool> AddCharacterToTeam(int teamOrdinal, string characterId)
        {
            if (!IsCharacterAvailable(characterId, out Team owningTeam))
            {
                return false;
            }

            var player = await _playersManager.GetPlayerFromCharacterId(characterId);

            if (player == null)
            {
                return false;
            }

            var team = _ordinalTeamMap[teamOrdinal];

            if(team.TryAddPlayer(player))
            {
                _allCharacterIds.Add(characterId);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AddOutfitAliasToTeam(int teamOrdinal, string alias)
        {
            if (!IsOutfitAvailable(alias, out Team owningTeam))
            {
                return false;
            }

            var players = await _playersManager.GetPlayersFromOutfitAlias(alias);

            if (players == null || !players.Any())
            {
                return false;
            }

            var team = _ordinalTeamMap[teamOrdinal];
            var anyPlayersAdded = false;

            //TODO: track which players were added and which weren't

            foreach (var player in players)
            {
                if (team.TryAddPlayer(player))
                {
                    _allCharacterIds.Add(player.Id);
                    anyPlayersAdded = true;
                }
            }

            return anyPlayersAdded;
        }

        private bool IsCharacterAvailable(string characterId, out Team owningTeam)
        {
            var team1 = _ordinalTeamMap[1];

            if (team1.ContainsPlayer(characterId))
            {
                owningTeam = team1;
                return false;
            }

            var team2 = _ordinalTeamMap[2];
            if (team2.ContainsPlayer(characterId))
            {
                owningTeam = team2;
                return false;
            }

            owningTeam = null;
            return true;
        }

        private bool IsOutfitAvailable(string alias, out Team owningTeam)
        {
            var team1 = _ordinalTeamMap[1];

            if (team1.ContainsOutfit(alias))
            {
                owningTeam = team1;
                return false;
            }

            var team2 = _ordinalTeamMap[2];
            if (team2.ContainsOutfit(alias))
            {
                owningTeam = team2;
                return false;
            }

            owningTeam = null;
            return true;
        }

        private Team GetOtherTeamFromOrdinal(int initTeamOrdinal)
        {
            return initTeamOrdinal == 1 ? _ordinalTeamMap[2] : _ordinalTeamMap[1];
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
