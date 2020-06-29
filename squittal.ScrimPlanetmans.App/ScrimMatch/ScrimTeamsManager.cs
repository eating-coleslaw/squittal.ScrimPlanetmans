using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Data;
using Microsoft.EntityFrameworkCore;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimTeamsManager : IScrimTeamsManager, IDisposable
    {
        private readonly IScrimPlayersService _scrimPlayers;
        private readonly IOutfitService _outfitService;
        private readonly IFactionService _factionService;
        private readonly IConstructedTeamService _constructedTeamService;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IScrimMatchDataService _matchDataService;
        private readonly ILogger<ScrimTeamsManager> _logger;

        private readonly Team Team1;
        private readonly Team Team2;

        private readonly Dictionary<int, Team> _ordinalTeamMap = new Dictionary<int, Team>();

        //private readonly List<string> _allCharacterIds = new List<string>();

        private readonly List<Player> _allPlayers = new List<Player>();

        private ConcurrentDictionary<string, int> PlayerTeamOrdinalsMap { get; set; } = new ConcurrentDictionary<string, int>();

        //private readonly List<Player> _participatingPlayers = new List<Player>();

        private readonly string _defaultAliasPreText = "tm";

        public MaxPlayerPointsTracker MaxPlayerPointsTracker { get; private set; } = new MaxPlayerPointsTracker();


        public ScrimTeamsManager(IScrimPlayersService scrimPlayers, IOutfitService outfitService, IFactionService factionService,
            IScrimMessageBroadcastService messageService, IScrimMatchDataService matchDataService,
            IConstructedTeamService constructedTeamService, IDbContextHelper dbContextHelper, ILogger<ScrimTeamsManager> logger)
        {
            _scrimPlayers = scrimPlayers;
            _outfitService = outfitService;
            _factionService = factionService;
            _messageService = messageService;
            _matchDataService = matchDataService;
            _constructedTeamService = constructedTeamService;
            _dbContextHelper = dbContextHelper;
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

        public int? GetTeamScoreDisplay(int teamOrdinal)
        {
            var team = GetTeam(teamOrdinal);
            if (team == null)
            {
                return null;
            }

            return team.EventAggregate.Points;
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
        
        public Team GetTeamFromConstructedTeamFaction(int constructedTeamId, int factionId)
        {
            if (!IsConstructedTeamFactionAvailable(constructedTeamId, factionId, out Team owningTeam))
            {
                return owningTeam;
            }
            else
            {
                return null;
            }
        }

        public int? GetFirstTeamWithFactionId(int factionId)
        {
            foreach (var teamOrdinal in _ordinalTeamMap.Keys.ToList())
            {
                var teamFactionId = GetTeam(teamOrdinal).FactionId;
                if (factionId == teamFactionId)
                {
                    return teamOrdinal;
                }
            }

            return null;
        }

        public IEnumerable<string> GetAllPlayerIds()
        {
            var characterIds = new List<string>();
            
            foreach (var team in _ordinalTeamMap.Values)
            {
                characterIds.AddRange(team.GetAllPlayerIds());
            }

            return characterIds;

            //return _allCharacterIds;
        }

        public IEnumerable<Player> GetParticipatingPlayers()
        {
            //return _participatingPlayers;
            throw new NotImplementedException();
        }

        public IEnumerable<Player> GetTeamOutfitPlayers(int teamOrdinal, string outfitAliasLower)
        {
            return GetTeam(teamOrdinal).GetOutfitPlayers(outfitAliasLower);
        }
        
        public IEnumerable<Player> GetTeamNonOutfitPlayers(int teamOrdinal)
        {
            return GetTeam(teamOrdinal).GetNonOutfitPlayers();
        }
        
        public IEnumerable<Player> GetTeamConstructedTeamFactionPlayers(int teamOrdinal, int constructedTeamId, int factionId)
        {
            return GetTeam(teamOrdinal).GetConstructedTeamFactionPlayers(constructedTeamId, factionId);
        }

        public int? GetNextWorldId(int previousWorldId)
        {
            if (_allPlayers.Any(p => p.WorldId == previousWorldId))
            {
                return previousWorldId;
            }
            else if (_allPlayers.Any())
            {
                return _allPlayers.Where(p => p.WorldId > 0).Select(p => p.WorldId).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        private void UpdateTeamFaction(int teamOrdinal, int? factionId)
        {
            var team = GetTeam(teamOrdinal);
            var oldFactionId = team.FactionId;

            team.FactionId = factionId;

            var abbrev = factionId == null ? "null" : _factionService.GetFactionAbbrevFromId((int)factionId);

            var oldAbbrev = oldFactionId == null ? "null" : _factionService.GetFactionAbbrevFromId((int)oldFactionId);

            _messageService.BroadcastTeamFactionChangeMessage(new TeamFactionChangeMessage(teamOrdinal, factionId, abbrev, oldFactionId, oldAbbrev));

            _logger.LogInformation($"Faction for Team {teamOrdinal} changed from {oldAbbrev} to {abbrev}");
        }

        public bool UpdateTeamAlias(int teamOrdinal, string alias, bool isCustom = false)
        {
            var team = _ordinalTeamMap[teamOrdinal];
            var oldAlias = team.Alias;
            
            if (team.TrySetAlias(alias, isCustom))
            {
                _logger.LogInformation($"Alias for Team {teamOrdinal} changed from {oldAlias} to {alias}");
                _messageService.BroadcastTeamAliasChangeMessage(new TeamAliasChangeMessage(teamOrdinal, alias, oldAlias));
                return true;
            }
            else
            {
                _logger.LogInformation($"Couldn't change {team.NameInternal} display Alias: custom alias already set");
                return false;
            }
        }

        #pragma warning disable CS1998
        public async Task<bool> UdatePlayerTemporaryAlias(string playerId, string newAlias)
        {
            var player = GetPlayerFromId(playerId);
            var oldAlias = (player.NameDisplay != player.NameFull) ? player.NameDisplay : string.Empty;

            if (player.TrySetNameAlias(newAlias))
            {
                // Send message before updating database so UI/Overlay get updates faster
                _messageService.BroadcastPlayerNameDisplayChangeMessage(new PlayerNameDisplayChangeMessage(player, newAlias, oldAlias));

                if (player.IsParticipating)
                {
                    #pragma warning disable CS4014
                    Task.Run(() =>
                    {
                        _matchDataService.SaveMatchParticipatingPlayer(player);
                    }).ConfigureAwait(false);
                    #pragma warning restore CS4014
                }
                
                return true;
            }
            else
            {
                _messageService.BroadcastSimpleMessage($"<span style=\"color: red; font-weight: 700;\">Couldn't change {player.NameFull} match alias: new alias is invalid</span>");
                return false;
            }
        }
        #pragma warning restore CS1998

        #pragma warning disable CS1998
        public async Task ClearPlayerDisplayName(string playerId)
        {
            var player = GetPlayerFromId(playerId);

            if (string.IsNullOrWhiteSpace(player.NameTrimmed) && string.IsNullOrWhiteSpace(player.NameAlias))
            {
                return;
            }

            var oldAlias = (player.NameDisplay != player.NameFull) ? player.NameDisplay : string.Empty;

            player.ClearAllDisplayNameSources();

            // Send message before updating database so UI/Overlay get updates faster
            _messageService.BroadcastPlayerNameDisplayChangeMessage(new PlayerNameDisplayChangeMessage(player, string.Empty, oldAlias));

            if (player.IsParticipating)
            {
                #pragma warning disable CS4014
                Task.Run(() =>
                {
                    _matchDataService.SaveMatchParticipatingPlayer(player);
                }).ConfigureAwait(false);
                #pragma warning restore CS4014
            }    
        }
        #pragma warning restore CS1998

        public async Task<bool> TryAddFreeTextInputCharacterToTeam(int teamOrdinal, string inputString)
        {
            Regex idRegex = new Regex("[0-9]{19}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            bool isId = idRegex.Match(inputString).Success;

            if (isId)
            {
                if (await TryAddCharacterIdToTeam(teamOrdinal, inputString))
                {
                    return true;
                }
            }

            Regex nameRegex = new Regex("[A-Za-z0-9]{1,32}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            bool isName = nameRegex.Match(inputString).Success;

            if (isName)
            {
                return await TryAddCharacterNameToTeam(teamOrdinal, inputString);
            }

            return false;
        }

        public async Task<bool> TryAddCharacterIdToTeam(int teamOrdinal, string characterId)
        {
            if (!IsCharacterAvailable(characterId))
            {
                return false;
            }

            var player = await _scrimPlayers.GetPlayerFromCharacterId(characterId);

            if (player == null)
            {
                return false;
            }

            return TryAddPlayerToTeam(teamOrdinal, player);
        }

        public async Task<bool> TryAddCharacterNameToTeam(int teamOrdinal, string characterName)
        {
            var player = await _scrimPlayers.GetPlayerFromCharacterName(characterName);

            if (player == null)
            {
                return false;
            }

            if (!IsCharacterAvailable(player.Id))
            {
                return false;
            }

            return TryAddPlayerToTeam(teamOrdinal, player);
        }

        private bool TryAddPlayerToTeam(int teamOrdinal, Player player)
        {
            var team = GetTeam(teamOrdinal);

            player.TeamOrdinal = team.TeamOrdinal;

            player.IsOutfitless = IsPlayerOutfitless(player);

            if (team.TryAddPlayer(player))
            {
                //_allCharacterIds.Add(player.Id);
                _allPlayers.Add(player);

                PlayerTeamOrdinalsMap.TryAdd(player.Id, teamOrdinal);

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

        private bool IsPlayerOutfitless(Player player)
        {
            var aliasLower = player.OutfitAliasLower;

            if (IsOutfitAvailable(aliasLower))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #region Add Entities To Teams
        // Returns whether specified character was added to the specified team
        public async Task<bool> AddCharacterToTeam(int teamOrdinal, string characterId)
        {
            if (!IsCharacterAvailable(characterId))
            {
                return false;
            }

            var player = await _scrimPlayers.GetPlayerFromCharacterId(characterId);

            if (player == null)
            {
                return false;
            }

            var team = GetTeam(teamOrdinal);

            player.TeamOrdinal = team.TeamOrdinal;

            if (team.TryAddPlayer(player))
            {
                //_allCharacterIds.Add(characterId);
                _allPlayers.Add(player);

                PlayerTeamOrdinalsMap.TryAdd(player.Id, teamOrdinal);

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
            if (!IsOutfitAvailable(aliasLower))
            {
                return false;
            }

            /* Add Outfit to Team */
            var outfit = await _outfitService.GetOutfitByAlias(aliasLower);

            if (outfit == null)
            {
                return false;
            }

            outfit.TeamOrdinal = teamOrdinal;

            var team = GetTeam(teamOrdinal);

            if (!team.TryAddOutfit(outfit))
            {
                return false;
            }

            // If not yet set, set team alias to alias of the first outfit added to it
            if (TeamOutfitCount(teamOrdinal) == 1 && TeamConstructedTeamCount(teamOrdinal) == 0 && team.Alias == $"{ _defaultAliasPreText}{teamOrdinal}")
            {
                UpdateTeamAlias(teamOrdinal, outfit.Alias);
            }

            if (team.FactionId == null && outfit.FactionId != null)
            {
                UpdateTeamFaction(teamOrdinal, outfit.FactionId);
            }

            SendTeamOutfitAddedMessage(outfit);


            /* Add Outfit Players to Team */
            var loadStartedMessage = new TeamOutfitChangeMessage(outfit, TeamChangeType.OutfitMembersLoadStarted);
            _messageService.BroadcastTeamOutfitChangeMessage(loadStartedMessage);

            var loadCompleteMessage = new TeamOutfitChangeMessage(outfit, TeamChangeType.OutfitMembersLoadCompleted);

            var players = await _scrimPlayers.GetPlayersFromOutfitAlias(aliasLower);

            if (players == null || !players.Any())
            {
                _messageService.BroadcastTeamOutfitChangeMessage(loadCompleteMessage);
                return false;
            }

            var anyPlayersAdded = false;

            var lastPlayer = players.LastOrDefault();

            //TODO: track which players were added and which weren't

            foreach (var player in players)
            {
                player.TeamOrdinal = teamOrdinal;
                player.FactionId = (int)outfit.FactionId;
                player.WorldId = (int)outfit.WorldId;

                if (team.TryAddPlayer(player))
                {
                    //_allCharacterIds.Add(player.Id);
                    _allPlayers.Add(player);

                    PlayerTeamOrdinalsMap.TryAdd(player.Id, teamOrdinal);

                    var isLastPlayer = (player == lastPlayer);

                    SendTeamPlayerAddedMessage(player, isLastPlayer);

                    anyPlayersAdded = true;
                }
            }
            
            _messageService.BroadcastTeamOutfitChangeMessage(loadCompleteMessage);

            return anyPlayersAdded;
        }

        public async Task<bool> AddConstructedTeamFactionMembersToTeam(int teamOrdinal, int constructedTeamId, int factionId)
        {
            if (!IsConstructedTeamFactionAvailable(constructedTeamId, factionId))
            {
                return false;
            }

            var owningTeam = GetTeam(teamOrdinal);

            if (owningTeam == null)
            {
                return false;
            }

            var constructedTeam = await _constructedTeamService.GetConstructedTeam(constructedTeamId, true);

            if (constructedTeam == null)
            {
                return false;
            }

            var matchInfo = new ConstructedTeamMatchInfo
            {
                ConstructedTeam = constructedTeam,
                TeamOrdinal = teamOrdinal,
                ActiveFactionId = factionId
            };

            //if (!owningTeam.TryAddConstructedTeamMatchInfo(matchInfo))
            //{
            //    return false;
            //}

            if (!owningTeam.TryAddConstructedTeamFaction(matchInfo))
            {
                return false;
            }

            // If not yet set, set team alias to alias of the first constructed team added to it
            if (TeamOutfitCount(teamOrdinal) == 0 && TeamConstructedTeamCount(teamOrdinal) == 1 && owningTeam.Alias == $"{ _defaultAliasPreText}{teamOrdinal}")
            {
                UpdateTeamAlias(teamOrdinal, constructedTeam.Alias);
            }

            if (owningTeam.FactionId == null)
            {
                UpdateTeamFaction(teamOrdinal, factionId);
            }

            var message = new TeamConstructedTeamChangeMessage(teamOrdinal, constructedTeam, factionId, TeamChangeType.Add);
            _messageService.BroadcastTeamConstructedTeamChangeMessage(message);


            var loadStartedMessage = new TeamConstructedTeamChangeMessage(teamOrdinal, constructedTeam, factionId, TeamChangeType.ConstructedTeamMembersLoadStarted);
            _messageService.BroadcastTeamConstructedTeamChangeMessage(loadStartedMessage);

            var loadCompletedMessage = new TeamConstructedTeamChangeMessage(teamOrdinal, constructedTeam, factionId, TeamChangeType.ConstructedTeamMembersLoadCompleted);

            var players = await _constructedTeamService.GetConstructedTeamFactionPlayers(constructedTeamId, factionId);

            if (players == null || !players.Any())
            {
                _messageService.BroadcastTeamConstructedTeamChangeMessage(loadCompletedMessage);
                return false;
            }

            var anyPlayersAdded = false;
            var playersAddedCount = 0;

            var lastPlayer = players.LastOrDefault();

            //TODO: track which players were added and which weren't

            foreach (var player in players)
            {
                if (!IsCharacterAvailable(player.Id))
                {
                    continue;
                }
                
                player.TeamOrdinal = teamOrdinal;
                player.ConstructedTeamId = constructedTeamId;

                player.IsOutfitless = true; // IsPlayerOutfitless(player);

                if (owningTeam.TryAddPlayer(player))
                {
                    //_allCharacterIds.Add(player.Id);
                    _allPlayers.Add(player);

                    PlayerTeamOrdinalsMap.TryAdd(player.Id, teamOrdinal);

                    var isLastPlayer = (player == lastPlayer);

                    SendTeamPlayerAddedMessage(player, isLastPlayer);

                    anyPlayersAdded = true;
                    playersAddedCount += 1;
                }
            }

            loadCompletedMessage = new TeamConstructedTeamChangeMessage(teamOrdinal, constructedTeam, factionId, TeamChangeType.ConstructedTeamMembersLoadCompleted, playersAddedCount);

            _messageService.BroadcastTeamConstructedTeamChangeMessage(loadCompletedMessage);

            return anyPlayersAdded;
        }
        #endregion Add Entities To Teams

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

            var newPlayers = players.Where(p => IsCharacterAvailable(p.Id)).ToList();

            var oldPlayers = players.Where(p => !newPlayers.Contains(p)).ToList();

            foreach (var player in oldPlayers)
            {
                var oldOnlineStatus = GetPlayerFromId(player.Id).IsOnline;
                var newOnlineStatus = player.IsOnline;

                if (oldOnlineStatus != newOnlineStatus)
                {
                    SetPlayerOnlineStatus(player.Id, newOnlineStatus);
                }
            }

            if (!newPlayers.Any())
            {
                return false;
            }

            var lastPlayer = newPlayers.LastOrDefault();

            var outfit = outfitTeam.Outfits.Where(o => o.AliasLower == aliasLower)
                                                    //.Select(o => (int)o.FactionId)
                                                    .FirstOrDefault();

            var outfitFactionID = (int)outfit.FactionId;
            var outfitWorldId = (int)outfit.WorldId;

            var anyPlayersAdded = false;

            //TODO: track which players were added and which weren't

            foreach (var player in newPlayers)
            {
                var isLastPlayer = (player == lastPlayer);

                player.TeamOrdinal = teamOrdinal;
                player.FactionId = outfitFactionID;
                player.WorldId = outfitWorldId;

                if (outfitTeam.TryAddPlayer(player))
                {
                    //_allCharacterIds.Add(player.Id);
                    _allPlayers.Add(player);

                    PlayerTeamOrdinalsMap.TryAdd(player.Id, teamOrdinal);

                    SendTeamPlayerAddedMessage(player, isLastPlayer);

                    anyPlayersAdded = true;
                }
            }

            var newMemberCount = GetTeam(teamOrdinal).Players.Where(p => p.OutfitAliasLower == aliasLower && !p.IsOutfitless).Count();

            outfit.MemberCount = newMemberCount;

            var loadCompleteMessage = new TeamOutfitChangeMessage(outfit, TeamChangeType.OutfitMembersLoadCompleted);
            _messageService.BroadcastTeamOutfitChangeMessage(loadCompleteMessage);

            return anyPlayersAdded;
        }

        #region Remove Entities From Teams
        public async Task<bool> RemoveOutfitFromTeamAndDb(string aliasLower)
        {
            var outfit = GetTeamFromOutfitAlias(aliasLower).Outfits.FirstOrDefault(o => o.AliasLower == aliasLower);
            var outfitId = outfit.Id;
            var teamOrdinal = outfit.TeamOrdinal;

            var success = RemoveOutfitFromTeam(aliasLower);

            if (!success)
            {
                return false;
            }

            await RemoveOutfitMatchDataFromDb(outfitId, teamOrdinal);

            await UpdateMatchParticipatingPlayers();

            return true;
        }

        public bool RemoveOutfitFromTeam(string aliasLower)
        {
            var team = GetTeamFromOutfitAlias(aliasLower);

            if (team == null)
            {
                return false;
            }

            var outfit = team.Outfits.FirstOrDefault(o => o.AliasLower == aliasLower);

            team.TryRemoveOutfit(aliasLower);

            var players = team.Players.Where(p => p.OutfitAliasLower == aliasLower && !p.IsOutfitless).ToList();

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

            //TODO: handle updating Match Configuration's Server ID setting here
            if (team.ConstructedTeamsMatchInfo.Any())
            {
                var nextTeam = team.ConstructedTeamsMatchInfo.FirstOrDefault();
                UpdateTeamAlias(team.TeamOrdinal, nextTeam.ConstructedTeam.Alias);
                UpdateTeamFaction(team.TeamOrdinal, nextTeam.ActiveFactionId);
            }
            else if (team.Outfits.Any())
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

            SendTeamOutfitRemovedMessage(outfit);

            return anyPlayersRemoved;
        }

        private async Task RemoveOutfitMatchDataFromDb(string outfitId, int teamOrdinal)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            if (string.IsNullOrWhiteSpace(currentMatchId))
            {
                return;
            }

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var participatingPlayers = await dbContext.ScrimMatchParticipatingPlayers
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && e.TeamOrdinal == teamOrdinal
                                                                    && e.IsFromOutfit
                                                                    && e.OutfitId == outfitId)
                                                        .ToListAsync();

                List<Task> TaskList = new List<Task>();

                foreach (var player in participatingPlayers)
                {
                    var removePlayerTask = RemoveCharacterMatchDataFromDb(player.CharacterId, teamOrdinal);
                    TaskList.Add(removePlayerTask);
                }

                await Task.WhenAll(TaskList);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            //var TaskList = new List<Task>();

            //var deathsTask = RemoveOutfitMatchDeathsFromDb(outfitId, teamOrdinal);
            //TaskList.Add(deathsTask);

            //var destructionsTask = RemoveOutfitMatchVehicleDestructionsFromDb(outfitId);
            //TaskList.Add(destructionsTask);

            //await Task.WhenAll(TaskList);

            //var currentMatchRound = _matchDataService.CurrentMatchRound;

            //if (currentMatchRound > 0)
            //{
            //    await SaveTeamMatchResultsToDb(teamOrdinal);
            //}
        }

        private async Task RemoveOutfitMatchDeathsFromDb(string outfitId, int teamOrdinal)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;
            var currentMatchRound = _matchDataService.CurrentMatchRound;

            if (currentMatchRound <= 0)
            {
                return;
            }

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allDeathEvents = await dbContext.ScrimDeaths
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && (e.AttackerOutfitId == outfitId
                                                                        || e.VictimOutfitId == outfitId))
                                                        .ToListAsync();

                if (allDeathEvents == null || !allDeathEvents.Any())
                {
                    return;
                }

                #region Set Up Distinct Interaction Target Lists
                var distinctVictimTeams = allDeathEvents
                                            .Where(e => e.AttackerOutfitId == outfitId)
                                            .Select(e => e.VictimTeamOrdinal)
                                            .Distinct()
                                            .ToList();

                var distinctAttackerTeams = allDeathEvents
                                                .Where(e => e.VictimOutfitId == outfitId)
                                                .Select(e => e.AttackerTeamOrdinal)
                                                .Distinct()
                                                .ToList();

                var distinctTeams = new List<int>
                {
                    teamOrdinal
                };
                distinctTeams.AddRange(distinctAttackerTeams.Where(e => !distinctTeams.Contains(e)).ToList());
                distinctTeams.AddRange(distinctVictimTeams.Where(e => !distinctTeams.Contains(e)).ToList());


                var distinctVictimCharacterIds = allDeathEvents
                                            .Where(e => e.AttackerOutfitId == outfitId)
                                            .Select(e => e.VictimCharacterId)
                                            .Distinct()
                                            .ToList();

                var distinctAttackerCharacterIds = allDeathEvents
                                                .Where(e => e.VictimOutfitId == outfitId)
                                                .Select(e => e.AttackerCharacterId)
                                                .Distinct()
                                                .ToList();

                var distinctCharacterIds = new List<string>();
                distinctCharacterIds.AddRange(distinctAttackerCharacterIds);
                distinctCharacterIds.AddRange(distinctVictimCharacterIds.Where(e => !distinctCharacterIds.Contains(e)).ToList());
                #endregion Set Up Distinct Interaction Target Lists

                var teamUpdates = new Dictionary<int, ScrimEventAggregateRoundTracker>();
                var playerUpdates = new Dictionary<string, ScrimEventAggregateRoundTracker>();

                foreach (var team in distinctTeams)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    teamUpdates.Add(team, tracker);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    playerUpdates.Add(character, tracker);
                }


                for (var round = 1; round <= currentMatchRound; round++)
                {
                    foreach (var deathEvent in allDeathEvents.Where(e => e.ScrimMatchRound == round))
                    {
                        var attackerId = deathEvent.AttackerCharacterId;
                        var victimId = deathEvent.VictimCharacterId;

                        var attackerTeamOrdinal = deathEvent.AttackerTeamOrdinal;
                        var victimTeamOrdinal = deathEvent.VictimTeamOrdinal;

                        var deathType = deathEvent.DeathType;

                        var points = deathEvent.Points;
                        var isHeadshot = deathEvent.IsHeadshot ? 1 : 0;

                        var victimIsInOutfit = (deathEvent.VictimOutfitId == outfitId);

                        if (deathType == DeathEventType.Kill)
                        {
                            var attackerUpdate = new ScrimEventAggregate()
                            {
                                Points = points,
                                NetScore = points,
                                Kills = 1,
                                Headshots = isHeadshot
                            };

                            var victimUpdate = new ScrimEventAggregate()
                            {
                                NetScore = -points,
                                Deaths = 1,
                                HeadshotDeaths = isHeadshot
                            };

                            // Updating team stats for character being removed is already handled elsewhere
                            if (victimIsInOutfit)
                            {
                                teamUpdates[attackerTeamOrdinal].AddToCurrent(attackerUpdate);
                                playerUpdates[attackerId].AddToCurrent(attackerUpdate);
                            }
                            else
                            {
                                teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                                playerUpdates[victimId].AddToCurrent(victimUpdate);
                            }

                        }
                        else if (deathType == DeathEventType.Suicide)
                        {
                            var victimUpdate = new ScrimEventAggregate()
                            {
                                Points = points,
                                NetScore = points,
                                Deaths = 1,
                                Suicides = 1
                            };

                            if (!victimIsInOutfit)
                            {
                                teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                                playerUpdates[victimId].AddToCurrent(victimUpdate);

                            }
                        }
                        else if (deathType == DeathEventType.Teamkill)
                        {
                            var attackerUpdate = new ScrimEventAggregate()
                            {
                                Points = points,
                                NetScore = points,
                                Teamkills = 1
                            };

                            var victimUpdate = new ScrimEventAggregate()
                            {
                                Deaths = 1,
                                TeamkillDeaths = 1
                            };

                            if (victimIsInOutfit)
                            {
                                teamUpdates[attackerTeamOrdinal].AddToCurrent(attackerUpdate);
                                playerUpdates[attackerId].AddToCurrent(attackerUpdate);
                            }
                            else
                            {
                                teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                                playerUpdates[victimId].AddToCurrent(victimUpdate);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    foreach (var team in distinctTeams)
                    {
                        teamUpdates[team].SaveRoundToHistory(round);
                    }

                    foreach (var character in distinctCharacterIds)
                    {
                        playerUpdates[character].SaveRoundToHistory(round);
                    }
                }

                // Transfer the updates to the actual entities
                foreach (var tOrdinal in distinctTeams)
                {
                    var team = GetTeam(tOrdinal);
                    if (team == null)
                    {
                        continue;
                    }
                    team.EventAggregateTracker.SubtractFromHistory(teamUpdates[tOrdinal]);

                    SendTeamStatUpdateMessage(team);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var player = GetPlayerFromId(character);
                    if (player == null)
                    {
                        continue;
                    }
                    player.EventAggregateTracker.SubtractFromHistory(playerUpdates[character]);

                    SendPlayerStatUpdateMessage(player);
                }

                dbContext.ScrimDeaths.RemoveRange(allDeathEvents);

                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        private async Task RemoveOutfitMatchVehicleDestructionsFromDb(string outfitId)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var destructionsToRemove = await dbContext.ScrimVehicleDestructions
                                                            .Where(e => e.ScrimMatchId == currentMatchId
                                                                        && (e.AttackerOutfitId == outfitId
                                                                            || e.VictimOutfitId == outfitId))
                                                            .ToListAsync();

                dbContext.ScrimVehicleDestructions.RemoveRange(destructionsToRemove);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public async Task<bool> RemoveConstructedTeamFactionFromTeamAndDb(int constructedTeamId, int factionId)
        {
            var success = RemoveConstructedTeamFactionFromTeam(constructedTeamId, factionId);

            if (!success)
            {
                return false;
            }

            await RemoveConstructedTeamFactionMatchDataFromDb(constructedTeamId, factionId);

            await UpdateMatchParticipatingPlayers();

            return true;
        }

        public bool RemoveConstructedTeamFactionFromTeam(int constructedTeamId, int factionId)
        {
            //var team = GetTeamFromConstructedTeamId(constructedTeamId);
            var team = GetTeamFromConstructedTeamFaction(constructedTeamId, factionId);

            if (team == null)
            {
                return false;
            }

            var constructedTeamMatchInfo = team.ConstructedTeamsMatchInfo
                                                .Where(t => t.ConstructedTeam.Id == constructedTeamId && t.ActiveFactionId == factionId)
                                                .FirstOrDefault();

            team.TryRemoveConstructedTeamFaction(constructedTeamId, factionId);

            var players = team.GetConstructedTeamFactionPlayers(constructedTeamId, factionId).ToList();

            var anyPlayersRemoved = false;
            
            if (players != null && players.Any())
            {
                //return false;
                foreach (var player in players)
                {
                    if (RemovePlayerFromTeam(player))
                    {
                        anyPlayersRemoved = true;
                    }
                }
            }

            //TODO: handle updating Match Configuration's Server ID setting here
            if (team.ConstructedTeamsMatchInfo.Any())
            {
                var nextTeam = team.ConstructedTeamsMatchInfo.FirstOrDefault();
                UpdateTeamAlias(team.TeamOrdinal, nextTeam.ConstructedTeam.Alias);
                UpdateTeamFaction(team.TeamOrdinal, nextTeam.ActiveFactionId);
            }
            else if (team.Outfits.Any())
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

            SendTeamConstructedTeamRemovedMessage(team.TeamOrdinal, constructedTeamMatchInfo);

            return anyPlayersRemoved;
        }

        private async Task RemoveConstructedTeamFactionMatchDataFromDb(int constructedTeamId, int factionId)
        {
            var team = GetTeamFromConstructedTeamFaction(constructedTeamId, factionId);

            if (team == null)
            {
                return;
            }

            var teamOrdinal = team.TeamOrdinal;

            var currentMatchId = _matchDataService.CurrentMatchId;

            if (string.IsNullOrWhiteSpace(currentMatchId))
            {
                return;
            }

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var participatingPlayers = await dbContext.ScrimMatchParticipatingPlayers
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && e.IsFromConstructedTeam
                                                                    && e.ConstructedTeamId == constructedTeamId
                                                                    && e.FactionId == factionId)
                                                        .ToListAsync();

                List<Task> TaskList = new List<Task>();

                foreach (var player in participatingPlayers)
                {
                    var removePlayerTask = RemoveCharacterMatchDataFromDb(player.CharacterId, teamOrdinal);
                    TaskList.Add(removePlayerTask);
                }

                await Task.WhenAll(TaskList);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public async Task<bool> RemoveCharacterFromTeamAndDb(string characterId)
        {
            var teamOrdinal = GetTeamOrdinalFromPlayerId(characterId);
            
            var success = RemoveCharacterFromTeam(characterId);

            if (!success)
            {
                return false;
            }

            await RemoveCharacterMatchDataFromDb(characterId, (int)teamOrdinal);

            await UpdateMatchParticipatingPlayers();

            return true;
        }

        private async Task RemoveCharacterMatchDataFromDb(string characterId, int teamOrdinal)
        {
            var TaskList = new List<Task>();
            
            var deathsTask = RemoveCharacterMatchDeathsFromDb(characterId, teamOrdinal);
            TaskList.Add(deathsTask);

            var destructionsTask = RemoveCharacterMatchVehicleDestructionsFromDb(characterId);
            TaskList.Add(destructionsTask);

            var revivesTask = RemoveCharacterMatchRevivesFromDb(characterId, teamOrdinal);
            TaskList.Add(revivesTask);
            
            var damageAssistsTask = RemoveCharacterMatchDamageAssistsFromDb(characterId, teamOrdinal);
            TaskList.Add(damageAssistsTask);
            
            var grenadeAssistsTask = RemoveCharacterMatchGrenadeAssistsFromDb(characterId, teamOrdinal);
            TaskList.Add(grenadeAssistsTask);
            
            var spotAssistsTask = RemoveCharacterMatchSpotAssistsFromDb(characterId, teamOrdinal);
            TaskList.Add(spotAssistsTask);

            await Task.WhenAll(TaskList);

            await _matchDataService.TryRemoveMatchParticipatingPlayer(characterId); // TODO: add this to TaskList?

            var currentMatchRound = _matchDataService.CurrentMatchRound;

            if (currentMatchRound > 0)
            { 
                await SaveTeamMatchResultsToDb(teamOrdinal);
            }
        }

        private async Task RemoveCharacterMatchDeathsFromDb(string characterId, int teamOrdinal)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;
            var currentMatchRound = _matchDataService.CurrentMatchRound;

            if (currentMatchRound <= 0)
            {
                return;
            }

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allDeathEvents = await dbContext.ScrimDeaths
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && (e.AttackerCharacterId == characterId
                                                                        || e.VictimCharacterId == characterId))
                                                        .ToListAsync();

                if (allDeathEvents == null || !allDeathEvents.Any())
                {
                    return;
                }

                #region Set Up Distinct Interaction Target Lists
                var distinctVictimTeams = allDeathEvents
                                            .Where(e => e.AttackerCharacterId == characterId)
                                            .Select(e => e.VictimTeamOrdinal)
                                            .Distinct()
                                            .ToList();

                var distinctAttackerTeams = allDeathEvents
                                                .Where(e => e.VictimCharacterId == characterId)
                                                .Select(e => e.AttackerTeamOrdinal)
                                                .Distinct()
                                                .ToList();

                var distinctTeams = new List<int>();
                distinctTeams.Add(teamOrdinal);
                distinctTeams.AddRange(distinctAttackerTeams.Where(e => !distinctTeams.Contains(e)).ToList());
                distinctTeams.AddRange(distinctVictimTeams.Where(e => !distinctTeams.Contains(e)).ToList());


                var distinctVictimCharacterIds = allDeathEvents
                                            .Where(e => e.AttackerCharacterId == characterId)
                                            .Select(e => e.VictimCharacterId)
                                            .Distinct()
                                            .ToList();

                var distinctAttackerCharacterIds = allDeathEvents
                                                .Where(e => e.VictimCharacterId == characterId)
                                                .Select(e => e.AttackerCharacterId)
                                                .Distinct()
                                                .ToList();

                var distinctCharacterIds = new List<string>();
                distinctCharacterIds.AddRange(distinctAttackerCharacterIds);
                distinctCharacterIds.AddRange(distinctVictimCharacterIds.Where(e => !distinctCharacterIds.Contains(e)).ToList());
                #endregion Set Up Distinct Interaction Target Lists

                var teamUpdates = new Dictionary<int, ScrimEventAggregateRoundTracker>();
                var playerUpdates = new Dictionary<string, ScrimEventAggregateRoundTracker>();

                foreach (var team in distinctTeams)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    teamUpdates.Add(team, tracker);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    playerUpdates.Add(character, tracker);
                }


                for (var round = 1; round <= currentMatchRound; round++)
                {
                    foreach (var deathEvent in allDeathEvents.Where(e => e.ScrimMatchRound == round))
                    {
                        var attackerId = deathEvent.AttackerCharacterId;
                        var victimId = deathEvent.VictimCharacterId;

                        var attackerTeamOrdinal = deathEvent.AttackerTeamOrdinal;
                        var victimTeamOrdinal = deathEvent.VictimTeamOrdinal;

                        var deathType = deathEvent.DeathType;

                        var points = deathEvent.Points;
                        var isHeadshot = deathEvent.IsHeadshot? 1 : 0;

                        var characterIsVictim = (victimId == characterId);

                        if (deathType == DeathEventType.Kill)
                        {
                            var attackerUpdate = new ScrimEventAggregate()
                            {
                                Points = points,
                                NetScore = points,
                                Kills = 1,
                                Headshots = isHeadshot
                            };

                            var victimUpdate = new ScrimEventAggregate()
                            {
                                NetScore = -points,
                                Deaths = 1,
                                HeadshotDeaths = isHeadshot
                            };

                            // Updating team stats for character being removed is already handled elsewhere
                            if (characterIsVictim)
                            {
                                teamUpdates[attackerTeamOrdinal].AddToCurrent(attackerUpdate);
                                playerUpdates[attackerId].AddToCurrent(attackerUpdate);
                            }
                            else
                            {
                                teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                                playerUpdates[victimId].AddToCurrent(victimUpdate);
                            }

                        }
                        else if (deathType == DeathEventType.Suicide)
                        {
                            var victimUpdate = new ScrimEventAggregate()
                            {
                                Points = points,
                                NetScore = points,
                                Deaths = 1,
                                Suicides = 1
                            };

                            if (!characterIsVictim)
                            {
                                teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                                playerUpdates[victimId].AddToCurrent(victimUpdate);

                            }
                        }
                        else if (deathType == DeathEventType.Teamkill)
                        {
                            var attackerUpdate = new ScrimEventAggregate()
                            {
                                Points = points,
                                NetScore = points,
                                Teamkills = 1
                            };

                            var victimUpdate = new ScrimEventAggregate()
                            {
                                Deaths = 1,
                                TeamkillDeaths = 1
                            };

                            if (characterIsVictim)
                            {
                                teamUpdates[attackerTeamOrdinal].AddToCurrent(attackerUpdate);
                                playerUpdates[attackerId].AddToCurrent(attackerUpdate);
                            }
                            else
                            {
                                teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                                playerUpdates[victimId].AddToCurrent(victimUpdate);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    foreach (var team in distinctTeams)
                    {
                        teamUpdates[team].SaveRoundToHistory(round);
                    }

                    foreach (var character in distinctCharacterIds)
                    {
                        playerUpdates[character].SaveRoundToHistory(round);
                    }
                }

                // Transfer the updates to the actual entities
                foreach (var tOrdinal in distinctTeams)
                {
                    var team = GetTeam(tOrdinal);
                    team.EventAggregateTracker.SubtractFromHistory(teamUpdates[tOrdinal]);

                    SendTeamStatUpdateMessage(team);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var player = GetPlayerFromId(character);

                    if (player == null)
                    {
                        continue;
                    }

                    player.EventAggregateTracker.SubtractFromHistory(playerUpdates[character]);

                    SendPlayerStatUpdateMessage(player);
                }

                dbContext.ScrimDeaths.RemoveRange(allDeathEvents);

                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        private async Task RemoveCharacterMatchVehicleDestructionsFromDb(string characterId)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var destructionsToRemove = await dbContext.ScrimVehicleDestructions
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && (e.AttackerCharacterId == characterId
                                                                        || e.VictimCharacterId == characterId))
                                                        .ToListAsync();

                dbContext.ScrimVehicleDestructions.RemoveRange(destructionsToRemove);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task RemoveCharacterMatchRevivesFromDb(string characterId, int teamOrdinal)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;
            var currentMatchRound = _matchDataService.CurrentMatchRound;

            if (currentMatchRound <= 0)
            {
                return;
            }

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allReviveEvents = await dbContext.ScrimRevives
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && (e.MedicCharacterId == characterId
                                                                        || e.RevivedCharacterId == characterId))
                                                        .ToListAsync();

                if (allReviveEvents == null || !allReviveEvents.Any())
                {
                    return;
                }

                #region Set Up Distinct Interaction Target Lists
                var distinctRevivedTeams = allReviveEvents
                                            .Where(e => e.MedicCharacterId == characterId)
                                            .Select(e => e.RevivedTeamOrdinal)
                                            .Distinct()
                                            .ToList();

                var distinctMedicTeams = allReviveEvents
                                                .Where(e => e.RevivedCharacterId == characterId)
                                                .Select(e => e.MedicTeamOrdinal)
                                                .Distinct()
                                                .ToList();

                var distinctTeams = new List<int>();
                distinctTeams.Add(teamOrdinal);
                distinctTeams.AddRange(distinctMedicTeams.Where(e => !distinctTeams.Contains(e)).ToList());
                distinctTeams.AddRange(distinctRevivedTeams.Where(e => !distinctTeams.Contains(e)).ToList());


                var distinctRevivedCharacterIds = allReviveEvents
                                            .Where(e => e.MedicCharacterId == characterId)
                                            .Select(e => e.RevivedCharacterId)
                                            .Distinct()
                                            .ToList();

                var distinctMedicCharacterIds = allReviveEvents
                                                .Where(e => e.RevivedCharacterId == characterId)
                                                .Select(e => e.MedicCharacterId)
                                                .Distinct()
                                                .ToList();

                var distinctCharacterIds = new List<string>();
                distinctCharacterIds.AddRange(distinctMedicCharacterIds);
                distinctCharacterIds.AddRange(distinctRevivedCharacterIds.Where(e => !distinctCharacterIds.Contains(e)).ToList());
                #endregion Set Up Distinct Interaction Target Lists

                var teamUpdates = new Dictionary<int, ScrimEventAggregateRoundTracker>();
                var playerUpdates = new Dictionary<string, ScrimEventAggregateRoundTracker>();

                foreach (var team in distinctTeams)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    teamUpdates.Add(team, tracker);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    playerUpdates.Add(character, tracker);
                }


                for (var round = 1; round <= currentMatchRound; round++)
                {
                    foreach (var reviveEvent in allReviveEvents.Where(e => e.ScrimMatchRound == round))
                    {
                        var medicId = reviveEvent.MedicCharacterId;
                        var revivedId = reviveEvent.RevivedCharacterId;

                        var medicTeamOrdinal = reviveEvent.MedicTeamOrdinal;
                        var revivedTeamOrdinal = reviveEvent.RevivedTeamOrdinal;

                        var points = reviveEvent.Points;

                        var characterIsRevived = (revivedId == characterId);

                        var medicUpdate = new ScrimEventAggregate()
                        {
                            Points = points,
                            NetScore = points,
                            RevivesGiven = 1
                        };

                        var revivedUpdate = new ScrimEventAggregate()
                        {
                            NetScore = -points,
                            RevivesTaken = 1
                        };

                        // Updating team stats for character being removed is already handled elsewhere
                        if (characterIsRevived)
                        {
                            teamUpdates[medicTeamOrdinal].AddToCurrent(medicUpdate);
                            playerUpdates[medicId].AddToCurrent(medicUpdate);
                        }
                        else
                        {
                            teamUpdates[revivedTeamOrdinal].AddToCurrent(revivedUpdate);
                            playerUpdates[revivedId].AddToCurrent(revivedUpdate);
                        }
                    }

                    foreach (var team in distinctTeams)
                    {
                        teamUpdates[team].SaveRoundToHistory(round);
                    }

                    foreach (var character in distinctCharacterIds)
                    {
                        playerUpdates[character].SaveRoundToHistory(round);
                    }
                }

                // Transfer the updates to the actual entities
                foreach (var tOrdinal in distinctTeams)
                {
                    var team = GetTeam(tOrdinal);
                    team.EventAggregateTracker.SubtractFromHistory(teamUpdates[tOrdinal]);

                    SendTeamStatUpdateMessage(team);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var player = GetPlayerFromId(character);

                    if (player == null)
                    {
                        continue;
                    }

                    player.EventAggregateTracker.SubtractFromHistory(playerUpdates[character]);

                    SendPlayerStatUpdateMessage(player);
                }

                dbContext.ScrimRevives.RemoveRange(allReviveEvents);

                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        private async Task RemoveCharacterMatchDamageAssistsFromDb(string characterId, int teamOrdinal)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;
            var currentMatchRound = _matchDataService.CurrentMatchRound;

            if (currentMatchRound <= 0)
            {
                return;
            }

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allDamageAssistEvents = await dbContext.ScrimDamageAssists
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && (e.AttackerCharacterId == characterId
                                                                        || e.VictimCharacterId == characterId))
                                                        .ToListAsync();

                if (allDamageAssistEvents == null || !allDamageAssistEvents.Any())
                {
                    return;
                }

                #region Set Up Distinct Interaction Target Lists
                var distinctVictimTeams = allDamageAssistEvents
                                            .Where(e => e.AttackerCharacterId == characterId)
                                            .Select(e => e.VictimTeamOrdinal)
                                            .Distinct()
                                            .ToList();

                var distinctAttackerTeams = allDamageAssistEvents
                                                .Where(e => e.VictimCharacterId == characterId)
                                                .Select(e => e.AttackerTeamOrdinal)
                                                .Distinct()
                                                .ToList();

                var distinctTeams = new List<int>();
                distinctTeams.Add(teamOrdinal);
                distinctTeams.AddRange(distinctAttackerTeams.Where(e => !distinctTeams.Contains(e)).ToList());
                distinctTeams.AddRange(distinctVictimTeams.Where(e => !distinctTeams.Contains(e)).ToList());


                var distinctVictimCharacterIds = allDamageAssistEvents
                                            .Where(e => e.AttackerCharacterId == characterId)
                                            .Select(e => e.VictimCharacterId)
                                            .Distinct()
                                            .ToList();

                var distinctAttackerCharacterIds = allDamageAssistEvents
                                                .Where(e => e.VictimCharacterId == characterId)
                                                .Select(e => e.AttackerCharacterId)
                                                .Distinct()
                                                .ToList();

                var distinctCharacterIds = new List<string>();
                distinctCharacterIds.AddRange(distinctAttackerCharacterIds);
                distinctCharacterIds.AddRange(distinctVictimCharacterIds.Where(e => !distinctCharacterIds.Contains(e)).ToList());
                #endregion Set Up Distinct Interaction Target Lists

                var teamUpdates = new Dictionary<int, ScrimEventAggregateRoundTracker>();
                var playerUpdates = new Dictionary<string, ScrimEventAggregateRoundTracker>();

                foreach (var team in distinctTeams)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    teamUpdates.Add(team, tracker);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    playerUpdates.Add(character, tracker);
                }


                for (var round = 1; round <= currentMatchRound; round++)
                {
                    foreach (var damageAssistEvent in allDamageAssistEvents.Where(e => e.ScrimMatchRound == round))
                    {
                        var attackerId = damageAssistEvent.AttackerCharacterId;
                        var victimId = damageAssistEvent.VictimCharacterId;

                        var attackerTeamOrdinal = damageAssistEvent.AttackerTeamOrdinal;
                        var victimTeamOrdinal = damageAssistEvent.VictimTeamOrdinal;

                        var points = damageAssistEvent.Points;

                        var characterIsVictim = (victimId == characterId);

                        var attackerUpdate = new ScrimEventAggregate()
                        {
                            Points = points,
                            NetScore = points,
                            DamageAssists = 1
                        };

                        var victimUpdate = new ScrimEventAggregate()
                        {
                            NetScore = -points,
                            DamageAssistedDeaths = 1
                        };

                        // Updating team stats for character being removed is already handled elsewhere
                        if (characterIsVictim)
                        {
                            teamUpdates[attackerTeamOrdinal].AddToCurrent(attackerUpdate);
                            playerUpdates[attackerId].AddToCurrent(attackerUpdate);
                        }
                        else
                        {
                            teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                            playerUpdates[victimId].AddToCurrent(victimUpdate);
                        }
                    }

                    foreach (var team in distinctTeams)
                    {
                        teamUpdates[team].SaveRoundToHistory(round);
                    }

                    foreach (var character in distinctCharacterIds)
                    {
                        playerUpdates[character].SaveRoundToHistory(round);
                    }
                }

                // Transfer the updates to the actual entities
                foreach (var tOrdinal in distinctTeams)
                {
                    var team = GetTeam(tOrdinal);
                    team.EventAggregateTracker.SubtractFromHistory(teamUpdates[tOrdinal]);

                    SendTeamStatUpdateMessage(team);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var player = GetPlayerFromId(character);

                    if (player == null)
                    {
                        continue;
                    }

                    player.EventAggregateTracker.SubtractFromHistory(playerUpdates[character]);

                    SendPlayerStatUpdateMessage(player);
                }

                dbContext.ScrimDamageAssists.RemoveRange(allDamageAssistEvents);

                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        private async Task RemoveCharacterMatchGrenadeAssistsFromDb(string characterId, int teamOrdinal)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;
            var currentMatchRound = _matchDataService.CurrentMatchRound;

            if (currentMatchRound <= 0)
            {
                return;
            }

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allGrenadeAssistEvents = await dbContext.ScrimGrenadeAssists
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && (e.AttackerCharacterId == characterId
                                                                        || e.VictimCharacterId == characterId))
                                                        .ToListAsync();

                if (allGrenadeAssistEvents == null || !allGrenadeAssistEvents.Any())
                {
                    return;
                }

                #region Set Up Distinct Interaction Target Lists
                var distinctVictimTeams = allGrenadeAssistEvents
                                            .Where(e => e.AttackerCharacterId == characterId)
                                            .Select(e => e.VictimTeamOrdinal)
                                            .Distinct()
                                            .ToList();

                var distinctAttackerTeams = allGrenadeAssistEvents
                                                .Where(e => e.VictimCharacterId == characterId)
                                                .Select(e => e.AttackerTeamOrdinal)
                                                .Distinct()
                                                .ToList();

                var distinctTeams = new List<int>();
                distinctTeams.Add(teamOrdinal);
                distinctTeams.AddRange(distinctAttackerTeams.Where(e => !distinctTeams.Contains(e)).ToList());
                distinctTeams.AddRange(distinctVictimTeams.Where(e => !distinctTeams.Contains(e)).ToList());


                var distinctVictimCharacterIds = allGrenadeAssistEvents
                                            .Where(e => e.AttackerCharacterId == characterId)
                                            .Select(e => e.VictimCharacterId)
                                            .Distinct()
                                            .ToList();

                var distinctAttackerCharacterIds = allGrenadeAssistEvents
                                                .Where(e => e.VictimCharacterId == characterId)
                                                .Select(e => e.AttackerCharacterId)
                                                .Distinct()
                                                .ToList();

                var distinctCharacterIds = new List<string>();
                distinctCharacterIds.AddRange(distinctAttackerCharacterIds);
                distinctCharacterIds.AddRange(distinctVictimCharacterIds.Where(e => !distinctCharacterIds.Contains(e)).ToList());
                #endregion Set Up Distinct Interaction Target Lists

                var teamUpdates = new Dictionary<int, ScrimEventAggregateRoundTracker>();
                var playerUpdates = new Dictionary<string, ScrimEventAggregateRoundTracker>();

                foreach (var team in distinctTeams)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    teamUpdates.Add(team, tracker);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    playerUpdates.Add(character, tracker);
                }


                for (var round = 1; round <= currentMatchRound; round++)
                {
                    foreach (var grenadeAssistEvent in allGrenadeAssistEvents.Where(e => e.ScrimMatchRound == round))
                    {
                        var attackerId = grenadeAssistEvent.AttackerCharacterId;
                        var victimId = grenadeAssistEvent.VictimCharacterId;

                        var attackerTeamOrdinal = grenadeAssistEvent.AttackerTeamOrdinal;
                        var victimTeamOrdinal = grenadeAssistEvent.VictimTeamOrdinal;

                        var points = grenadeAssistEvent.Points;

                        var characterIsVictim = (victimId == characterId);

                        var attackerUpdate = new ScrimEventAggregate()
                        {
                            Points = points,
                            NetScore = points,
                            GrenadeAssists = 1
                        };

                        var victimUpdate = new ScrimEventAggregate()
                        {
                            NetScore = -points,
                            GrenadeAssistedDeaths = 1
                        };

                        // Updating team stats for character being removed is already handled elsewhere
                        if (characterIsVictim)
                        {
                            teamUpdates[attackerTeamOrdinal].AddToCurrent(attackerUpdate);
                            playerUpdates[attackerId].AddToCurrent(attackerUpdate);
                        }
                        else
                        {
                            teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                            playerUpdates[victimId].AddToCurrent(victimUpdate);
                        }
                    }

                    foreach (var team in distinctTeams)
                    {
                        teamUpdates[team].SaveRoundToHistory(round);
                    }

                    foreach (var character in distinctCharacterIds)
                    {
                        playerUpdates[character].SaveRoundToHistory(round);
                    }
                }

                // Transfer the updates to the actual entities
                foreach (var tOrdinal in distinctTeams)
                {
                    var team = GetTeam(tOrdinal);
                    team.EventAggregateTracker.SubtractFromHistory(teamUpdates[tOrdinal]);

                    SendTeamStatUpdateMessage(team);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var player = GetPlayerFromId(character);

                    if (player == null)
                    {
                        continue;
                    }

                    player.EventAggregateTracker.SubtractFromHistory(playerUpdates[character]);

                    SendPlayerStatUpdateMessage(player);
                }

                dbContext.ScrimGrenadeAssists.RemoveRange(allGrenadeAssistEvents);

                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        private async Task RemoveCharacterMatchSpotAssistsFromDb(string characterId, int teamOrdinal)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;
            var currentMatchRound = _matchDataService.CurrentMatchRound;

            if (currentMatchRound <= 0)
            {
                return;
            }

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allSpotAssistEvents = await dbContext.ScrimSpotAssists
                                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                                    && (e.SpotterCharacterId == characterId
                                                                        || e.VictimCharacterId == characterId))
                                                        .ToListAsync();

                if (allSpotAssistEvents == null || !allSpotAssistEvents.Any())
                {
                    return;
                }

                #region Set Up Distinct Interaction Target Lists
                var distinctVictimTeams = allSpotAssistEvents
                                            .Where(e => e.SpotterCharacterId == characterId)
                                            .Select(e => e.VictimTeamOrdinal)
                                            .Distinct()
                                            .ToList();

                var distinctSpotterTeams = allSpotAssistEvents
                                                .Where(e => e.VictimCharacterId == characterId)
                                                .Select(e => e.SpotterTeamOrdinal)
                                                .Distinct()
                                                .ToList();

                var distinctTeams = new List<int>();
                distinctTeams.Add(teamOrdinal);
                distinctTeams.AddRange(distinctSpotterTeams.Where(e => !distinctTeams.Contains(e)).ToList());
                distinctTeams.AddRange(distinctVictimTeams.Where(e => !distinctTeams.Contains(e)).ToList());


                var distinctVictimCharacterIds = allSpotAssistEvents
                                            .Where(e => e.SpotterCharacterId == characterId)
                                            .Select(e => e.VictimCharacterId)
                                            .Distinct()
                                            .ToList();

                var distinctSpotterCharacterIds = allSpotAssistEvents
                                                .Where(e => e.VictimCharacterId == characterId)
                                                .Select(e => e.SpotterCharacterId)
                                                .Distinct()
                                                .ToList();

                var distinctCharacterIds = new List<string>();
                distinctCharacterIds.AddRange(distinctSpotterCharacterIds);
                distinctCharacterIds.AddRange(distinctVictimCharacterIds.Where(e => !distinctCharacterIds.Contains(e)).ToList());
                #endregion Set Up Distinct Interaction Target Lists

                var teamUpdates = new Dictionary<int, ScrimEventAggregateRoundTracker>();
                var playerUpdates = new Dictionary<string, ScrimEventAggregateRoundTracker>();

                foreach (var team in distinctTeams)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    teamUpdates.Add(team, tracker);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var tracker = new ScrimEventAggregateRoundTracker();
                    playerUpdates.Add(character, tracker);
                }


                for (var round = 1; round <= currentMatchRound; round++)
                {
                    foreach (var spotAssistEvent in allSpotAssistEvents.Where(e => e.ScrimMatchRound == round))
                    {
                        var spotterId = spotAssistEvent.SpotterCharacterId;
                        var victimId = spotAssistEvent.VictimCharacterId;

                        var spotterTeamOrdinal = spotAssistEvent.SpotterTeamOrdinal;
                        var victimTeamOrdinal = spotAssistEvent.VictimTeamOrdinal;

                        var points = spotAssistEvent.Points;

                        var characterIsVictim = (victimId == characterId);

                        var spotterUpdate = new ScrimEventAggregate()
                        {
                            Points = points,
                            NetScore = points,
                            SpotAssists = 1
                        };

                        var victimUpdate = new ScrimEventAggregate()
                        {
                            NetScore = -points,
                            SpotAssistedDeaths = 1
                        };

                        // Updating team stats for character being removed is already handled elsewhere
                        if (characterIsVictim)
                        {
                            teamUpdates[spotterTeamOrdinal].AddToCurrent(spotterUpdate);
                            playerUpdates[spotterId].AddToCurrent(spotterUpdate);
                        }
                        else
                        {
                            teamUpdates[victimTeamOrdinal].AddToCurrent(victimUpdate);
                            playerUpdates[victimId].AddToCurrent(victimUpdate);
                        }
                    }

                    foreach (var team in distinctTeams)
                    {
                        teamUpdates[team].SaveRoundToHistory(round);
                    }

                    foreach (var character in distinctCharacterIds)
                    {
                        playerUpdates[character].SaveRoundToHistory(round);
                    }
                }

                // Transfer the updates to the actual entities
                foreach (var tOrdinal in distinctTeams)
                {
                    var team = GetTeam(tOrdinal);
                    team.EventAggregateTracker.SubtractFromHistory(teamUpdates[tOrdinal]);

                    SendTeamStatUpdateMessage(team);
                }

                foreach (var character in distinctCharacterIds)
                {
                    var player = GetPlayerFromId(character);

                    if (player == null)
                    {
                        continue;
                    }

                    player.EventAggregateTracker.SubtractFromHistory(playerUpdates[character]);

                    SendPlayerStatUpdateMessage(player);
                }

                dbContext.ScrimSpotAssists.RemoveRange(allSpotAssistEvents);

                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        public bool RemoveCharacterFromTeam(string characterId)
        {
            var player = GetPlayerFromId(characterId);

            if (player == null)
            {
                return false;
            }

            var team = GetTeam(player.TeamOrdinal);

            if ( RemovePlayerFromTeam(player))
            {
                if (characterId == MaxPlayerPointsTracker.GetOwningCharacterId())
                {
                    // TODO: Update Match Max Player Points
                }

                if (team.ConstructedTeamsMatchInfo.Any())
                {
                    var nextTeam = team.ConstructedTeamsMatchInfo.FirstOrDefault();
                    UpdateTeamFaction(team.TeamOrdinal, nextTeam.ActiveFactionId);
                }
                else if (team.Outfits.Any())
                {
                    var nextOutfit = team.Outfits.FirstOrDefault();
                    UpdateTeamFaction(team.TeamOrdinal, nextOutfit.FactionId);
                }
                else if (team.Players.Any())
                {
                    var nextPlayer = team.Players.FirstOrDefault();
                    UpdateTeamFaction(team.TeamOrdinal, nextPlayer.FactionId);
                }
                else
                {
                    UpdateTeamFaction(team.TeamOrdinal, null);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemovePlayerFromTeam(Player player)
        {
            var team = GetTeam(player.TeamOrdinal);

            if(team.TryRemovePlayer(player.Id))
            {
                //_allCharacterIds.RemoveAll(id => id == player.Id);
                _allPlayers.RemoveAll(p => p.Id == player.Id);

                PlayerTeamOrdinalsMap.TryRemove(player.Id, out var ordinalOut);

                //if (_participatingPlayers.Any(p => p.Id == player.Id))
                //{
                //    _participatingPlayers.RemoveAll(p => p.Id == player.Id);
                //}

                if (!team.Players.Any())
                {
                    UpdateTeamFaction(player.TeamOrdinal, null);
                }

                SendTeamPlayerRemovedMessage(player);

                return true;
            }

            return false;
        }

        private async Task UpdateMatchParticipatingPlayers()
        {
            var matchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allMatchParticipatingPlayerIds = await dbContext.ScrimMatchParticipatingPlayers
                                                            .Where(e => e.ScrimMatchId == matchId)
                                                            .Select(e => e.CharacterId)
                                                            .ToListAsync();

                if (!allMatchParticipatingPlayerIds.Any())
                {
                    return;
                }

                var TaskList = new List<Task>();

                foreach (var playerId in allMatchParticipatingPlayerIds)
                {
                    var player = GetPlayerFromId(playerId);

                    if (!player.EventAggregateTracker.RoundHistory.Any() || player.EventAggregate.Events == 0)
                    {
                        var playerTask = SetPlayerParticipatingStatus(playerId, false);
                        TaskList.Add(playerTask);
                    }
                }

                await Task.WhenAll(TaskList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }
        }
        #endregion Remove Entities From Teams


        #region Clear Teams
        public void ClearAllTeams()
        {
            foreach (var teamOrdinal in _ordinalTeamMap.Keys.ToList())
            {
                ClearTeam(teamOrdinal);
            }

            MaxPlayerPointsTracker = new MaxPlayerPointsTracker();
        }

        public void ClearTeam(int teamOrdinal)
        {
            var team = GetTeam(teamOrdinal);

            if (team == null)
            {
                return;
            }

            var constructedTeamsMatchInfo = team.ConstructedTeamsMatchInfo.ToList();

            foreach(var matchInfo in constructedTeamsMatchInfo)
            {
                RemoveConstructedTeamFactionFromTeam(matchInfo.ConstructedTeam.Id, matchInfo.ActiveFactionId);
            }

            var allAliases = team.Outfits.Select(o => o.AliasLower).ToList();

            foreach (var alias in allAliases)
            {
                RemoveOutfitFromTeam(alias);
            }
            
            // if there are no outfitless players, we're done
            if (!team.Players.Any())
            {
                team.ClearEventAggregateHistory();

                // TODO: broadcast "Finished Clearing Team" message
                return;
            }

            var allPlayers = team.Players.ToList();

            foreach (var player in allPlayers)
            {
                RemovePlayerFromTeam(player);
            }

            team.ClearEventAggregateHistory();
            // TODO: broadcast "Finished Clearing Team" message
        }
        #endregion Clear Teams

        #region Roll Back Round
        public async Task RollBackAllTeamStats(int currentRound)
        {
            var TaskList = new List<Task>();
            
            foreach (var teamOrdinal in _ordinalTeamMap.Keys.ToList())
            {
                RollBackTeamStats(teamOrdinal, currentRound);

                //await SaveTeamMatchResultsToDb(teamOrdinal);
                var teamTask = SaveTeamMatchResultsToDb(teamOrdinal);
                TaskList.Add(teamTask);
            }

            var eventsDbTask = RemoveAllMatchRoundEventsFromDb(currentRound);
            TaskList.Add(eventsDbTask);

            var participatingPlayersTask = UpdateMatchParticipatingPlayers();
            TaskList.Add(participatingPlayersTask);

            await Task.WhenAll(TaskList);
        }

        public void RollBackTeamStats(int teamOrdinal, int currentRound)
        {
            var team = GetTeam(teamOrdinal);

            if (team == null)
            {
                return;
            }

            team.EventAggregateTracker.RollBackRound(currentRound);
            
            var players = team.ParticipatingPlayers.ToList();

            foreach (var player in players)
            {
                player.EventAggregateTracker.RollBackRound(currentRound);

                team.ParticipatingPlayers.RemoveAll(p => p.Id == player.Id);

                //_participatingPlayers.RemoveAll(p => p.Id == player.Id);

                SendPlayerStatUpdateMessage(player);
            }

            var maxPointsChanged = TryUpdateMaxPlayerPointsTrackerFromTeam(teamOrdinal);

            var overlayMessageData = new OverlayMessageData
            {
                RedrawPointGraph = maxPointsChanged,
                MatchMaxPlayerPoints = MaxPlayerPointsTracker.GetMaxPoints()
            };

            SendTeamStatUpdateMessage(team, overlayMessageData);
        }

        private async Task RemoveAllMatchRoundEventsFromDb(int roundToRemove)
        {
            var TaskList = new List<Task>();

            var deathsTask = RemoveAllMatchRoundDeathsFromDb(roundToRemove);
            TaskList.Add(deathsTask);

            var destructionsTask = RemoveAllMatchRoundVehicleDestructionsFromDb(roundToRemove);
            TaskList.Add(destructionsTask);
            
            var revivesTask = RemoveAllMatchRoundRevivesFromDb(roundToRemove);
            TaskList.Add(revivesTask);
            
            var damageAssistsTask = RemoveAllMatchRoundDamageAssistsFromDb(roundToRemove);
            TaskList.Add(damageAssistsTask);
            
            var grenadeAssistsTask = RemoveAllMatchRoundGrenadeAssistsFromDb(roundToRemove);
            TaskList.Add(grenadeAssistsTask);
            
            var spotAssistsTask = RemoveAllMatchRoundSpotAssistsFromDb(roundToRemove);
            TaskList.Add(spotAssistsTask);
            
            var controlsTask = RemoveAllMatchRoundFacilityControlsFromDb(roundToRemove);
            TaskList.Add(controlsTask);

            await Task.WhenAll(TaskList);
        }

        private async Task RemoveAllMatchRoundDeathsFromDb(int roundToRemove)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allDeathEvents = dbContext.ScrimDeaths
                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                    && e.ScrimMatchRound == roundToRemove)
                                        .AsEnumerable();

                dbContext.ScrimDeaths.RemoveRange(allDeathEvents);

                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }

        private async Task RemoveAllMatchRoundVehicleDestructionsFromDb(int roundToRemove)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var destructionsToRemove = dbContext.ScrimVehicleDestructions
                                                .Where(e => e.ScrimMatchId == currentMatchId
                                                            && e.ScrimMatchRound == roundToRemove)
                                                .AsEnumerable();

                dbContext.ScrimVehicleDestructions.RemoveRange(destructionsToRemove);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task RemoveAllMatchRoundRevivesFromDb(int roundToRemove)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var revivesToRemove = dbContext.ScrimRevives
                                                .Where(e => e.ScrimMatchId == currentMatchId
                                                            && e.ScrimMatchRound == roundToRemove)
                                                .AsEnumerable();

                dbContext.ScrimRevives.RemoveRange(revivesToRemove);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task RemoveAllMatchRoundDamageAssistsFromDb(int roundToRemove)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var damageAssistsToRemove = dbContext.ScrimDamageAssists
                                                .Where(e => e.ScrimMatchId == currentMatchId
                                                            && e.ScrimMatchRound == roundToRemove)
                                                .AsEnumerable();

                dbContext.ScrimDamageAssists.RemoveRange(damageAssistsToRemove);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task RemoveAllMatchRoundGrenadeAssistsFromDb(int roundToRemove)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var grenadeAssistsToRemove = dbContext.ScrimGrenadeAssists
                                                .Where(e => e.ScrimMatchId == currentMatchId
                                                            && e.ScrimMatchRound == roundToRemove)
                                                .AsEnumerable();

                dbContext.ScrimGrenadeAssists.RemoveRange(grenadeAssistsToRemove);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task RemoveAllMatchRoundSpotAssistsFromDb(int roundToRemove)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var spotAssistsToRemove = dbContext.ScrimSpotAssists
                                                .Where(e => e.ScrimMatchId == currentMatchId
                                                            && e.ScrimMatchRound == roundToRemove)
                                                .AsEnumerable();

                dbContext.ScrimSpotAssists.RemoveRange(spotAssistsToRemove);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task RemoveAllMatchRoundFacilityControlsFromDb(int roundToRemove)
        {
            var currentMatchId = _matchDataService.CurrentMatchId;

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var allControlEvents = dbContext.ScrimFacilityControls
                                        .Where(e => e.ScrimMatchId == currentMatchId
                                                    && e.ScrimMatchRound == roundToRemove)
                                        .AsEnumerable();

                dbContext.ScrimFacilityControls.RemoveRange(allControlEvents);

                await dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return;
            }
        }
        #endregion Roll Back Round

        private bool TryUpdateMaxPlayerPointsTrackerFromTeam(int teamOrdinal)
        {
            var team = GetTeam(teamOrdinal);
            var maxTeamPointsPlayer = team.Players.Where(p => p.EventAggregate.Points == team.Players.Select(ip => ip.EventAggregate.Points).Max()).FirstOrDefault();

            if (maxTeamPointsPlayer == null)
            {
                return false;
            }

            return MaxPlayerPointsTracker.TryUpdateMaxPoints(maxTeamPointsPlayer.EventAggregate.Points, maxTeamPointsPlayer.Id);
        }

        #region Match Entity Availability Methods
        public bool IsCharacterAvailable(string characterId)
        {
            foreach (var team in _ordinalTeamMap.Values)
            {
                if (team.ContainsPlayer(characterId))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsCharacterAvailable(string characterId, out Team owningTeam)
        {
            foreach (var team in _ordinalTeamMap.Values)
            {
                if (team.ContainsPlayer(characterId))
                {
                    owningTeam = team;
                    return false;
                }
            }

            owningTeam = null;
            return true;
        }

        public bool IsOutfitAvailable(string alias)
        {
            foreach (var team in _ordinalTeamMap.Values)
            {
                if (team.ContainsOutfit(alias))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsOutfitAvailable(string alias, out Team owningTeam)
        {
            foreach (var team in _ordinalTeamMap.Values)
            {
                if (team.ContainsOutfit(alias))
                {
                    owningTeam = team;
                    return false;
                }
            }

            owningTeam = null;
            return true;
        }

        public bool IsConstructedTeamFactionAvailable(int constructedTeamId, int factionId)
        {
            foreach (var team in _ordinalTeamMap.Values)
            {
                if (team.ContainsConstructedTeamFaction(constructedTeamId, factionId))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsConstructedTeamFactionAvailable(int constructedTeamId, int factionId, out Team owningTeam)
        {
            foreach (var team in _ordinalTeamMap.Values)
            {
                if (team.ContainsConstructedTeamFaction(constructedTeamId, factionId))
                {
                    owningTeam = team;
                    return false;
                }
            }

            owningTeam = null;
            return true;
        }

        public bool IsConstructedTeamAnyFactionAvailable(int constructedTeamId)
        {
            for (var factionId = 1; factionId <=3; factionId++)
            {
                if (IsConstructedTeamFactionAvailable(constructedTeamId, factionId))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Match Entity Availability Methods

        public Player GetPlayerFromId(string characterId)
        {
            //return _allPlayers.FirstOrDefault(p => p.Id == characterId);

            var teamOrdinal = GetTeamOrdinalFromPlayerId(characterId);
            if (teamOrdinal == null)
            {
                return null;
            }

            var team = GetTeam((int)teamOrdinal);
            if (team == null)
            {
                return null;
            }

            team.TryGetPlayerFromId(characterId, out var player);

            return player;
        }

        public int? GetTeamOrdinalFromPlayerId(string characterId)
        {
            //var player = _allPlayers.FirstOrDefault(p => p.Id == characterId);
            //if (player == null)
            //{
            //    return null;
            //}
            //return player.TeamOrdinal;

            if (PlayerTeamOrdinalsMap.TryGetValue(characterId, out var teamOrdinal))
            {
                return teamOrdinal;
            }
            else
            {
                return null;
            }
        }

        public bool DoPlayersShareTeam(Player firstPlayer, Player secondPlayer)
        {
            if (firstPlayer == null || secondPlayer == null)
            {
                return false;
            }

            return firstPlayer.TeamOrdinal == secondPlayer.TeamOrdinal;
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
        
        private int TeamConstructedTeamCount(int teamOrdinal)
        {
            var team = GetTeam(teamOrdinal);

            if (team != null)
            {
                return team.ConstructedTeamsMatchInfo.Count();
            }
            else
            {
                return -1;
            }
        }

        public async Task UpdatePlayerStats(string characterId, ScrimEventAggregate updates)
        {
            var player = GetPlayerFromId(characterId);

            player.AddStatsUpdate(updates);

            if (!player.IsBenched)
            {
                player.IsActive = true;
            }

            //player.IsParticipating = true;
            await SetPlayerParticipatingStatus(characterId, true);

            //if (!_participatingPlayers.Any(p => p.Id == player.Id))
            //{
            //    _participatingPlayers.Add(player);
            //}

            var team = GetTeam((int)GetTeamOrdinalFromPlayerId(characterId));

            team.AddStatsUpdate(updates);

            if (!team.ParticipatingPlayers.Any(p => p.Id == player.Id))
            {
                team.ParticipatingPlayers.Add(player);
            }

            var maxPointsChanged = MaxPlayerPointsTracker.TryUpdateMaxPoints(player.EventAggregate.Points, player.Id);

            var overlayMessageData = new OverlayMessageData
            {
                RedrawPointGraph = maxPointsChanged,
                MatchMaxPlayerPoints = MaxPlayerPointsTracker.GetMaxPoints()
            };

            SendPlayerStatUpdateMessage(player, overlayMessageData);

            SendTeamStatUpdateMessage(team, overlayMessageData);
        }

        public void UpdateTeamStats(int teamOrdinal, ScrimEventAggregate updates)
        {
            var team = GetTeam(teamOrdinal);

            team.AddStatsUpdate(updates);

            SendTeamStatUpdateMessage(team);
        }

        public async Task SaveRoundEndScores(int round)
        {
            foreach (var teamOrdinal in _ordinalTeamMap.Keys.ToList())
            {
                SaveTeamRoundEndScores(teamOrdinal, round);

                await SaveTeamMatchResultsToDb(teamOrdinal);
            }
        }

        public void SaveTeamRoundEndScores(int teamOrdinal, int round)
        {
            var team = GetTeam(teamOrdinal);

            team.EventAggregateTracker.SaveRoundToHistory(round);

            var players = team.ParticipatingPlayers.ToList();

            foreach (var player in players)
            {
                player.EventAggregateTracker.SaveRoundToHistory(round);
            }
        }

        public async Task SaveTeamMatchResultsToDb(int teamOrdinal)
        {
            var currentScrimMatchId = _matchDataService.CurrentMatchId;
            
            var resultsAggregate = new ScrimEventAggregate().Add(GetTeam(teamOrdinal).EventAggregate);

            var resultsEntity = new ScrimMatchTeamResult
            {
                ScrimMatchId = currentScrimMatchId,
                TeamOrdinal = teamOrdinal,
                Points = resultsAggregate.Points,
                NetScore = resultsAggregate.NetScore,
                Kills = resultsAggregate.Kills,
                Deaths = resultsAggregate.Deaths,
                Headshots = resultsAggregate.Headshots,
                HeadshotDeaths = resultsAggregate.HeadshotDeaths,
                Suicides = resultsAggregate.Suicides,
                Teamkills = resultsAggregate.Teamkills,
                TeamkillDeaths = resultsAggregate.TeamkillDeaths,
                RevivesGiven = resultsAggregate.RevivesGiven,
                RevivesTaken = resultsAggregate.RevivesTaken,
                DamageAssists = resultsAggregate.DamageAssists,
                UtilityAssists = resultsAggregate.UtilityAssists,
                DamageAssistedDeaths = resultsAggregate.DamageAssistedDeaths,
                UtilityAssistedDeaths = resultsAggregate.UtilityAssistedDeaths,
                ObjectiveCaptureTicks = resultsAggregate.ObjectiveCaptureTicks,
                ObjectiveDefenseTicks = resultsAggregate.ObjectiveDefenseTicks,
                BaseDefenses = resultsAggregate.BaseDefenses,
                BaseCaptures = resultsAggregate.BaseCaptures
            };

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                // Scrim Match Team Results
                var storeResultEntity = await dbContext.ScrimMatchTeamResults.FirstOrDefaultAsync(result => result.ScrimMatchId == currentScrimMatchId && result.TeamOrdinal == teamOrdinal);

                if (storeResultEntity == null)
                {
                    dbContext.ScrimMatchTeamResults.Add(resultsEntity);
                }
                else
                {
                    storeResultEntity = resultsEntity;
                    dbContext.ScrimMatchTeamResults.Update(storeResultEntity);
                }

                // Team Results Point Adjustments
                var updateAdjustments = resultsAggregate.PointAdjustments.ToList();

                var storeAdjustmentEntities = await dbContext.ScrimMatchTeamPointAdjustments
                                                        .Where(adj => adj.ScrimMatchId == currentScrimMatchId && adj.TeamOrdinal == teamOrdinal)
                                                        .ToListAsync();

                var allAdjustments = new List<PointAdjustment>();

                allAdjustments.AddRange(updateAdjustments);
                allAdjustments.AddRange(storeAdjustmentEntities
                                            .Select(ConvertFromDbModel)
                                            .Where(e => !allAdjustments.Any(a => a.Timestamp == e.Timestamp))
                                            .ToList());

                var createdAdjustments = new List<ScrimMatchTeamPointAdjustment>();

                foreach (var adjustment in allAdjustments)
                {
                    var storeEntity = storeAdjustmentEntities.Where(e => e.Timestamp == adjustment.Timestamp).FirstOrDefault();
                    var updateAdjustment = updateAdjustments.Where(a => a.Timestamp == adjustment.Timestamp).FirstOrDefault();

                    if (storeEntity == null)
                    {
                        var updateEntity = BuildScrimMatchTeamPointAdjustment(currentScrimMatchId, teamOrdinal, updateAdjustment);
                        createdAdjustments.Add(updateEntity);
                    }
                    else if (updateAdjustment == null)
                    {
                        dbContext.ScrimMatchTeamPointAdjustments.Remove(storeEntity);
                    }
                    else
                    {
                        var updateEntity = BuildScrimMatchTeamPointAdjustment(currentScrimMatchId, teamOrdinal, updateAdjustment);
                        storeEntity = updateEntity;
                        dbContext.ScrimMatchTeamPointAdjustments.Update(storeEntity);
                    }
                }

                if (createdAdjustments.Any())
                {
                    await dbContext.ScrimMatchTeamPointAdjustments.AddRangeAsync(createdAdjustments);
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private PointAdjustment ConvertFromDbModel(ScrimMatchTeamPointAdjustment adjustment)
        {
            return new PointAdjustment
            {
                Timestamp = adjustment.Timestamp,
                Points = adjustment.Points,
                Rationale = adjustment.Rationale
            };
        }

        private ScrimMatchTeamPointAdjustment BuildScrimMatchTeamPointAdjustment(string scrimMatchId, int teamOrdinal, PointAdjustment adjustment)
        {
            return new ScrimMatchTeamPointAdjustment
            {
                ScrimMatchId = scrimMatchId,
                TeamOrdinal = teamOrdinal,
                Timestamp = adjustment.Timestamp,
                Points = adjustment.Points,
                AdjustmentType = adjustment.AdjustmentType,
                Rationale = adjustment.Rationale
            };
        }

        public async Task AdjustTeamPoints(int teamOrdinal, PointAdjustment adjustment)
        {
            var statUpdate = new ScrimEventAggregate();

            statUpdate.AddPointAdjustment(adjustment);
            
            var team = GetTeam(teamOrdinal);

            team.AddStatsUpdate(statUpdate);

            if (_matchDataService.CurrentMatchRound > 0)
            {
                await SaveTeamMatchResultsToDb(teamOrdinal);
            }

            SendTeamStatUpdateMessage(team);
        }

        public async Task RemoveTeamPointAdjustment(int teamOrdinal, PointAdjustment adjustment)
        {
            var statUpdate = new ScrimEventAggregate();

            statUpdate.AddPointAdjustment(adjustment);

            var team = GetTeam(teamOrdinal);

            team.SubtractStatsUpdate(statUpdate);

            if (_matchDataService.CurrentMatchRound > 0)
            {
                await SaveTeamMatchResultsToDb(teamOrdinal);
            }

            SendTeamStatUpdateMessage(team);
        }


        #region Player Status Updates
        public void SetPlayerOnlineStatus(string characterId, bool isOnline)
        {
            var player = GetPlayerFromId(characterId);
            player.IsOnline = isOnline;

            SendPlayerStatUpdateMessage(player);
        }
        
        public async Task SetPlayerParticipatingStatus(string characterId, bool isParticipating)
        {
            var player = GetPlayerFromId(characterId);

            var wasAlreadyParticipating = player.IsParticipating;
            
            player.IsParticipating = isParticipating;
            player.IsActive = (!player.IsBenched && isParticipating);

            SendPlayerStatUpdateMessage(player);

            if (wasAlreadyParticipating == isParticipating)
            {
                return;
            }

            if (!isParticipating)
            {
                await _matchDataService.TryRemoveMatchParticipatingPlayer(characterId);
            }
            else if (isParticipating)
            {
                await _matchDataService.SaveMatchParticipatingPlayer(player);
            }
        }

        public void SetPlayerBenchedStatus(string characterId, bool isBenched)
        {
            var player = GetPlayerFromId(characterId);
            player.IsBenched = isBenched;
            player.IsActive = (!isBenched && player.IsParticipating);

            SendPlayerStatUpdateMessage(player);
        }

        public void SetPlayerLoadoutId(string characterId, int? loadoutId)
        {
            if (loadoutId == null || loadoutId <= 0)
            {
                return;
            }

            var player = GetPlayerFromId(characterId);
           player.LoadoutId = loadoutId;

            SendPlayerStatUpdateMessage(player);
        }
        #endregion

        #region Messaging
        private void SendTeamPlayerAddedMessage(Player player, bool isLastOfOutfit = false)
        {
            var payload = new TeamPlayerChangeMessage(player, TeamPlayerChangeType.Add, isLastOfOutfit);
            _messageService.BroadcastTeamPlayerChangeMessage(payload);
        }

        private void SendTeamPlayerRemovedMessage(Player player)
        {
            var payload = new TeamPlayerChangeMessage(player, TeamPlayerChangeType.Remove);
            _messageService.BroadcastTeamPlayerChangeMessage(payload);
        }

        private void SendTeamOutfitAddedMessage(Outfit outfit)
        {
            var payload = new TeamOutfitChangeMessage(outfit, TeamChangeType.Add);
            _messageService.BroadcastTeamOutfitChangeMessage(payload);
        }

        private void SendTeamOutfitRemovedMessage(Outfit outfit)
        {
            var payload = new TeamOutfitChangeMessage(outfit, TeamChangeType.Remove);
            _messageService.BroadcastTeamOutfitChangeMessage(payload);
        }

        private void SendTeamConstructedTeamAddedMessage(int teamOrdinal, ConstructedTeamMatchInfo teamMatchInfo)
        {
            var payload = new TeamConstructedTeamChangeMessage(teamOrdinal, teamMatchInfo.ConstructedTeam, teamMatchInfo.ActiveFactionId, TeamChangeType.Add);

            _messageService.BroadcastTeamConstructedTeamChangeMessage(payload);
        }

        private void SendTeamConstructedTeamRemovedMessage(int teamOrdinal, ConstructedTeamMatchInfo teamMatchInfo)
        {
            var payload = new TeamConstructedTeamChangeMessage(teamOrdinal, teamMatchInfo.ConstructedTeam, teamMatchInfo.ActiveFactionId, TeamChangeType.Remove);

            _messageService.BroadcastTeamConstructedTeamChangeMessage(payload);
        }

        private void SendPlayerStatUpdateMessage(Player player)
        {
            var payload = new PlayerStatUpdateMessage(player);
            _messageService.BroadcastPlayerStatUpdateMessage(payload);
        }

        private void SendPlayerStatUpdateMessage(Player player, OverlayMessageData overlayMessageData)
        {
            var payload = new PlayerStatUpdateMessage(player, overlayMessageData);
            _messageService.BroadcastPlayerStatUpdateMessage(payload);
        }

        private void SendTeamStatUpdateMessage(Team team)
        {
            var payload = new TeamStatUpdateMessage(team);
            _messageService.BroadcastTeamStatUpdateMessage(payload);
        }

        private void SendTeamStatUpdateMessage(Team team, OverlayMessageData overlayMessageData)
        {
            var payload = new TeamStatUpdateMessage(team, overlayMessageData);
            _messageService.BroadcastTeamStatUpdateMessage(payload);
        }

        #endregion

        public void Dispose()
        {
            return;
        }
    }
}
