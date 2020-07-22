using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ConstructedTeamService : IConstructedTeamService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly ICharacterService _characterService;
        private readonly IScrimPlayersService _playerService;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<ConstructedTeamService> _logger;

        public string CurrentMatchId { get; set; }
        public int CurrentMatchRound { get; set; } = 0;

        public static Regex ConstructedTeamNameRegex { get; } = new Regex("^([A-Za-z0-9()\\[\\]\\-_][ ]{0,1}){1,49}[A-Za-z0-9()\\[\\]\\-_]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex ConstructedTeamAliasRegex { get; } = new Regex("^[A-Za-z0-9]{1,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex CharacterNameRegex { get; } = new Regex("^[A-Za-z0-9]{1,32}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly KeyedSemaphoreSlim _constructedTeamLock = new KeyedSemaphoreSlim();

        public ConstructedTeamService(IDbContextHelper dbContextHelper, ICharacterService characterService, IScrimPlayersService playerService,
            IScrimMessageBroadcastService messageService, ILogger<ConstructedTeamService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _characterService = characterService;
            _playerService = playerService;
            _messageService = messageService;
            _logger = logger;
        }


        #region GET Methods
        public async Task<ConstructedTeam> GetConstructedTeam(int teamId, bool ignoreCollections = false)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var team = await dbContext.ConstructedTeams.FirstOrDefaultAsync(t => t.Id == teamId);

                if (ignoreCollections || team == null)
                {
                    return team;
                }

                team.PlayerMemberships = await dbContext.ConstructedTeamPlayerMemberships.Where(m => m.ConstructedTeamId == teamId).ToListAsync();

                return team;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return null;
            }
        }

        public async Task<int> GetConstructedTeamMemberCount(int teamId)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var team = await dbContext.ConstructedTeams.FirstOrDefaultAsync(t => t.Id == teamId);

                if (team == null)
                {
                    return -1;
                }

                return await dbContext.ConstructedTeamPlayerMemberships
                                .Where(m => m.ConstructedTeamId == teamId)
                                .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return -1;
            }
        }

        public async Task<IEnumerable<string>> GetConstructedTeamFactionMemberIds(int teamId, int factionId)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var team = await dbContext.ConstructedTeams.FirstOrDefaultAsync(t => t.Id == teamId);

                if (team == null)
                {
                    return null;
                }

                return await dbContext.ConstructedTeamPlayerMemberships
                                .Where(m => m.ConstructedTeamId == teamId && m.FactionId == factionId)
                                .Select(m => m.CharacterId)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return null;
            }
        }

        public async Task<IEnumerable<ConstructedTeamPlayerMembership>> GetConstructedTeamFactionMembers(int teamId, int factionId)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var team = await dbContext.ConstructedTeams.FirstOrDefaultAsync(t => t.Id == teamId);

                if (team == null)
                {
                    return null;
                }

                return await dbContext.ConstructedTeamPlayerMemberships
                                .Where(m => m.ConstructedTeamId == teamId && m.FactionId == factionId)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return null;
            }
        }

        public async Task<IEnumerable<Character>> GetConstructedTeamFactionCharacters(int teamId, int factionId)
        {
            var members = await GetConstructedTeamFactionMembers(teamId, factionId);

            if (members == null || !members.Any())
            {
                return null;
            }

            List<ConstructedTeamPlayerMembership> unprocessedMembers = new List<ConstructedTeamPlayerMembership>();
            unprocessedMembers.AddRange(members.ToList());

            List<Character> processedCharacters = new List<Character>();

            IEnumerable<Task<Character>> getCharacterTasksQuery =
                from member in members select _characterService.GetCharacterAsync(member.CharacterId);
            
            List<Task<Character>> getCharacterTasks = getCharacterTasksQuery.ToList();

            while (getCharacterTasks.Count > 0)
            {
                Task<Character> firstFinishedTask = await Task.WhenAny(getCharacterTasks);

                getCharacterTasks.Remove(firstFinishedTask);

                var character = firstFinishedTask.Result;

                if (character != null)
                {
                    processedCharacters.Add(character);
                    unprocessedMembers.RemoveAll(m => m.CharacterId == character.Id);
                }
            }

            foreach (var member in unprocessedMembers)
            {
                var character = new Character
                {
                    Name = "Unnamed Player",
                    Id = member.CharacterId,
                    FactionId = member.FactionId
                };

                processedCharacters.Add(character);
            }

            return processedCharacters;
        }
        
        public async Task<IEnumerable<ConstructedTeamMemberDetails>> GetConstructedTeamFactionMemberDetails(int teamId, int factionId)
        {
            var members = await GetConstructedTeamFactionMembers(teamId, factionId);

            if (members == null || !members.Any())
            {
                return null;
            }

            List<ConstructedTeamPlayerMembership> unprocessedMembers = new List<ConstructedTeamPlayerMembership>();
            unprocessedMembers.AddRange(members.ToList());

            List<ConstructedTeamMemberDetails> processedCharacters = new List<ConstructedTeamMemberDetails>();

            IEnumerable<Task<Character>> getCharacterTasksQuery =
                from member in members select _characterService.GetCharacterAsync(member.CharacterId);
            
            List<Task<Character>> getCharacterTasks = getCharacterTasksQuery.ToList();

            while (getCharacterTasks.Count > 0)
            {
                Task<Character> firstFinishedTask = await Task.WhenAny(getCharacterTasks);

                getCharacterTasks.Remove(firstFinishedTask);

                var character = firstFinishedTask.Result;

                if (character != null)
                {
                    var member = members.Where(m => m.CharacterId == character.Id).FirstOrDefault();
                    
                    processedCharacters.Add(ConvertToMemberDetailsModel(character, member));
                    unprocessedMembers.RemoveAll(m => m.CharacterId == character.Id);
                }
            }

            foreach (var member in unprocessedMembers)
            {
                var details = new ConstructedTeamMemberDetails
                {
                    NameFull = $"up{member.CharacterId}",
                    NameAlias = string.Empty,
                    CharacterId = member.CharacterId,
                    FactionId = member.FactionId
                };

                processedCharacters.Add(details);
            }

            return processedCharacters;
        }

        private ConstructedTeamMemberDetails ConvertToMemberDetailsModel(Character character, ConstructedTeamPlayerMembership membership)
        {
            return new ConstructedTeamMemberDetails
            {
                CharacterId = membership.CharacterId,
                ConstructedTeamId = membership.ConstructedTeamId,
                FactionId = membership.FactionId,
                NameFull = character.Name,
                NameAlias = membership.Alias,
                PrestigeLevel = character.PrestigeLevel,
                WorldId = character.WorldId
            };
        }

        public async Task<IEnumerable<Player>> GetConstructedTeamFactionPlayers(int teamId, int factionId)
        {
            var members = await GetConstructedTeamFactionMembers(teamId, factionId);

            if (members == null || !members.Any())
            {
                return null;
            }

            List<ConstructedTeamPlayerMembership> unprocessedMembers = new List<ConstructedTeamPlayerMembership>();
            unprocessedMembers.AddRange(members.ToList());

            List<Player> processedPlayers = new List<Player>();

            IEnumerable<Task<Player>> getPlayerTasksQuery =
                from member in members select _playerService.GetPlayerFromCharacterId(member.CharacterId);

            List<Task<Player>> getPlayersTasks = getPlayerTasksQuery.ToList();

            while (getPlayersTasks.Count > 0)
            {
                Task<Player> firstFinishedTask = await Task.WhenAny(getPlayersTasks);

                getPlayersTasks.Remove(firstFinishedTask);

                var player = firstFinishedTask.Result;

                if (player != null)
                {
                    var playerAlias = members.Where(m => m.CharacterId == player.Id).Select(m => m.Alias).FirstOrDefault();


                    player.TrySetNameAlias(playerAlias);

                    processedPlayers.Add(player);
                    unprocessedMembers.RemoveAll(m => m.CharacterId == player.Id);
                }
            }

            foreach (var member in unprocessedMembers)
            {
                var name = string.IsNullOrWhiteSpace(member.Alias) ? $"up{member.CharacterId}" : member.Alias;

                var character = new Character
                {
                    Id = member.CharacterId,
                    Name = name,
                    IsOnline = true,
                    PrestigeLevel = 0,
                    FactionId = factionId,
                    WorldId = 0
                };

                var player = new Player(character);

                player.TrySetNameAlias(name);

                processedPlayers.Add(player);
            }

            return processedPlayers;
        }

        public async Task<ConstructedTeamFormInfo> GetConstructedTeamFormInfo(int teamId, bool ignoreCollections = false)
        {
            var constructedTeam = await GetConstructedTeam(teamId, ignoreCollections);

            if (constructedTeam == null)
            {
                return null;
            }

            var teamInfo = ConvertToTeamFormInfo(constructedTeam);

            if (ignoreCollections || !constructedTeam.PlayerMemberships.Any())
            {
                return teamInfo;
            }

            var teamCharacters = new List<Character>();

            foreach (var member in constructedTeam.PlayerMemberships)
            {
                try
                {
                    var character = await _characterService.GetCharacterAsync(member.CharacterId);

                    if (character != null)
                    {
                        teamCharacters.Add(character);
                    }
                    else
                    {
                        _logger.LogError($"Census API returned no data for characterId {member.CharacterId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error fetching Census API data for characterId {member.CharacterId}: {ex}");
                }
            }

            teamInfo.Characters = teamCharacters;

            return teamInfo;
        }

        private ConstructedTeamFormInfo ConvertToTeamFormInfo(ConstructedTeam constructedTeam)
        {
            return new ConstructedTeamFormInfo
            {
                Id = constructedTeam.Id,
                Name = constructedTeam.Name,
                Alias = constructedTeam.Alias,
                IsHiddenFromSelection = constructedTeam.IsHiddenFromSelection
            };
        }

        private ConstructedTeam ConvertToDbModel(ConstructedTeamFormInfo formInfo)
        {
            return new ConstructedTeam
            {
                Name = formInfo.Name,
                Alias = formInfo.Alias,
                IsHiddenFromSelection = formInfo.IsHiddenFromSelection
            };
        }

        public async Task<IEnumerable<ConstructedTeam>> GetConstructedTeams(bool ignoreCollections = false, bool includeHiddenTeams = false)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var teams = includeHiddenTeams
                                ? await dbContext.ConstructedTeams.ToListAsync()
                                : await dbContext.ConstructedTeams.Where(t => !t.IsHiddenFromSelection).ToListAsync();


                if (ignoreCollections || !teams.Any())
                {
                    return teams;
                }

                foreach (var team in teams)
                {
                    team.PlayerMemberships = await dbContext.ConstructedTeamPlayerMemberships.Where(m => m.ConstructedTeamId == team.Id).ToListAsync();
                }

                return teams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return null;
            }
        }
        #endregion GET Methods

        #region CREATE / EDIT Methods
        public async Task SaveConstructedTeam(ConstructedTeamFormInfo constructedTeamFormInfo)
        {
            await CreateConstructedTeam(ConvertToDbModel(constructedTeamFormInfo));
        }

        public async Task<bool> UpdateConstructedTeamInfo(ConstructedTeam teamUpdate)
        {
            var updateId = teamUpdate.Id;
            var updateName = teamUpdate.Name;
            var updateAlias = teamUpdate.Alias;
            var updateIsHidden = teamUpdate.IsHiddenFromSelection;

            if (!IsValidConstructedTeamName(updateName))
            {
                _logger.LogError($"Error update Constructed Team {updateId} info: invalid team name");
                return false;
            }

            if (!IsValidConstructedTeamAlias(updateAlias))
            {
                _logger.LogError($"Error update Constructed Team {updateId} info: invalid team alias");
                return false;
            }

            using (await _constructedTeamLock.WaitAsync($"{updateId}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeEntity = await GetConstructedTeam(updateId, true);

                    if (storeEntity == null)
                    {
                        return false;
                    }

                    var oldName = storeEntity.Name;
                    var oldAlias = storeEntity.Alias;
                    var oldIsHidden = storeEntity.IsHiddenFromSelection;

                    storeEntity.Name = updateName;
                    storeEntity.Alias = updateAlias;
                    storeEntity.IsHiddenFromSelection = updateIsHidden;

                    dbContext.ConstructedTeams.Update(storeEntity);

                    await dbContext.SaveChangesAsync();

                    var message = new ConstructedTeamInfoChangeMessage(storeEntity, oldName, oldAlias, oldIsHidden);
                    _messageService.BroadcastConstructedTeamInfoChangeMessage(message);

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error update Constructed Team {updateId} info: {ex}");
                    return false;
                }
            }
        }

        public async Task<ConstructedTeam> CreateConstructedTeam(ConstructedTeam constructedTeam)
        {
            if (!IsValidConstructedTeamName(constructedTeam.Name))
            {
                return null;
            }

            if (!IsValidConstructedTeamAlias(constructedTeam.Alias))
            {
                return null;
            }

            using (await _constructedTeamLock.WaitAsync($"{constructedTeam.Id}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    dbContext.ConstructedTeams.Add(constructedTeam);

                    await dbContext.SaveChangesAsync();

                    return constructedTeam;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());

                    return null;
                }
            }
        }

        public async Task<Character> TryAddCharacterToConstructedTeam(int teamId, string characterInput, string customAlias)
        {
            Regex idRegex = new Regex("^[0-9]{19}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            bool isId = idRegex.Match(characterInput).Success;

            Character characterOut;

            try
            {
                if (isId)
                {
                    characterOut = await TryAddCharacterIdToConstructedTeam(teamId, characterInput, customAlias);

                    if (characterOut != null)
                    {
                        return characterOut;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to add character ID to constructed team: {ex}");
            }

            try
            {
                Regex nameRegex = new Regex("^[A-Za-z0-9]{1,32}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                bool isName = nameRegex.Match(characterInput).Success;

                if (isName)
                {
                    return await TryAddCharacterNameToConstructedTeam(teamId, characterInput, customAlias);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to add character name to constructed team: {ex}");
            }

            return null;
        }

        private async Task<Character> TryAddCharacterIdToConstructedTeam(int teamId, string characterId, string customAlias)
        {
            using (await _constructedTeamLock.WaitAsync($"{teamId}^{characterId}"))
            {
                if (await IsCharacterIdOnTeam(teamId, characterId))
                {
                    return null;
                }

                var character = await _characterService.GetCharacterAsync(characterId);

                if (character == null)
                {
                    return null;
                }

                string playerAlias;
                if (string.IsNullOrWhiteSpace(customAlias))
                {
                    playerAlias = Player.GetTrimmedPlayerName(character.Name, character.WorldId);

                    if (string.IsNullOrWhiteSpace(playerAlias))
                    {
                        playerAlias = character.Name;
                    }
                }
                else
                {
                    playerAlias = customAlias;
                }

                if (await TryAddCharacterToConstructedTeamDb(teamId, characterId, character.FactionId, playerAlias))
                {
                    var member = new ConstructedTeamPlayerMembership
                    {
                        ConstructedTeamId = teamId,
                        CharacterId = characterId,
                        FactionId = character.FactionId,
                        Alias = playerAlias
                    };

                    var memberDetails = ConvertToMemberDetailsModel(character, member);

                    var changeMessage = new ConstructedTeamMemberChangeMessage(teamId, character, memberDetails, ConstructedTeamMemberChangeType.Add);
                    _messageService.BroadcastConstructedTeamMemberChangeMessage(changeMessage);

                    return character;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<Character> TryAddCharacterNameToConstructedTeam(int teamId, string characterName, string customAlias)
        {
            var character = await _characterService.GetCharacterByNameAsync(characterName);

            if (character == null)
            {
                return null;
            }

            using (await _constructedTeamLock.WaitAsync($"{teamId}^{character.Id}"))
            {
                if (await IsCharacterIdOnTeam(teamId, character.Id))
                {
                    return null;
                }

                string playerAlias;
                if (string.IsNullOrWhiteSpace(customAlias))
                {
                    playerAlias = Player.GetTrimmedPlayerName(character.Name, character.WorldId);

                    if (string.IsNullOrWhiteSpace(playerAlias))
                    {
                        playerAlias = characterName;
                    }
                }
                else
                {
                    playerAlias = customAlias;
                }

                if (await TryAddCharacterToConstructedTeamDb(teamId, character.Id, character.FactionId, playerAlias))
                {
                    var member = new ConstructedTeamPlayerMembership
                    {
                        ConstructedTeamId = teamId,
                        CharacterId = character.Id,
                        FactionId = character.FactionId,
                        Alias = playerAlias
                    };

                    var memberDetails = ConvertToMemberDetailsModel(character, member);

                    var changeMessage = new ConstructedTeamMemberChangeMessage(teamId, character, memberDetails, ConstructedTeamMemberChangeType.Add);
                    _messageService.BroadcastConstructedTeamMemberChangeMessage(changeMessage);

                    return character;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<bool> TryAddCharacterToConstructedTeamDb(int teamId, string characterId, int factionId, string alias)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            // Don't allow NSO characters onto teams
            if (factionId > 3 || factionId <= 0)
            {
                return false;
            }    

            var newEntity = new ConstructedTeamPlayerMembership
            {
                ConstructedTeamId = teamId,
                CharacterId = characterId,
                FactionId = factionId,
                Alias = alias
            };

            try
            {
                var storeEntity = await dbContext.ConstructedTeamPlayerMemberships
                                            .Where(m => m.CharacterId == characterId && m.ConstructedTeamId == teamId)
                                            .FirstOrDefaultAsync();
                
                if (storeEntity != null)
                {
                    return false;
                }
                
                dbContext.ConstructedTeamPlayerMemberships.Add(newEntity);

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding character ID {characterId} to team ID {teamId} in database: {ex}");

                return false;
            }
        }

        public async Task<bool> TryRemoveCharacterFromConstructedTeam(int teamId, string characterId)
        {
            using (await _constructedTeamLock.WaitAsync($"{teamId}^{characterId}"))
            {
                if (await TryRemoveCharacterFromConstructedTeamDb(teamId, characterId))
                {
                    var changeMessage = new ConstructedTeamMemberChangeMessage(teamId, characterId, ConstructedTeamMemberChangeType.Remove);
                    _messageService.BroadcastConstructedTeamMemberChangeMessage(changeMessage);

                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        private async Task<bool> TryRemoveCharacterFromConstructedTeamDb(int teamId, string characterId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                var storeEntity = await dbContext.ConstructedTeamPlayerMemberships
                                            .Where(m => m.CharacterId == characterId && m.ConstructedTeamId == teamId)
                                            .FirstOrDefaultAsync();

                if (storeEntity == null)
                {
                    return false;
                }

                dbContext.ConstructedTeamPlayerMemberships.Remove(storeEntity);

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing character ID {characterId} from team ID {teamId} in database: {ex}");

                return false;
            }
        }

        public async Task<bool> TryUpdateMemberAlias(int teamId, string characterId, string oldAlias, string newAlias)
        {
            if (string.IsNullOrWhiteSpace(newAlias) || !CharacterNameRegex.Match(newAlias).Success || oldAlias == newAlias)
            {
                return false;
            }

            using (await _constructedTeamLock.WaitAsync($"{teamId}^{characterId}"))
            {
                if (await TryUpdateMemberAliasInDb(teamId, characterId, newAlias))
                {
                    var changeMessage = new ConstructedTeamMemberChangeMessage(teamId, characterId, ConstructedTeamMemberChangeType.UpdateAlias, oldAlias, newAlias);
                    _messageService.BroadcastConstructedTeamMemberChangeMessage(changeMessage);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> TryClearMemberAlias(int teamId, string characterId, string oldAlias)
        {
            if (string.IsNullOrWhiteSpace(oldAlias))
            {
                return false;
            }

            using (await _constructedTeamLock.WaitAsync($"{teamId}^{characterId}"))
            {
                if (await TryUpdateMemberAliasInDb(teamId, characterId, null))
                {
                    var changeMessage = new ConstructedTeamMemberChangeMessage(teamId, characterId, ConstructedTeamMemberChangeType.UpdateAlias, oldAlias, null);
                    _messageService.BroadcastConstructedTeamMemberChangeMessage(changeMessage);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private async Task<bool> TryUpdateMemberAliasInDb(int teamId, string characterId, string newAlias)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            try
            {
                var storeEntity = await dbContext.ConstructedTeamPlayerMemberships
                                            .Where(m => m.CharacterId == characterId && m.ConstructedTeamId == teamId)
                                            .FirstOrDefaultAsync();

                if (storeEntity == null)
                {
                    return false;
                }

                var oldAlias = storeEntity.Alias;

                storeEntity.Alias = newAlias;

                dbContext.ConstructedTeamPlayerMemberships.Update(storeEntity);

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                var newAliasDisplay = string.IsNullOrWhiteSpace(newAlias) ? "null" : newAlias;
                
                _logger.LogError($"Error updating alias to {newAliasDisplay} for character ID {characterId} on team ID {teamId} in database: {ex}");

                return false;
            }
        }
        #endregion CREATE / EDIT Methods

        public async Task<bool> IsCharacterIdOnTeam(int teamId, string characterId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.ConstructedTeamPlayerMemberships.AnyAsync(m => m.CharacterId == characterId && m.ConstructedTeamId == teamId);
        }

        public static bool IsValidConstructedTeamName(string name)
        {
            return ConstructedTeamNameRegex.Match(name).Success;
        }

        public static bool IsValidConstructedTeamAlias(string alias)
        {
            return ConstructedTeamAliasRegex.Match(alias).Success;
        }
    }
}
