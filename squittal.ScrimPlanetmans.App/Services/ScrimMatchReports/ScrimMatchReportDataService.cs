using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
using System;
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
            /*
             SELECT config1.ScrimMatchId, MAX(StartTime), MAX(config1.ScrimMatchRound), MAX(config1.Title)
                FROM ScrimMatchRoundConfiguration config1
                INNER JOIN ScrimMatchRoundConfiguration config2
                    ON config1.ScrimMatchId = config2.ScrimMatchId
                INNER JOIN ScrimMatch match
                    ON config1.ScrimMatchId = match.Id 
                WHERE config1.ScrimMatchRound > config2.ScrimMatchRound
                GROUP BY config1.ScrimMatchId
                HAVING MAX(config1.ScrimMatchRound) > 1
                ORDER BY MAX(StartTime) DESC 
            */

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
    }
}
