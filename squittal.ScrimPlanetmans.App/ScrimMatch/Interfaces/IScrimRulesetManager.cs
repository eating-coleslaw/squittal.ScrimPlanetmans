using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimRulesetManager
    {
        Task<Ruleset> GetDefaultRuleset();
        Task SeedDefaultRuleset();
        Task SeedScrimActionModels();
    }
}
