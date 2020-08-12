using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Rulesets
{
    public interface IRulesetDataService
    {
        Task<PaginatedList<Ruleset>> GetRulesetListAsync(int? pageIndex, CancellationToken cancellationToken);

        Task<Ruleset> GetRulesetFromIdAsync(int rulesetId, CancellationToken cancellationToken, bool includeCollections = true);
        Task<Ruleset> GetRulesetFromNameAsync(string name);
        Task<Ruleset> GetLatestRulesetAsync();

        Task<Ruleset> GetDefaultRulesetAsync();

        Task<IEnumerable<int>> GetAllRulesetIdsAsync();
        Task<IEnumerable<string>> GetAllRulesetNamesAsync();

        Task SaveRulesetAsync(Ruleset ruleset);

        Task SaveActionRuleAsync(RulesetActionRule rule);
        Task SaveItemCategoryRuleAsync(RulesetItemCategoryRule rule);

        Task RefreshStoreAsync();
        Task<IEnumerable<RulesetActionRule>> GetRulesetActionRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetItemCategoryRule>> GetRulesetItemCategoryRulesAsync(int rulesetId, CancellationToken cancellationToken);
    }
}
