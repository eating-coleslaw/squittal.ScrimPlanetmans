using squittal.ScrimPlanetmans.Models.ScrimEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IScrimMatchDataService
    {
        string CurrentMatchId { get; set; }
        int CurrentMatchRound { get; set; }

        Task SaveToCurrentMatch(Data.Models.ScrimMatch scrimMatch);

        Task SaveCurrentMatchRoundConfiguration(MatchConfiguration matchConfiguration);
        Task RemoveMatchRoundConfiguration(int roundToDelete);


        Task<Data.Models.ScrimMatch> GetCurrentMatch();

        IEnumerable<Data.Models.ScrimMatch> GetAllMatches();


    }
}
