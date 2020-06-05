using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Data.Models;
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
        private readonly IScrimTeamsManager _teamsManager;
        private readonly ICharacterService _characterService;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<ConstructedTeamService> _logger;

        public string CurrentMatchId { get; set; }
        public int CurrentMatchRound { get; set; } = 0;

        public static Regex ConstructedTeamNameRegex { get; } = new Regex("^([A-Za-z0-9()\\[\\]\\-_][ ]{0,1}){1,49}[A-Za-z0-9()\\[\\]\\-_]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex ConstructedTeamAliasRegex { get; } = new Regex("^[A-Za-z0-9]{1,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ConstructedTeamService(IDbContextHelper dbContextHelper, IScrimTeamsManager teamsManager, ICharacterService characterService,
            IScrimMessageBroadcastService messageService, ILogger<ConstructedTeamService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _teamsManager = teamsManager;
            _characterService = characterService;
            _messageService = messageService;
            _logger = logger;
        }

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

                team.FactionPreferences = await dbContext.ConstructedTeamFactionPreferences.Where(pref => pref.ConstructedTeamId == teamId).ToListAsync();
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

        public async Task<IEnumerable<Character>> GetConstructionTeamFactionCharacters(int teamId, int factionId)
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

        public async Task<ConstructedTeamMatchInfo> GetConstructedTeamMatchInfo(int teamId)
        {
                var constructedTeam = await GetConstructedTeam(teamId, false);

                if (constructedTeam == null)
                {
                    return null;
                }

                var teamInfo = ConvertToTeamMatchInfo(constructedTeam);

                var matchTeam = _teamsManager.GetTeamFromConstructedTeamId(teamId);

                if (matchTeam == null)
                {
                    return teamInfo;
                }

                teamInfo.TeamOrdinal = matchTeam.TeamOrdinal;

                if (!constructedTeam.PlayerMemberships.Any())
                {
                    return teamInfo;
                }

                var teamPlayers = new List<Player>();

                foreach (var member in constructedTeam.PlayerMemberships)
                {
                    var player = _teamsManager.GetPlayerFromId(member.CharacterId);
                    if (player != null)
                    {
                        teamPlayers.Add(player);
                        teamInfo.OnlineMembersCount += (player.IsOnline ? 1 : 0);

                        if (teamInfo.ActiveFactionId == null)
                        {
                            teamInfo.ActiveFactionId = player.FactionId;
                        }
                    }
                }

                teamInfo.Players = teamPlayers;

                return teamInfo;
        }

        private ConstructedTeamMatchInfo ConvertToTeamMatchInfo(ConstructedTeam constructedTeam)
        {
                return new ConstructedTeamMatchInfo
                {
                    Id = constructedTeam.Id,
                    Name = constructedTeam.Name,
                    Alias = constructedTeam.Alias,
                    FactionPreferences = constructedTeam.FactionPreferences,
                    TotalMembersCount = constructedTeam.PlayerMemberships.Count()
                };
        }

        public async Task<ConstructedTeamFormInfo> GetConstructedTeamFormInfo(int teamId, bool ignoreCollections = false)
        {
            //var constructedTeam = await GetConstructedTeam(teamId, false);
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
                FactionPreferences = constructedTeam.FactionPreferences
            };
        }

        private ConstructedTeam ConvertToDbModel(ConstructedTeamFormInfo formInfo)
        {
            //if (formInfo.Id == null || formInfo.Id < 0)
            //{
            //    return null;
            //}
            
            return new ConstructedTeam
            {
                //Id = (int)formInfo.Id,
                Name = formInfo.Name,
                Alias = formInfo.Alias
                //FactionPreferences = formInfo.FactionPreferences
            };
        }

        public async Task AddConstructedTeamToMatch(int constructedTeamId, int matchTeamOrdinal, int factionId)
        {
            throw new NotImplementedException();
        }

        

        public async Task<IEnumerable<ConstructedTeam>> GetConstructedTeams(bool ignoreCollections = false)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var teams = await dbContext.ConstructedTeams.ToListAsync();


                if (ignoreCollections || !teams.Any())
                {
                    return teams;
                }

                foreach (var team in teams)
                {
                    team.FactionPreferences = await dbContext.ConstructedTeamFactionPreferences.Where(pref => pref.ConstructedTeamId == team.Id).ToListAsync();
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

        public async Task SaveConstructedTeam(ConstructedTeamFormInfo constructedTeamFormInfo)
        {
            await CreateConstructedTeam(ConvertToDbModel(constructedTeamFormInfo));
        }

        public async Task<bool> UpdateConstructedTeamInfo(ConstructedTeam teamUpdate)
        {
            var updateId = teamUpdate.Id;
            var updateName = teamUpdate.Name;
            var updateAlias = teamUpdate.Alias;

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

                storeEntity.Name = updateName;
                storeEntity.Alias = updateAlias;

                dbContext.ConstructedTeams.Update(storeEntity);

                await dbContext.SaveChangesAsync();

                var message = new ConstructedTeamInfoChangeMessage(storeEntity, oldName, oldAlias);
                _messageService.BroadcastConstructedTeamInfoChangeMessage(message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error update Constructed Team {updateId} info: {ex}");
                return false;
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

        public async Task<bool> IsCharacterIdOnTeam(int teamId, string characterId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.ConstructedTeamPlayerMemberships.AnyAsync(m => m.CharacterId == characterId && m.ConstructedTeamId == teamId);
        }

        public async Task<Character> TryAddCharacterToConstructedTeam(int teamId, string characterInput)
        {
            Regex idRegex = new Regex("^[0-9]{19}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            bool isId = idRegex.Match(characterInput).Success;

            Character characterOut;

            try
            {
                if (isId)
                {
                    characterOut = await TryAddCharacterIdToConstructedTeam(teamId, characterInput);

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
                    return await TryAddCharacterNameToConstructedTeam(teamId, characterInput);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to add character name to constructed team: {ex}");
            }

            return null;
        }

        private async Task<Character> TryAddCharacterIdToConstructedTeam(int teamId, string characterId)
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


            if (await TryAddCharacterToConstructedTeamDb(teamId, characterId, character.FactionId))
            {
                var changeMessage = new ConstructedTeamMemberChangeMessage(teamId, character, ConstructedTeamMemberChangeType.Add);
                _messageService.BroadcastConstructedTeamMemberChangeMessage(changeMessage);

                return character;
            }
            else
            {
                return null;
            }
        }

        private async Task<Character> TryAddCharacterNameToConstructedTeam(int teamId, string characterName)
        {
            var character = await _characterService.GetCharacterByNameAsync(characterName);

            if (character == null)
            {
                return null;
            }

            if (await IsCharacterIdOnTeam(teamId, character.Id))
            {
                return null;
            }

            if (await TryAddCharacterToConstructedTeamDb(teamId, character.Id, character.FactionId))
            {
                var changeMessage = new ConstructedTeamMemberChangeMessage(teamId, character, ConstructedTeamMemberChangeType.Add);
                _messageService.BroadcastConstructedTeamMemberChangeMessage(changeMessage);

                return character;
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> TryAddCharacterToConstructedTeamDb(int teamId, string characterId, int factionId)
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
                FactionId = factionId
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
