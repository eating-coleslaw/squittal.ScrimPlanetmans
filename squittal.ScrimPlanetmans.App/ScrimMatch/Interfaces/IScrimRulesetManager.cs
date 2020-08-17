using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimRulesetManager
    {
        Task<Ruleset> GetActiveRulesetAsync(bool forceRefresh = false);
        Task<Ruleset> GetDefaultRuleset();

        Task<Ruleset> ActivateRuleset(int rulesetId);
        Task SetupActiveRuleset();

        Task SeedDefaultRuleset();
        Task SeedScrimActionModels();
        Task<IEnumerable<Ruleset>> GetRulesetsAsync(CancellationToken cancellationToken);
    }
}
