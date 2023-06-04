using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatchReports
{
    public class ScrimMatchReportDataService : IScrimMatchReportDataService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly ILogger<ScrimMatchReportDataService> _logger;

        private readonly int _scrimMatchBrowserPageSize = 15;

        public ScrimMatchReportDataService(IDbContextHelper dbContextHelper, ILogger<ScrimMatchReportDataService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _logger = logger;
        }

        public async Task<PaginatedList<ScrimMatchInfo>> GetHistoricalScrimMatchesListAsync(int? pageIndex, ScrimMatchReportBrowserSearchFilter searchFilter, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var scrimMatchesQuery = dbContext.ScrimMatchInfo
                                                    .Where(GetHistoricalScrimMatchBrowserWhereExpression(searchFilter))
                                                    .AsQueryable();

                var paginatedList = await PaginatedList<ScrimMatchInfo>.CreateAsync(scrimMatchesQuery.AsNoTracking().OrderByDescending(m => m.StartTime), pageIndex ?? 1, _scrimMatchBrowserPageSize, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                if (paginatedList == null)
                {
                    return null;
                }

                foreach (var match in paginatedList.Contents)
                {
                    match.SetTeamAliases();
                }

                return paginatedList;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchesListAsync page {pageIndex}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchesListAsync page {pageIndex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                
                return null;
            }
        }

        private Expression<Func<ScrimMatchInfo, bool>> GetHistoricalScrimMatchBrowserWhereExpression(ScrimMatchReportBrowserSearchFilter searchFilter)
        {
            var isDefaultFilter = searchFilter.IsDefaultFilter;

            Expression<Func<ScrimMatchInfo, bool>> whereExpression = null;

            if (isDefaultFilter)
            {
                Expression<Func<ScrimMatchInfo, bool>> roundExpression = m => m.RoundCount >= searchFilter.MinimumRoundCount;

                var twoHoursAgo = DateTime.UtcNow - TimeSpan.FromHours(2);
                Expression<Func<ScrimMatchInfo, bool>> recentMatchExpression = m => m.StartTime >= twoHoursAgo;

                roundExpression = roundExpression.Or(recentMatchExpression);

                whereExpression = whereExpression == null ? roundExpression : whereExpression.And(roundExpression);
            }
            else
            {
                Expression<Func<ScrimMatchInfo, bool>> roundExpression = m => m.RoundCount >= searchFilter.MinimumRoundCount;

                whereExpression = whereExpression == null ? roundExpression : whereExpression.And(roundExpression);
            }

            if (searchFilter.SearchStartDate != null)
            {
                Expression<Func<ScrimMatchInfo, bool>> startDateExpression = m => m.StartTime >= searchFilter.SearchStartDate;

                whereExpression = whereExpression == null ? startDateExpression : whereExpression.And(startDateExpression);
            }

            if (searchFilter.SearchEndDate != null)
            {
                Expression<Func<ScrimMatchInfo, bool>> endDateExpression = m => m.StartTime <= searchFilter.SearchEndDate;

                whereExpression = whereExpression == null ? endDateExpression : whereExpression.And(endDateExpression);
            }

            if (searchFilter.RulesetId != 0)
            {
                Expression<Func<ScrimMatchInfo, bool>> rulesetExpression = m => m.RulesetId == searchFilter.RulesetId;

                whereExpression = whereExpression == null ? rulesetExpression : whereExpression.And(rulesetExpression);
            }

            if (searchFilter.FacilityId != -1)
            {
                Expression<Func<ScrimMatchInfo, bool>> facilityExpression;

                if (searchFilter.FacilityId == 0)
                {
                    facilityExpression = m => m.FacilityId != null;
                }
                else
                {
                    facilityExpression = m => m.FacilityId == searchFilter.FacilityId;
                }

                whereExpression = whereExpression == null ? facilityExpression : whereExpression.And(facilityExpression);
            }

            if (searchFilter.WorldId != 0)
            {
                Expression<Func<ScrimMatchInfo, bool>> worldExpression = m => m.WorldId == searchFilter.WorldId;

                whereExpression = whereExpression == null ? worldExpression : whereExpression.And(worldExpression);
            }

            if (searchFilter.SearchTermsList.Any() || searchFilter.AliasSearchTermsList.Any())
            {
                Expression<Func<ScrimMatchInfo, bool>> termsExpresion = null;
                Expression<Func<ScrimMatchInfo, bool>> searchTermsExpression = null;
                Expression<Func<ScrimMatchInfo, bool>> aliasTermsExpression = null;

                foreach (var term in searchFilter.SearchTermsList)
                {
                    Expression<Func<ScrimMatchInfo, bool>> exp = m => m.Title.Contains(term); // DbFunctionsExtensions.Like(EF.Functions, m.Title, "%" + term + "%");
                    searchTermsExpression = searchTermsExpression == null ? exp : searchTermsExpression.Or(exp);
                }

                termsExpresion = searchTermsExpression;


                foreach (var term in searchFilter.AliasSearchTermsList)
                {
                    Expression<Func<ScrimMatchInfo, bool>> exp = m => m.ScrimMatchId.Contains(term); // DbFunctionsExtensions.Like(EF.Functions, m.ScrimMatchId, "%" + term + "%");
                    aliasTermsExpression = aliasTermsExpression == null ? exp : aliasTermsExpression.And(exp);
                }

                termsExpresion = termsExpresion == null ? aliasTermsExpression : termsExpresion.Or(aliasTermsExpression);

                whereExpression = whereExpression == null ? termsExpresion : whereExpression.And(termsExpresion);
            }

            return whereExpression;
        }

        public async Task<ScrimMatchInfo> GetHistoricalScrimMatchInfoAsync(string scrimMatchId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var scrimMatchInfo = await dbContext.ScrimMatchInfo.Where(m => m.ScrimMatchId == scrimMatchId).FirstOrDefaultAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                if (scrimMatchInfo != null)
                {
                    scrimMatchInfo.SetTeamAliases();
                }

                return scrimMatchInfo;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfoAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfoAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<int>> GetScrimMatchBrowserFacilityIdsListAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var distinctFacilityIds = await dbContext.ScrimMatchInfo
                                                        .Where(m => m.FacilityId != null)
                                                        .Select(m => (int)m.FacilityId)
                                                        .Distinct()
                                                        .ToListAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return distinctFacilityIds;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetScrimMatchBrowserFacilityIdsListAsync");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetScrimMatchBrowserFacilityIdsListAsync");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<Ruleset>> GetScrimMatchBrowseRulesetIdsListAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var distinctRulesetIds = await dbContext.ScrimMatchInfo
                                                            .Select(m => m.RulesetId)
                                                            .Distinct()
                                                            .ToListAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                if (!distinctRulesetIds.Any())
                {
                    return null;
                }

                var distinctRulesets = await dbContext.Rulesets
                                                            .Where(r => distinctRulesetIds.Contains(r.Id))
                                                            .ToListAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return distinctRulesets;
                
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetScrimMatchBrowserFacilityIdsListAsync");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetScrimMatchBrowserFacilityIdsListAsync");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        private Ruleset ConvertToRulesetModel(ScrimMatchInfo scrimMatchInfo)
        {
            return new Ruleset
            {
                Id = scrimMatchInfo.RulesetId,
                Name = scrimMatchInfo.RulesetName
            };
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryPlayerStats>>  GetHistoricalScrimMatchInfantryPlayerStatsAsync(string scrimMatchId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryPlayerStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId)
                                        .OrderBy(e => e.NameDisplay)
                                        .ToListAsync(cancellationToken);

            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryPlayerStatsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryPlayerStatsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }
        
        public async Task<IEnumerable<ScrimMatchReportInfantryPlayerRoundStats>> GetHistoricalScrimMatchInfantryPlayerRoundStatsAsync(string scrimMatchId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryPlayerRoundStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId)
                                        .OrderBy(e => e.NameDisplay)
                                        .ThenBy(e => e.ScrimMatchRound)
                                        .ToListAsync(cancellationToken);

            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryPlayerRoundStatsAsync scrimMatchId: {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryPlayerRoundStatsAsync scrimMatchId: {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryPlayerRoundStats>> GetHistoricalScrimMatchInfantryPlayerRoundStatsAsync(string scrimMatchId, int scrimMatchRound, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryPlayerRoundStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId && e.ScrimMatchRound == scrimMatchRound)
                                        .OrderBy(e => e.NameDisplay)
                                        .ThenBy(e => e.ScrimMatchRound)
                                        .ToListAsync(cancellationToken);

            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryPlayerRoundStatsAsync scrimMatchId: {scrimMatchId} scrimMatchRound: {scrimMatchRound}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryPlayerRoundStatsAsync scrimMatchId: {scrimMatchId} scrimMatchRound: {scrimMatchRound}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }


        public async Task<IEnumerable<ScrimMatchReportInfantryTeamStats>> GetHistoricalScrimMatchInfantryTeamStatsAsync(string scrimMatchId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryTeamStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId)
                                        .OrderBy(e => e.TeamOrdinal)
                                        .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryTeamStatsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryTeamStatsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }
        
        public async Task<IEnumerable<ScrimMatchReportInfantryTeamRoundStats>> GetHistoricalScrimMatchInfantryTeamRoundStatsAsync(string scrimMatchId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryTeamRoundStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId)
                                        .OrderBy(e => e.TeamOrdinal)
                                        .ThenBy(e => e.ScrimMatchRound)
                                        .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryTeamStatsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryTeamStatsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryTeamRoundStats>> GetHistoricalScrimMatchInfantryTeamRoundStatsAsync(string scrimMatchId, int scrimMatchRound, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryTeamRoundStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId && e.ScrimMatchRound == scrimMatchRound)
                                        .OrderBy(e => e.TeamOrdinal)
                                        .ThenBy(e => e.ScrimMatchRound)
                                        .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryTeamStatsAsync scrimMatchId: {scrimMatchId} scrimMatchRound: {scrimMatchRound}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryTeamStatsAsync scrimMatchId: {scrimMatchId} scrimMatchRound: {scrimMatchRound}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryDeath>> GetHistoricalScrimMatchInfantryDeathsAsync(string scrimMatchId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryDeaths
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId)
                                        .OrderByDescending(e => e.Timestamp)
                                        .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryDeathsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryDeathsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryDeath>> GetHistoricalScrimMatchInfantryPlayerDeathsAsync(string scrimMatchId, string characterId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryDeaths
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId
                                                    && ( e.AttackerCharacterId == characterId
                                                         || e.VictimCharacterId == characterId ) )
                                        .OrderByDescending(e => e.Timestamp)
                                        .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryDeathsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryDeathsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryPlayerHeadToHeadStats>> GetHistoricalScrimMatchInfantryPlayerHeadToHeadStatsAsync(string scrimMatchId, string characterId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryPlayerHeadToHeadStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId
                                                    && e.PlayerCharacterId == characterId)
                                        .OrderByDescending(e => e.PlayerTeamOrdinal != e.OpponentTeamOrdinal)
                                        .ThenBy(e => e.OpponentNameDisplay)
                                        .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryPlayerHeadToHeadStatsAsync scrimMatchId {scrimMatchId} characterId {characterId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryPlayerHeadToHeadStatsAsync scrimMatchId {scrimMatchId} characterId {characterId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryPlayerClassEventCounts>> GetHistoricalScrimMatchInfantryPlayeClassEventCountsAsync(string scrimMatchId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryPlayerClassEventCounts
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId)
                                        .OrderByDescending(e => e.TeamOrdinal)
                                        .ThenBy(e => e.NameDisplay)
                                        .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryPlayeClassEventCountsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryPlayeClassEventCountsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryPlayerWeaponStats>> GetHistoricalScrimMatchInfantryPlayerWeaponStatsAsync(string scrimMatchId, string characterId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryPlayerWeaponStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId
                                                    && e.CharacterId == characterId)
                                        .OrderByDescending(e => e.Kills)
                                        .ThenByDescending(e => e.Deaths)
                                        .ThenBy(e => e.WeaponName)
                                        .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetHistoricalScrimMatchInfantryPlayeClassEventCountsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetHistoricalScrimMatchInfantryPlayeClassEventCountsAsync scrimMatchId {scrimMatchId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        //public async Task<PaginatedList<ScrimMatchReportInfantryDeath>> GetHistoricalScrimMatchInfantryPlayerDeathsAsync(string scrimMatchId, int? pageIndex)
        //{
        //    try
        //    {
        //        using var factory = _dbContextHelper.GetFactory();
        //        var dbContext = factory.GetDbContext();

        //        var scrimMatchesQuery = dbContext.ScrimMatchReportInfantryDeaths.Where(d => d.ScrimMatchId == scrimMatchId).AsQueryable();

        //        var paginatedList = await PaginatedList<ScrimMatchReportInfantryDeath>.CreateAsync(scrimMatchesQuery.AsNoTracking().OrderByDescending(d => d.Timestamp), pageIndex ?? 1, _scrimMatchBrowserPageSize);

        //        if (paginatedList == null)
        //        {
        //            return null;
        //        }

        //        return paginatedList;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"{ex}");

        //        return null;
        //    }
        //}
    }
}
