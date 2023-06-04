using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatchReports
{
    public interface IScrimMatchReportDataService
    {
        Task<PaginatedList<ScrimMatchInfo>> GetHistoricalScrimMatchesListAsync(int? pageIndex, ScrimMatchReportBrowserSearchFilter searchFilter, CancellationToken cancellationToken);
        //Task<PaginatedList<ScrimMatchReportInfantryDeath>> GetHistoricalScrimMatchInfantryPlayerDeathsAsync(string scrimMatchId, int? pageIndex);
        Task<IEnumerable<ScrimMatchReportInfantryDeath>> GetHistoricalScrimMatchInfantryDeathsAsync(string scrimMatchId, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryPlayerClassEventCounts>> GetHistoricalScrimMatchInfantryPlayeClassEventCountsAsync(string scrimMatchId, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryDeath>> GetHistoricalScrimMatchInfantryPlayerDeathsAsync(string scrimMatchId, string characterId, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryPlayerHeadToHeadStats>> GetHistoricalScrimMatchInfantryPlayerHeadToHeadStatsAsync(string scrimMatchId, string characterId, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryPlayerRoundStats>> GetHistoricalScrimMatchInfantryPlayerRoundStatsAsync(string scrimMatchId, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryPlayerRoundStats>> GetHistoricalScrimMatchInfantryPlayerRoundStatsAsync(string scrimMatchId, int scrimMatchRound, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryPlayerStats>> GetHistoricalScrimMatchInfantryPlayerStatsAsync(string scrimMatchId, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryPlayerWeaponStats>> GetHistoricalScrimMatchInfantryPlayerWeaponStatsAsync(string scrimMatchId, string characterId, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryTeamRoundStats>> GetHistoricalScrimMatchInfantryTeamRoundStatsAsync(string scrimMatchId, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryTeamRoundStats>> GetHistoricalScrimMatchInfantryTeamRoundStatsAsync(string scrimMatchId, int scrimMatchRound, CancellationToken cancellationToken);
        Task<IEnumerable<ScrimMatchReportInfantryTeamStats>> GetHistoricalScrimMatchInfantryTeamStatsAsync(string scrimMatchId, CancellationToken cancellationToken);
        Task<ScrimMatchInfo> GetHistoricalScrimMatchInfoAsync(string scrimMatchId, CancellationToken cancellationToken);
        Task<IEnumerable<int>> GetScrimMatchBrowserFacilityIdsListAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Ruleset>> GetScrimMatchBrowseRulesetIdsListAsync(CancellationToken cancellationToken);
    }
}
