using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimRulesetManager
    {
        Task SeedDefaultRuleset();
        Task SeedScrimActionModels();
    }
}
