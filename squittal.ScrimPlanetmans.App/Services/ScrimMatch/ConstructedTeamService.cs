using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ConstructedTeamService : IConstructedTeamService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IScrimTeamsManager _teamsManager;
        private readonly ICharacterService _characterService;
        private readonly ILogger<ConstructedTeamService> _logger;

        public string CurrentMatchId { get; set; }
        public int CurrentMatchRound { get; set; } = 0;

        public ConstructedTeamService(IDbContextHelper dbContextHelper, IScrimTeamsManager teamsManager, ICharacterService characterService, ILogger<ConstructedTeamService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _teamsManager = teamsManager;
            _characterService = characterService;
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

        public async Task<ConstructedTeamFormInfo> GetConstructedTeamFormInfo(int teamId)
        {
            var constructedTeam = await GetConstructedTeam(teamId, false);

            if (constructedTeam == null)
            {
                return null;
            }

            var teamInfo = ConvertToTeamFormInfo(constructedTeam);

            if (!constructedTeam.PlayerMemberships.Any())
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

        public async Task<ConstructedTeam> CreateConstructedTeam(ConstructedTeam constructedTeam)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                dbContext.ConstructedTeams.Add(constructedTeam);

                //var storeTeamEntity = await dbContext.ConstructedTeams.FirstOrDefaultAsync(ct => ct.Id == constructedTeam.Id);

                //if (storeTeamEntity == null)
                //{
                //    dbContext.ConstructedTeams.Add(constructedTeam);
                //}
                //else
                //{
                //    storeTeamEntity = constructedTeam;
                //    dbContext.ConstructedTeams.Update(constructedTeam);
                //}


                //// Team Results Point Adjustments
                //var updateAdjustments = resultsAggregate.PointAdjustments.ToList();

                //var storeAdjustmentEntities = await dbContext.ScrimMatchTeamPointAdjustments
                //                                        .Where(adj => adj.ScrimMatchId == currentScrimMatchId && adj.TeamOrdinal == teamOrdinal)
                //                                        .ToListAsync();

                //var allAdjustments = new List<PointAdjustment>();

                //allAdjustments.AddRange(updateAdjustments);
                //allAdjustments.AddRange(storeAdjustmentEntities
                //                            .Select(ConvertFromDbModel)
                //                            .Where(e => !allAdjustments.Any(a => a.Timestamp == e.Timestamp))
                //                            .ToList());

                //var createdAdjustments = new List<ScrimMatchTeamPointAdjustment>();

                //foreach (var adjustment in allAdjustments)
                //{
                //    var storeEntity = storeAdjustmentEntities.Where(e => e.Timestamp == adjustment.Timestamp).FirstOrDefault();
                //    var updateAdjustment = updateAdjustments.Where(a => a.Timestamp == adjustment.Timestamp).FirstOrDefault();

                //    if (storeEntity == null)
                //    {
                //        var updateEntity = BuildScrimMatchTeamPointAdjustment(currentScrimMatchId, teamOrdinal, updateAdjustment);
                //        createdAdjustments.Add(updateEntity);
                //    }
                //    else if (updateAdjustment == null)
                //    {
                //        dbContext.ScrimMatchTeamPointAdjustments.Remove(storeEntity);
                //    }
                //    else
                //    {
                //        var updateEntity = BuildScrimMatchTeamPointAdjustment(currentScrimMatchId, teamOrdinal, updateAdjustment);
                //        storeEntity = updateEntity;
                //        dbContext.ScrimMatchTeamPointAdjustments.Update(storeEntity);
                //    }
                //}

                //if (createdAdjustments.Any())
                //{
                //    await dbContext.ScrimMatchTeamPointAdjustments.AddRangeAsync(createdAdjustments);
                //}

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
}
