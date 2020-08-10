using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
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
                                                    /*
                                                    .Where(m => m.RoundCount >= searchFilter.MinimumRoundCount
                                                             && ( m.FacilityId == searchFilter.FacilityId 
                                                                  || ( m.FacilityId != null && searchFilter.FacilityId == 0 ) // Any Facility
                                                                  //|| ( m.FacilityId == null && searchFilter.FacilityId == -1 ) ) // No Facility Filter
                                                                  || searchFilter.FacilityId == -1 ) // Don't Filter
                                                             && ( m.WorldId == searchFilter.WorldId || searchFilter.WorldId == 0 )
                                                             //&& ( (searchFilter.SearchTermsList.Any(t => DbFunctionsExtensions.Like(EF.Functions, m.Title, "%" + t + "%") ) )
                                                             //&& ( DbFunctionsExtensions.Like(EF.Functions, m.Title, "%" + searchFilter.InputSearchTerms + "%")
                                                             //&& ( m.Title.Contains(searchFilter.InputSearchTerms)
                                                                  //|| m.Title.Intersect(searchFilter.SearchTermsList).Count() > 0 // searchFilter.SearchTermsList.Any(t => m.Title.Contains(t))
                                                             //&& ( ( !searchFilter.SearchTermsList.Any() || searchFilter.SearchTermsList.Count == 0
                                                             //       //|| searchFilter.SearchTermsList.Any(t => m.Title.ToLower().Contains(t)) ) 
                                                             //       || searchFilter.SearchTermsList.Any(t => m.Title.Contains(t)) ) 
                                                                    ) //)
                                                                  //&& ( !searchFilter.AliasSearchTermsList.Any()
                                                                  //     || searchFilter.AliasSearchTermsList.Any(t => m.ScrimMatchId.ToLower().Contains(t) ) ) ) )
                                                    //.Where(m => m.FacilityId == searchFilter.FacilityId || searchFilter.FacilityId == -1)
                                                    //.Where(m => m.WorldId == searchFilter.WorldId || searchFilter.WorldId == -1)
                                                    //.Where(m => searchFilter.AliasSearchTermsList.Count == 0
                                                                //|| searchFilter.AliasSearchTermsList.Any(t => DbFunctionsExtensions.Like(EF.Functions, m.ScrimMatchId, "%" + t + "%")) )
                                                    */
                                                    .AsQueryable();



                //Expression<Func< ScrimMatchInfo, bool>> whereExpression = null;
                //Expression<Func< ScrimMatchInfo, bool>> whereExpression = m => searchFilter.AliasSearchTermsList.Count == 0;

                //foreach (var term in searchFilter.AliasSearchTermsList)
                //{
                //    Expression<Func<ScrimMatchInfo, bool>> e1 = m => DbFunctionsExtensions.Like(EF.Functions, m.ScrimMatchId, "%" + term + "%");
                //    whereExpression = whereExpression == null ? e1 : whereExpression.Or(e1);
                //}

                //scrimMatchesQuery = scrimMatchesQuery.Where(whereExpression);

                //return query.Where(whereExpression);



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
            //Expression<Func<ScrimMatchInfo, bool>> whereExpression = m => m.RoundCount >= searchFilter.MinimumRoundCount;

            if (isDefaultFilter)
            {
                Expression<Func<ScrimMatchInfo, bool>> roundExpression = m => m.RoundCount >= searchFilter.MinimumRoundCount;

                var twoHoursAgo = DateTime.UtcNow - TimeSpan.FromHours(2);
                Expression<Func<ScrimMatchInfo, bool>> recentMatchExpression = m => m.StartTime >= twoHoursAgo;

                roundExpression = roundExpression.Or(recentMatchExpression);

                whereExpression = whereExpression == null ? roundExpression : whereExpression.And(roundExpression);

                //Expression<Func<ScrimMatchInfo, bool>> whereExpression = m => m.RoundCount >= searchFilter.MinimumRoundCount;
                //Expression<Func<ScrimMatchInfo, bool>> whereExpression = m => m.RoundCount >= searchFilter.MinimumRoundCount;
            }
            else
            {
                Expression<Func<ScrimMatchInfo, bool>> roundExpression = m => m.RoundCount >= searchFilter.MinimumRoundCount;

                whereExpression = whereExpression == null ? roundExpression : whereExpression.And(roundExpression);
            }

            //Expression<Func<ScrimMatchInfo, bool>> whereExpression = m => searchFilter.AliasSearchTermsList.Count == 0;

            //Expression<Func<ScrimMatchInfo, bool>> roundExpression = m => m.RoundCount >= searchFilter.MinimumRoundCount;

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
                    //termsExpresion = termsExpresion == null ? exp : termsExpresion.Or(exp);
                    searchTermsExpression = searchTermsExpression == null ? exp : searchTermsExpression.Or(exp);
                }

                termsExpresion = searchTermsExpression;


                foreach (var term in searchFilter.AliasSearchTermsList)
                {
                    Expression<Func<ScrimMatchInfo, bool>> exp = m => m.ScrimMatchId.Contains(term); // DbFunctionsExtensions.Like(EF.Functions, m.ScrimMatchId, "%" + term + "%");
                    //termsExpresion = termsExpresion == null ? exp : termsExpresion.Or(exp);
                    //termsExpresion = termsExpresion == null ? exp : termsExpresion.And(exp);
                    aliasTermsExpression = aliasTermsExpression == null ? exp : aliasTermsExpression.And(exp);
                }

                termsExpresion = termsExpresion == null ? aliasTermsExpression : termsExpresion.Or(aliasTermsExpression);

                whereExpression = whereExpression == null ? termsExpresion : whereExpression.And(termsExpresion);
            }

            return whereExpression;


            //scrimMatchesQuery = scrimMatchesQuery.Where(whereExpression);

            //Where(m => m.RoundCount >= searchFilter.MinimumRoundCount
            //                                                 && (m.FacilityId == searchFilter.FacilityId
            //                                                      || (m.FacilityId != null && searchFilter.FacilityId == 0) // Any Facility
            //                                                      || searchFilter.FacilityId == -1) // Don't Filter
            //                                                 && (m.WorldId == searchFilter.WorldId || searchFilter.WorldId == 0)
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
                                        .OrderByDescending(e => e.TeamOrdinal)
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
