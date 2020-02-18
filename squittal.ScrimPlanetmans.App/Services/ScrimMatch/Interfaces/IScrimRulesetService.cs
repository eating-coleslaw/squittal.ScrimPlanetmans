using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IScrimRulesetService
    {
        Task<Ruleset> GetRulesetFromId(int id);
        Task<Ruleset> GetRulesetFromName(string name);
        Task<Ruleset> GetLatestRuleset();

        Task<Ruleset> GetDefaultRuleset();

        Task<IEnumerable<int>> GetAllRulesetIds();
        Task<IEnumerable<string>> GetAllRulesetNames();

        Task SaveRuleset(Ruleset ruleset);

        Task SaveActionRule(RulesetActionRule rule);
        Task SaveItemCategoryRule(RulesetItemCategoryRule rule);

        Task RefreshStore();
    }
}
