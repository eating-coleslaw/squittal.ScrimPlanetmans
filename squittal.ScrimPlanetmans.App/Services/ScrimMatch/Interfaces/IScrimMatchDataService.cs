using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IScrimMatchDataService
    {
        string CurrentMatchId { get; set; }

        Task SaveToCurrentMatch(Data.Models.ScrimMatch scrimMatch);

        Task<Data.Models.ScrimMatch> GetCurrentMatch();

        IEnumerable<Data.Models.ScrimMatch> GetAllMatches();


    }
}
