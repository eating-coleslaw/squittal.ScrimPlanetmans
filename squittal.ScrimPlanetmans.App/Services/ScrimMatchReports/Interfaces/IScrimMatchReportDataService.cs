using squittal.ScrimPlanetmans.Models.ScrimMatchReports;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatchReports
{
    public interface IScrimMatchReportDataService
    {
        Task<PaginatedList<ScrimMatchInfo>> GetHistoricalScrimMatchesListAsync(int? pageIndex);
    }
}
