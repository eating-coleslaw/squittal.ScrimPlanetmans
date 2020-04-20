using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimRulesetManager
    {
        Task<Ruleset> GetActiveRuleset(bool forceRefresh = false);
        Task<Ruleset> GetDefaultRuleset();

        Task<Ruleset> ActivateRuleset(int rulesetId);
        Task SetupActiveRuleset();

        Task SeedDefaultRuleset();
        Task SeedScrimActionModels();
    }
}
