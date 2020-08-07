using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<PaginatedList<ScrimMatchInfo>> GetHistoricalScrimMatchesListAsync(int? pageIndex, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var scrimMatchesQuery = dbContext.ScrimMatchInfo.AsQueryable();

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
