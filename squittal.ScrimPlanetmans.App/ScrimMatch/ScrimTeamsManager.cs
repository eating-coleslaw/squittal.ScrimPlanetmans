using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Hubs;
using squittal.ScrimPlanetmans.Hubs.Models;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.App.ScrimMatch
{
    public class ScrimTeamsManager : IScrimTeamsManager
    {
        private readonly IScrimPlayersService _scrimPlayers;
        private readonly IWebsocketMonitor _wsMonitor;
        private readonly IHubContext<MatchSetupHub> _setupHubContext;
        private readonly ILogger<ScrimTeamsManager> _logger;

        private readonly Team Team1;
        private readonly Team Team2;

        private readonly Dictionary<int, Team> _ordinalTeamMap = new Dictionary<int, Team>();

        private readonly List<string> _allCharacterIds = new List<string>();

        private readonly List<Player> _allPlayers = new List<Player>();

        private Dictionary<string, int> _characterTeamOrdinalMap;

        public ScrimTeamsManager(IScrimPlayersService scrimPlayers, IWebsocketMonitor wsMonitor, IHubContext<MatchSetupHub> setupHubContext, ILogger<ScrimTeamsManager> logger)
        {
            _scrimPlayers = scrimPlayers;
            _wsMonitor = wsMonitor;
            _setupHubContext = setupHubContext;
            _logger = logger;

            Team1 = new Team("tm1", "Team 1", 1);
            _ordinalTeamMap.Add(1, Team1);

            Team2 = new Team("tm2", "Team 2", 2);
            _ordinalTeamMap.Add(2, Team2);
        }

        public Team GetTeamOne()
        {
            return Team1;
        }

        public Team GetTeamTwo()
        {
            return Team2;
        }

        public void UpdateTeamAlias(int teamOrdinal, string alias)
        {
            var team = _ordinalTeamMap[teamOrdinal];
            var oldAlias = team.Alias;
            
            team.Alias = alias;

            _logger.LogInformation($"Alias for Team {teamOrdinal} changed from {oldAlias} to {alias}");
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

            var player = await _scrimPlayers.GetPlayerFromCharacterId(characterId);

            if (player == null)
            {
                return false;
            }

            var team = _ordinalTeamMap[teamOrdinal];

            player.Team = team;

            if (team.TryAddPlayer(player))
            {
                _allCharacterIds.Add(characterId);
                _allPlayers.Add(player);

                await SendTeamPlayerAddedMessage(player);

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

            var players = await _scrimPlayers.GetPlayersFromOutfitAlias(alias);

            if (players == null || !players.Any())
            {
                return false;
            }

            var team = _ordinalTeamMap[teamOrdinal];
            var anyPlayersAdded = false;

            //TODO: track which players were added and which weren't

            foreach (var player in players)
            {
                player.Team = team;

                if (team.TryAddPlayer(player))
                {
                    _allCharacterIds.Add(player.Id);
                    _allPlayers.Add(player);

                    await SendTeamPlayerAddedMessage(player);

                    anyPlayersAdded = true;
                }
            }

            return anyPlayersAdded;
        }

        public async Task<bool> RemoveCharacterFromTeam(string characterId)
        {
            var player = GetPlayerFromId(characterId);

            if (player == null)
            {
                return false;
            }
            
            var team = player.Team;

            if(team.TryRemovePlayer(characterId))
            {
                _allCharacterIds.RemoveAll(id => id == characterId);
                _allPlayers.RemoveAll(p => p.Id == characterId);

                await SendTeamPlayerRemovedMessage(player);

                return true;
            }

            return false;
        }

        public bool IsCharacterAvailable(string characterId, out Team owningTeam)
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

        public bool IsOutfitAvailable(string alias, out Team owningTeam)
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

        public Player GetPlayerFromId(string characterId)
        {
            return _allPlayers.FirstOrDefault(p => p.Id == characterId);
        }

        private Team GetOtherTeamFromOrdinal(int initTeamOrdinal)
        {
            return initTeamOrdinal == 1 ? _ordinalTeamMap[2] : _ordinalTeamMap[1];
        }

        private async Task SendTeamPlayerAddedMessage(Player player)
        {
            var message = new TeamPlayerChangeMessage()
            {
                Player = player,
                TeamOrdinal = player.Team.TeamOrdinal,
                ChangeType = TeamPlayerChangeType.Add
            };

            await _setupHubContext.Clients.All.SendAsync("ReceiveMessage", message);
        }

        private async Task SendTeamPlayerRemovedMessage(Player player)
        {
            var message = new TeamPlayerChangeMessage()
            {
                Player = player,
                TeamOrdinal = player.Team.TeamOrdinal,
                ChangeType = TeamPlayerChangeType.Remove
            };

            await _setupHubContext.Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
