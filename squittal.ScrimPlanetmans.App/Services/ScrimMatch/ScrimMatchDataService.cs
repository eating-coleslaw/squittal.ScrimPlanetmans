using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ScrimMatchDataService : IScrimMatchDataService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly ILogger<ScrimMatchDataService> _logger;

        public string CurrentMatchId { get ; set; }
        public int CurrentMatchRound { get; set; } = 0;
        public int CurrentMatchRulesetId { get; set; }

        private readonly KeyedSemaphoreSlim _scrimMatchLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _scrimMatchRoundConfigurationLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _scrimMatchParticipatingPlayerLock = new KeyedSemaphoreSlim();

        public ScrimMatchDataService(IDbContextHelper dbContextHelper, ILogger<ScrimMatchDataService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _logger = logger;
        }

        IEnumerable<Data.Models.ScrimMatch> IScrimMatchDataService.GetAllMatches()
        {
            throw new NotImplementedException();
        }

        public async Task<Data.Models.ScrimMatch> GetCurrentMatch()
        {
            var matchId = CurrentMatchId;

            using (await _scrimMatchLock.WaitAsync(matchId))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    return await dbContext.ScrimMatches.FirstOrDefaultAsync(sm => sm.Id == matchId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                
                    return null;
                }
            }
        }

        public async Task SaveToCurrentMatch(Data.Models.ScrimMatch scrimMatch)
        {
            var id = scrimMatch.Id;

            using (await _scrimMatchLock.WaitAsync(id))
            {
                var oldMatchId = CurrentMatchId;

                CurrentMatchId = id;

                try
                {
                    CurrentMatchId = id;

                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeEntity = await dbContext.ScrimMatches.FirstOrDefaultAsync(sm => sm.Id == id);

                    if (storeEntity == null)
                    {
                        dbContext.ScrimMatches.Add(scrimMatch);
                    }
                    else
                    {
                        storeEntity = scrimMatch;
                        dbContext.ScrimMatches.Update(storeEntity);
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    CurrentMatchId = oldMatchId;

                    _logger.LogError(ex.ToString());
                }
            }
        }

        public async Task SaveCurrentMatchRoundConfiguration(MatchConfiguration matchConfiguration)
        {
            var matchId = CurrentMatchId;
            var round = CurrentMatchRound;

            using (await _scrimMatchRoundConfigurationLock.WaitAsync($"{matchId}_{round}"))
            {
                if (round <= 0)
                {
                    return;
                }

                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var matchEntity = await dbContext.ScrimMatches.FirstOrDefaultAsync(sm => sm.Id == matchId);

                    if (matchEntity == null)
                    {
                        return;
                    }

                    var storeEntity = await dbContext.ScrimMatchRoundConfigurations
                                                        .Where(rc => rc.ScrimMatchId == matchId && rc.ScrimMatchRound == round)
                                                        .FirstOrDefaultAsync();

                    if (storeEntity == null)
                    {
                        dbContext.ScrimMatchRoundConfigurations.Add(ConvertToDbModel(matchConfiguration));
                    }
                    else
                    {
                        storeEntity = ConvertToDbModel(matchConfiguration);
                        dbContext.ScrimMatchRoundConfigurations.Update(storeEntity);
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        public async Task RemoveMatchRoundConfiguration(int roundToDelete)
        {
            var matchId = CurrentMatchId;

            using (await _scrimMatchRoundConfigurationLock.WaitAsync($"{matchId}_{roundToDelete}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeEntity = await dbContext.ScrimMatchRoundConfigurations
                                                        .Where(rc => rc.ScrimMatchId == matchId && rc.ScrimMatchRound == roundToDelete)
                                                        .FirstOrDefaultAsync();

                    if (storeEntity == null)
                    {
                        return;
                    }
                    else
                    {
                        dbContext.ScrimMatchRoundConfigurations.Remove(storeEntity);
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        private ScrimMatchRoundConfiguration ConvertToDbModel(MatchConfiguration matchConfiguration)
        {
            return new ScrimMatchRoundConfiguration
            {
                ScrimMatchId = CurrentMatchId,
                ScrimMatchRound = CurrentMatchRound,
                Title = matchConfiguration.Title,
                RoundSecondsTotal = matchConfiguration.RoundSecondsTotal,
                WorldId = matchConfiguration.WorldId,
                IsManualWorldId = matchConfiguration.IsManualWorldId,
                FacilityId = matchConfiguration.FacilityId > 0 ? matchConfiguration.FacilityId : (int?)null,
                IsRoundEndedOnFacilityCapture = matchConfiguration.EndRoundOnFacilityCapture,

                TargetPointValue = matchConfiguration.TargetPointValue,
                InitialPoints = matchConfiguration.InitialPoints,

                PeriodicFacilityControlPoints = matchConfiguration.PeriodicFacilityControlPoints,
                PeriodicFacilityControlInterval = matchConfiguration.PeriodicFacilityControlInterval,

                EnableRoundTimeLimit = matchConfiguration.EnableRoundTimeLimit,
                RoundTimerDirection = matchConfiguration.RoundTimerDirection,
                EndRoundOnPointValueReached = matchConfiguration.EndRoundOnPointValueReached,
                MatchWinCondition = matchConfiguration.MatchWinCondition,
                RoundWinCondition = matchConfiguration.RoundWinCondition,
                EnablePeriodicFacilityControlRewards = matchConfiguration.EnablePeriodicFacilityControlRewards
            };
        }

        public async Task SaveMatchParticipatingPlayer(Player player)
        {
            var matchId = CurrentMatchId;

            using (await _scrimMatchParticipatingPlayerLock.WaitAsync($"{player.Id}_{matchId}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeEntity = await dbContext.ScrimMatchParticipatingPlayers
                                                .Where(p => p.CharacterId == player.Id && p.ScrimMatchId == matchId)
                                                .FirstOrDefaultAsync();

                    if (storeEntity == null)
                    {
                        dbContext.ScrimMatchParticipatingPlayers.Add(ConvertToDbModel(player, matchId));
                    }
                    else
                    {
                        storeEntity = ConvertToDbModel(player, matchId);
                        dbContext.ScrimMatchParticipatingPlayers.Update(storeEntity);
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }

        public async Task<bool> TryRemoveMatchParticipatingPlayer(string characterId)
        {
            var matchId = CurrentMatchId;

            using (await _scrimMatchParticipatingPlayerLock.WaitAsync($"{characterId}_{matchId}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeEntity = await dbContext.ScrimMatchParticipatingPlayers
                                                .Where(p => p.CharacterId == characterId && p.ScrimMatchId == matchId)
                                                .FirstOrDefaultAsync();

                    if (storeEntity == null)
                    {
                        return false;
                    }
                    else
                    {
                        dbContext.ScrimMatchParticipatingPlayers.Remove(storeEntity);
                    }

                    await dbContext.SaveChangesAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());

                    return false;
                }
            }
        }

        private ScrimMatchParticipatingPlayer ConvertToDbModel(Player player, string matchId)
        {
            return new ScrimMatchParticipatingPlayer
            {
                ScrimMatchId = matchId,
                CharacterId = player.Id,
                TeamOrdinal = player.TeamOrdinal,
                NameFull = player.NameFull,
                NameDisplay = player.NameDisplay,
                FactionId = player.FactionId,
                WorldId = player.WorldId,
                PrestigeLevel = player.PrestigeLevel,
                IsFromOutfit = !player.IsOutfitless,
                OutfitId = player.IsOutfitless ? null : player.OutfitId,
                OutfitAlias = player.IsOutfitless ? null : player.OutfitAlias,
                IsFromConstructedTeam = player.IsFromConstructedTeam,
                ConstructedTeamId = player.IsFromConstructedTeam ? player.ConstructedTeamId : null
            };
        }

        public async Task SaveScrimPeriodicControlTick(ScrimPeriodicControlTick dataModel)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            dbContext.ScrimPeriodicControlTicks.Add(dataModel);
            await dbContext.SaveChangesAsync();
        }
    }
}
