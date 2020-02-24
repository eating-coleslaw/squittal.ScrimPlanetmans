using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using squittal.ScrimPlanetmans.ScrimMatch.Events;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Shared.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimTeamsManager : IScrimTeamsManager, IDisposable
    {
        private readonly IScrimPlayersService _scrimPlayers;
        private readonly IOutfitService _outfitService;
        private readonly IFactionService _factionService;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<ScrimTeamsManager> _logger;

        private readonly Team Team1;
        private readonly Team Team2;

        private readonly Dictionary<int, Team> _ordinalTeamMap = new Dictionary<int, Team>();

        private readonly List<string> _allCharacterIds = new List<string>();

        private readonly List<Player> _allPlayers = new List<Player>();

        private readonly List<Player> _participatingPlayers = new List<Player>();

        private string _defaultAliasPreText = "tm";

        //private Dictionary<string, int> _characterTeamOrdinalMap;


        public ScrimTeamsManager(IScrimPlayersService scrimPlayers, IOutfitService outfitService, IFactionService factionService, IScrimMessageBroadcastService messageService, ILogger<ScrimTeamsManager> logger)
        {
            _scrimPlayers = scrimPlayers;
            _outfitService = outfitService;
            _factionService = factionService;
            _messageService = messageService;
            _logger = logger;

            Team1 = new Team($"{_defaultAliasPreText}1", "Team 1", 1);
            _ordinalTeamMap.Add(1, Team1);

            Team2 = new Team($"{_defaultAliasPreText}2", "Team 2", 2);
            _ordinalTeamMap.Add(2, Team2);
        }

        public Team GetTeam(int teamOrdinal)
        {
            return _ordinalTeamMap.GetValueOrDefault(teamOrdinal);
        }

        public string GetTeamAliasDisplay(int teamOrdinal)
        {
            var team = GetTeam(teamOrdinal);
            if (team == null)
            {
                return string.Empty;
            }

            return team.Alias;
        }

        public Team GetTeamOne()
        {
            return Team1;
        }

        public Team GetTeamTwo()
        {
            return Team2;
        }

        public Team GetTeamFromOutfitAlias(string aliasLower)
        {
            if (!IsOutfitAvailable(aliasLower, out Team owningTeam))
            {
                return owningTeam;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<string> GetAllPlayerIds()
        {
            return _allCharacterIds;
        }

        public IEnumerable<Player> GetParticipatingPlayers()
        {
            return _participatingPlayers;
        }

        private void UpdateTeamFaction(int teamOrdinal, int? factionId)
        {
            var team = GetTeam(teamOrdinal);
            var oldFactionId = team.FactionId;

            team.FactionId = factionId;

            var abbrev = factionId == null ? "null" : _factionService.GetFactionAbbrevFromId((int)factionId);

            var oldAbbrev = oldFactionId == null ? "null" : _factionService.GetFactionAbbrevFromId((int)oldFactionId);

            _logger.LogInformation($"Faction for Team {teamOrdinal} changed from {oldAbbrev} to {abbrev}");
        }

        public void UpdateTeamAlias(int teamOrdinal, string alias)
        {
            var team = _ordinalTeamMap[teamOrdinal];
            var oldAlias = team.Alias;
            
            team.Alias = alias;

            _logger.LogInformation($"Alias for Team {teamOrdinal} changed from {oldAlias} to {alias}");

            //TODO: add event for "Team X Alias Change"
        }

        //public void SubmitPlayersList()
        //{
        //    _wsMonitor.AddCharacterSubscriptions(_allCharacterIds);
        //}

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

            var team = GetTeam(teamOrdinal);

            //player.Team = team;
            player.TeamOrdinal = team.TeamOrdinal;


            if (team.TryAddPlayer(player))
            {
                _allCharacterIds.Add(characterId);
                _allPlayers.Add(player);

                if (team.FactionId == null)
                {
                    UpdateTeamFaction(teamOrdinal, player.FactionId);
                }

                SendTeamPlayerAddedMessage(player);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AddOutfitAliasToTeam(int teamOrdinal, string aliasLower, string alias)
        {
            if (!IsOutfitAvailable(aliasLower, out Team owningTeam))
            {
                return false;
            }

            /* Add Outfit to Team */
            var outfit = await _outfitService.GetOutfitByAlias(aliasLower);

            if (outfit == null)
            {
                return false;
            }

            var team = GetTeam(teamOrdinal); //_ordinalTeamMap[teamOrdinal];

            if (!team.TryAddOutfit(outfit))
            {
                return false;
            }

            // If not yet set, set team alias to alias of the first outfit added to it
            if (TeamOutfitCount(teamOrdinal) == 1 && team.Alias == $"{ _defaultAliasPreText}{teamOrdinal}")
            {
                UpdateTeamAlias(teamOrdinal, outfit.Alias);
            }

            if (team.FactionId == null && outfit.FactionId != null)
            {
                UpdateTeamFaction(teamOrdinal, outfit.FactionId);
            }

            /* Add Outfit Players to Team */
            var players = await _scrimPlayers.GetPlayersFromOutfitAlias(aliasLower);

            if (players == null || !players.Any())
            {
                return false;
            }

            var anyPlayersAdded = false;

            //TODO: track which players were added and which weren't

            foreach (var player in players)
            {
                //player.Team = team;
                player.TeamOrdinal = teamOrdinal; // team.TeamOrdinal;

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

        public async Task<bool> RefreshOutfitPlayers(string aliasLower)
        {
            if (IsOutfitAvailable(aliasLower, out Team outfitTeam))
            {
                return false;
            }

            var teamOrdinal = outfitTeam.TeamOrdinal;

            var players = await _scrimPlayers.GetPlayersFromOutfitAlias(aliasLower);

            if (players == null || !players.Any())
            {
                return false;
            }

            var anyPlayersAdded = false;

            //TODO: track which players were added and which weren't

            foreach (var player in players)
            {
                if (!IsCharacterAvailable(player.Id, out Team playerTeam))
                {
                    // TODO: broadcast "Couldn't Add Player to Team Message
                    continue;
                }
                
                //player.Team = team;
                player.TeamOrdinal = teamOrdinal; // team.TeamOrdinal;

                if (outfitTeam.TryAddPlayer(player))
                {
                    _allCharacterIds.Add(player.Id);
                    _allPlayers.Add(player);

                    SendTeamPlayerAddedMessage(player);

                    anyPlayersAdded = true;
                }
            }

            return anyPlayersAdded;

        }

        public bool RemoveOutfitFromTeam(string aliasLower)
        {
            var team = GetTeamFromOutfitAlias(aliasLower);

            if (team == null)
            {
                return false;
            }

            team.TryRemoveOutfit(aliasLower);

            var players = team.Players.Where(p => p.OutfitAliasLower == aliasLower).ToList();

            if (players == null || !players.Any())
            {
                return false;
            }

            var anyPlayersRemoved = false;

            foreach (var player in players)
            {
                if (RemovePlayerFromTeam(player))
                {
                    anyPlayersRemoved = true;
                }
            }

            if (team.Outfits.Any())
            {
                var nextOutfit = team.Outfits.FirstOrDefault();
                UpdateTeamAlias(team.TeamOrdinal, nextOutfit.Alias);
                UpdateTeamFaction(team.TeamOrdinal, nextOutfit.FactionId);
            }
            else if (team.Players.Any())
            {
                var nextPlayer = team.Players.FirstOrDefault();
                UpdateTeamAlias(team.TeamOrdinal, $"{_defaultAliasPreText}{team.TeamOrdinal}");
                UpdateTeamFaction(team.TeamOrdinal, nextPlayer.FactionId);
            }
            else
            {
                UpdateTeamAlias(team.TeamOrdinal, $"{_defaultAliasPreText}{team.TeamOrdinal}");
                UpdateTeamFaction(team.TeamOrdinal, null);
            }

            return anyPlayersRemoved;
        }

        public bool RemoveCharacterFromTeam(string characterId)
        {
            var player = GetPlayerFromId(characterId);

            if (player == null)
            {
                return false;
            }

            return RemovePlayerFromTeam(player);
        }

        public bool RemovePlayerFromTeam(Player player)
        {
            //var player = GetPlayerFromId(characterId);

            //if (player == null)
            //{
            //    return false;
            //}

            var team = GetTeam(player.TeamOrdinal); // player.Team;

            if(team.TryRemovePlayer(player.Id))
            {
                _allCharacterIds.RemoveAll(id => id == player.Id);
                _allPlayers.RemoveAll(p => p.Id == player.Id);

                if (_participatingPlayers.Any(p => p.Id == player.Id))
                {
                    _participatingPlayers.RemoveAll(p => p.Id == player.Id);
                }

                if (!team.Players.Any())
                {
                    UpdateTeamFaction(player.TeamOrdinal, null);
                }

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

        public bool IsPlayerTracked(string characterId)
        {
            return _allCharacterIds.Contains(characterId);
        }

        public Player GetPlayerFromId(string characterId)
        {
            return _allPlayers.FirstOrDefault(p => p.Id == characterId);
        }

        public int? GetTeamOrdinalFromPlayerId(string characterId)
        {
            var player = _allPlayers.FirstOrDefault(p => p.Id == characterId);
            if (player == null)
            {
                return null;
            }
            return player.TeamOrdinal;
        }

        public bool DoPlayersShareTeam(string firstId, string secondId, out int? firstOrdinal, out int? secondOrdinal)
        {
            firstOrdinal = GetTeamOrdinalFromPlayerId(firstId);
            secondOrdinal = GetTeamOrdinalFromPlayerId(secondId);

            if (firstOrdinal == null || secondOrdinal == null)
            {
                return false;
            }

            return firstOrdinal == secondOrdinal;
        }

        private int TeamOutfitCount(int teamOrdinal)
        {
            if (_ordinalTeamMap.TryGetValue(teamOrdinal, out Team team))
            {
                return team.Outfits.Count();
            }
            else
            {
                return -1;
            }
        }
        
        public void UpdatePlayerStats(string characterId, ScrimEventAggregate updates)
        {
            var player = GetPlayerFromId(characterId);

            player.EventAggregate.Add(updates);

            if (!_participatingPlayers.Any(p => p.Id == player.Id))
            {
                _participatingPlayers.Add(player);
            }

            var team = GetTeam((int)GetTeamOrdinalFromPlayerId(characterId));

            team.EventAggregate.Add(updates);

            //var teamStats = team.EventAggregate;

            if (!team.ParticipatingPlayers.Any(p => p.Id == player.Id))
            {
                team.ParticipatingPlayers.Add(player);
            }

            //teamStats.Add(updates);

            SendPlayerStatUpdateMessage(player);

            // TODO: broadcast Player stats update
            // TODO: broadcast Team stats update
        }

        public void SaveRoundEndScores(int round)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerOnlineStatus(string characterId, bool isOnline)
        {
            GetPlayerFromId(characterId).IsOnline = isOnline;
        }

        private bool TeamContainsOutfits(int teamOrdinal)
        {
            return TeamOutfitCount(teamOrdinal) > 0;
        }

        private void SendTeamPlayerAddedMessage(Player player)
        {
            var payload = new TeamPlayerChangeMessage(player, TeamPlayerChangeType.Add);
            _messageService.BroadcastTeamPlayerChangeMessage(payload);
        }

        private void SendTeamPlayerRemovedMessage(Player player)
        {
            var payload = new TeamPlayerChangeMessage(player, TeamPlayerChangeType.Remove);
            _messageService.BroadcastTeamPlayerChangeMessage(payload);
        }

        private void SendPlayerStatUpdateMessage(Player player)
        {
            var payload = new PlayerStatUpdateMessage(player);
            _messageService.BroadcastPlayerStatUpdateMessage(payload);
        }

        public void Dispose()
        {
            return;
        }
    }
}
