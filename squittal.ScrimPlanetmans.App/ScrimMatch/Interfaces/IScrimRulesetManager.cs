using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimRulesetManager
    {
        //event EventHandler<ActiveRulesetChangeEventArgs> RaiseActiveRulesetChangeEvent;
        //delegate void ActiveRulesetChangeMessageEventHandler(object sender, ActiveRulesetChangeEventArgs e);

        Task<Ruleset> GetActiveRulesetAsync(bool forceRefresh = false);
        Task<Ruleset> GetDefaultRuleset();

        Task<Ruleset> ActivateRuleset(int rulesetId);
        Task SetupActiveRuleset();

        Task SeedDefaultRuleset();
        Task SeedScrimActionModels();
    }
}
