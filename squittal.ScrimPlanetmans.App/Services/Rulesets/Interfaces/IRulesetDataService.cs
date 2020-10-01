using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Rulesets
{
    public interface IRulesetDataService
    {
        int DefaultRulesetId { get; }
        int ActiveRulesetId { get; }
        int CustomDefaultRulesetId { get; }

        Task<PaginatedList<Ruleset>> GetRulesetListAsync(int? pageIndex, CancellationToken cancellationToken);
        Task<Ruleset> GetRulesetFromIdAsync(int rulesetId, CancellationToken cancellationToken, bool includeCollections = true);
        Task<IEnumerable<RulesetActionRule>> GetRulesetActionRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetItemCategoryRule>> GetRulesetItemCategoryRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<Ruleset>> GetAllRulesetsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RulesetFacilityRule>> GetRulesetFacilityRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetFacilityRule>> GetUnusedRulesetFacilityRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<Ruleset> GetRulesetWithFacilityRules(int rulesetId, CancellationToken cancellationToken);
        
        Task SaveRulesetItemCategoryRules(int rulesetId, IEnumerable<RulesetItemCategoryRule> rules);
        Task SaveRulesetActionRules(int rulesetId, IEnumerable<RulesetActionRule> rules);
        Task<Ruleset> SaveNewRulesetAsync(Ruleset ruleset);
        Task SaveRulesetFacilityRules(int rulesetId, IEnumerable<RulesetFacilityRuleChange> rules);
        Task<bool> UpdateRulesetInfo(Ruleset rulesetUpdate, CancellationToken cancellationToken);
        
        void SetActiveRulesetId(int rulesetId);
        Task<Ruleset> SetCustomDefaultRulesetAsync(int rulesetId);
        Task<bool> ExportRulesetToJsonFile(int rulesetId, CancellationToken cancellationToken);
        Task<Ruleset> ImportNewRulesetFromJsonFile(string fileName, bool returnCollections = false);
        IEnumerable<string> GetJsonRulesetFileNames();
        Task<bool> CanDeleteRuleset(int rulesetId, CancellationToken cancellationToken);
        Task<bool> HasRulesetBeenUsedAsync(int rulesetId, CancellationToken cancellationToken);
        Task<bool> DeleteRulesetAsync(int rulesetId);
        Task<IEnumerable<RulesetItemRule>> GetRulesetItemRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task SaveRulesetItemRules(int rulesetId, IEnumerable<RulesetItemRule> rules);
        Task<IEnumerable<RulesetItemCategoryRule>> GetRulesetItemCategoryRulesDeferringToItemRules(int rulesetId, CancellationToken cancellationToken);
    }
}
