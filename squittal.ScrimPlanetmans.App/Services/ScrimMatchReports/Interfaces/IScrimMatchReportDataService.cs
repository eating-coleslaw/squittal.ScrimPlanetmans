using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatchReports
{
    public interface IScrimMatchReportDataService
    {
        Task<PaginatedList<ScrimMatchInfo>> GetHistoricalScrimMatchesListAsync(int? pageIndex);
        Task<PaginatedList<ScrimMatchReportInfantryDeath>> GetHistoricalScrimMatchInfantryPlayerDeathsAsync(string scrimMatchId, int? pageIndex);
        Task<IEnumerable<ScrimMatchReportInfantryPlayerStats>> GetHistoricalScrimMatchInfantryPlayerStatsAsync(string scrimMatchId);
        Task<IEnumerable<ScrimMatchReportInfantryTeamStats>> GetHistoricalScrimMatchInfantryTeamStatsAsync(string scrimMatchId);
        Task<ScrimMatchInfo> GetHistoricalScrimMatchInfoAsync(string scrimMatchId);
    }
}
