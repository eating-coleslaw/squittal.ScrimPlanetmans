using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Hubs;
//using squittal.ScrimPlanetmans.Hubs.Models;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using squittal.ScrimPlanetmans.ScrimMatch.Events;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimTeamsManager : IScrimTeamsManager, IDisposable
    {
        private readonly IScrimPlayersService _scrimPlayers;
        private readonly IWebsocketMonitor _wsMonitor;
        //private readonly IHubContext<MatchSetupHub> _hubContext;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly ILogger<ScrimTeamsManager> _logger;

        private readonly Team Team1;
        private readonly Team Team2;

        private readonly Dictionary<int, Team> _ordinalTeamMap = new Dictionary<int, Team>();

        private readonly List<string> _allCharacterIds = new List<string>();

        private readonly List<Player> _allPlayers = new List<Player>();

        //private Dictionary<string, int> _characterTeamOrdinalMap;

        public event EventHandler<TeamPlayerChangeEventArgs> RaiseTeamPlayerChangeEvent;
        public delegate void TeamPlayerChangeEventHandler(object sender, TeamPlayerChangeEventArgs e);

        public event EventHandler<SimpleMessageEventArgs> RaiseSimpleMessageEvent;
        public delegate void SimpleMessageEventHandler(object sender, SimpleMessageEventArgs e);

        public ScrimTeamsManager(IScrimPlayersService scrimPlayers, IWebsocketMonitor wsMonitor, IHubContext<EventHub> hubContext, ILogger<ScrimTeamsManager> logger)
        {
            _scrimPlayers = scrimPlayers;
            _wsMonitor = wsMonitor;
            _hubContext = hubContext;
            _logger = logger;

            Team1 = new Team("tm1", "Team 1", 1);
            _ordinalTeamMap.Add(1, Team1);

            Team2 = new Team("tm2", "Team 2", 2);
            _ordinalTeamMap.Add(2, Team2);
        }

        public Team GetTeam(int teamOrdinal)
        {
            return _ordinalTeamMap.GetValueOrDefault(teamOrdinal);
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

            //player.Team = team;
            player.TeamOrdinal = team.TeamOrdinal;


            if (team.TryAddPlayer(player))
            {
                _allCharacterIds.Add(characterId);
                _allPlayers.Add(player);

                SendTeamPlayerAddedMessage(player);

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
                //player.Team = team;
                player.TeamOrdinal = team.TeamOrdinal;

                if (team.TryAddPlayer(player))
                {
                    _allCharacterIds.Add(player.Id);
                    _allPlayers.Add(player);

                    SendTeamPlayerAddedMessage(player);

                    anyPlayersAdded = true;
                }
            }



            return anyPlayersAdded;
        }

        public bool RemoveCharacterFromTeam(string characterId)
        {
            var player = GetPlayerFromId(characterId);

            if (player == null)
            {
                return false;
            }

            var team = _ordinalTeamMap[player.TeamOrdinal]; // player.Team;

            if(team.TryRemovePlayer(characterId))
            {
                _allCharacterIds.RemoveAll(id => id == characterId);
                _allPlayers.RemoveAll(p => p.Id == characterId);

                SendTeamPlayerRemovedMessage(player);

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


        

        protected virtual void OnRaiseTeamPlayerChangeEvent(TeamPlayerChangeEventArgs e)
        {
            RaiseTeamPlayerChangeEvent?.Invoke(this, e);
        }

        private void SendTeamPlayerAddedMessage(Player player)
        {
            var payload = new TeamPlayerChangeMessage(player, TeamPlayerChangeType.Add);
            //payload.ChangeType = TeamPlayerChangeType.Add;

            OnRaiseTeamPlayerChangeEvent(new TeamPlayerChangeEventArgs(payload));

            _logger.LogDebug($"{payload.Info}");
            //_logger.LogDebug($"Sent TeamPlayerAdded Message: {player.NameFull} [{player.Id}]");
        }

        private void SendTeamPlayerRemovedMessage(Player player)
        {
            var payload = new TeamPlayerChangeMessage(player, TeamPlayerChangeType.Remove);
            //payload.ChangeType = TeamPlayerChangeType.Remove;

            OnRaiseTeamPlayerChangeEvent(new TeamPlayerChangeEventArgs(payload));

            _logger.LogDebug($"{payload.Info}");

            //_logger.LogInformation($"Sending TeamPlayerRemoved Message: {player.NameFull} [{player.Id}]");

            //await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

            //await _hubContext.Clients.All.SendAsync("ReceiveTeamPlayerChangeMessage", message);
            //await _hubContext.Clients.All.SendAsync("ReceiveMessage", $"Sending TeamPlayerAdded Message: {player.NameFull} [{player.Id}]");
        }

        protected virtual void OnRaiseSimpleMessageChangeEvent(SimpleMessageEventArgs e)
        {
            RaiseSimpleMessageEvent?.Invoke(this, e);
        }

        private void SendSimpleMessageAddedMessage(string s)
        {
            OnRaiseSimpleMessageChangeEvent(new SimpleMessageEventArgs(s));
        }

        public void Dispose()
        {
            return;
        }
    }
}
