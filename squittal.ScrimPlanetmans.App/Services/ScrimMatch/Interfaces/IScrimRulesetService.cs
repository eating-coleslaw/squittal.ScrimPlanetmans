using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IScrimRulesetService
    {
        Task<ScrimRuleset> GetRulesetFromId(int id);
        Task<ScrimRuleset> GetRulesetFromName(string name);
        Task<ScrimRuleset> GetLatestRuleset();

        Task<ScrimRuleset> GetDefaultRuleset();

        Task<IEnumerable<int>> GetAllRulesetIds();
        Task<IEnumerable<string>> GetAllRulesetNames();

        Task SaveRuleset(ScrimRuleset ruleset);

        Task SaveActionRule(ScrimActionRulePoints rule);
        Task SaveItemCategoryRule(ItemCategoryRule rule);

        Task RefreshStore();
    }
}
