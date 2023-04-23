using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.Models.Planetside;
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
        Task<Ruleset> GetRulesetFromIdAsync(int rulesetId, CancellationToken cancellationToken, bool includeCollections = true, bool includeOverlayConfiguration = true);
        Task<IEnumerable<Ruleset>> GetAllRulesetsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<RulesetActionRule>> GetRulesetActionRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<RulesetOverlayConfiguration> GetRulesetOverlayConfigurationAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetItemCategoryRule>> GetRulesetItemCategoryRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetItemRule>> GetRulesetItemRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetFacilityRule>> GetRulesetFacilityRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetFacilityRule>> GetUnusedRulesetFacilityRulesAsync(int rulesetId, CancellationToken cancellationToken);

        Task<Ruleset> GetRulesetWithFacilityRules(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetItemCategoryRule>> GetRulesetItemCategoryRulesDeferringToItemRules(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<ItemCategory>> GetItemCategoriesDeferringToItemRules(int rulesetId, CancellationToken cancellationToken);
        
        Task<Ruleset> SaveNewRulesetAsync(Ruleset ruleset);
        Task<bool> UpdateRulesetInfo(Ruleset rulesetUpdate, CancellationToken cancellationToken);
        Task<bool> SaveRulesetOverlayConfiguration(int rulesetId, RulesetOverlayConfiguration rulesetOverlayConfiguration, CancellationToken cancellationToken);
        Task SaveRulesetActionRules(int rulesetId, IEnumerable<RulesetActionRule> rules);
        Task SaveRulesetItemCategoryRules(int rulesetId, IEnumerable<RulesetItemCategoryRule> rules, bool isFromStoreRefresh);
        Task SaveRulesetItemRules(int rulesetId, IEnumerable<RulesetItemRule> rules, bool isFromStoreRefresh);
        Task SaveRulesetFacilityRules(int rulesetId, IEnumerable<RulesetFacilityRuleChange> rules, bool isFromStoreRefresh);

        void SetActiveRulesetId(int rulesetId);
        Task<Ruleset> SetCustomDefaultRulesetAsync(int rulesetId);

        Task<bool> CanDeleteRuleset(int rulesetId, CancellationToken cancellationToken);
        Task<bool> HasRulesetBeenUsedAsync(int rulesetId, CancellationToken cancellationToken);
        Task<bool> DeleteRulesetAsync(int rulesetId);
        
        Task<bool> ExportRulesetToJsonFile(int rulesetId, CancellationToken cancellationToken);
        Task<Ruleset> ImportNewRulesetFromJsonFile(string fileName, bool returnCollections = false, bool returnOverlayConfiguration = false);
        IEnumerable<string> GetJsonRulesetFileNames();
    }
}
