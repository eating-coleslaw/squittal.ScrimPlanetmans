using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IScrimMatchDataService
    {
        string CurrentMatchId { get; set; }
        int CurrentMatchRound { get; set; }
        int CurrentMatchRulesetId { get; set; }

        Task SaveToCurrentMatch(Data.Models.ScrimMatch scrimMatch);

        Task SaveCurrentMatchRoundConfiguration(MatchConfiguration matchConfiguration);
        Task RemoveMatchRoundConfiguration(int roundToDelete);


        Task<Data.Models.ScrimMatch> GetCurrentMatch();

        IEnumerable<Data.Models.ScrimMatch> GetAllMatches();
        Task<bool> TryRemoveMatchParticipatingPlayer(string characterId);
        Task SaveMatchParticipatingPlayer(Player player);
        Task SaveScrimPeriodicControlTick(ScrimPeriodicControlTick dataModel);
    }
}
