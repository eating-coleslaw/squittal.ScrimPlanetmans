using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<PaginatedList<ScrimMatchInfo>> GetHistoricalScrimMatchesListAsync(int? pageIndex)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var scrimMatchesQuery = dbContext.ScrimMatchInfo.AsQueryable();

                var paginatedList = await PaginatedList<ScrimMatchInfo>.CreateAsync(scrimMatchesQuery.AsNoTracking().OrderByDescending(m => m.StartTime), pageIndex ?? 1, _scrimMatchBrowserPageSize);

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
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                
                return null;
            }
        }

        public async Task<ScrimMatchInfo> GetHistoricalScrimMatchInfoAsync(string scrimMatchId)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var scrimMatchInfo = await dbContext.ScrimMatchInfo.Where(m => m.ScrimMatchId == scrimMatchId).FirstOrDefaultAsync();
                
                if (scrimMatchInfo != null)
                {
                    scrimMatchInfo.SetTeamAliases();
                }

                return scrimMatchInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryPlayerStats>>  GetHistoricalScrimMatchInfantryPlayerStatsAsync(string scrimMatchId)
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
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<ScrimMatchReportInfantryTeamStats>> GetHistoricalScrimMatchInfantryTeamStatsAsync(string scrimMatchId)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatchReportInfantryTeamStats
                                        .AsNoTracking()
                                        .Where(e => e.ScrimMatchId == scrimMatchId)
                                        .OrderBy(e => e.TeamOrdinal)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }
    }
}
