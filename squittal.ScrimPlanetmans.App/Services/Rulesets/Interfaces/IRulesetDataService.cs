﻿using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
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
        int DefaultRulesetId { get; }
        int ActiveRulesetId { get; }
        int CustomDefaultRulesetId { get; }

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
        Task SaveRulesetItemCategoryRules(int rulesetId, IEnumerable<RulesetItemCategoryRule> rules);
        Task SaveRulesetActionRules(int rulesetId, IEnumerable<RulesetActionRule> rules);
        Task<Ruleset> SaveNewRulesetAsync(Ruleset ruleset);
        Task<IEnumerable<Ruleset>> GetAllRulesetsAsync(CancellationToken cancellationToken);
        //Task<Ruleset> ActivateRulesetAsync(int rulesetId);
        Task SaveRulesetFacilityRules(int rulesetId, IEnumerable<RulesetFacilityRuleChange> rules);
        Task<IEnumerable<RulesetFacilityRule>> GetRulesetFacilityRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<IEnumerable<RulesetFacilityRule>> GetUnusedRulesetFacilityRulesAsync(int rulesetId, CancellationToken cancellationToken);
        Task<bool> UpdateRulesetInfo(Ruleset rulesetUpdate, CancellationToken cancellationToken);
        Task<Ruleset> GetRulesetWithFacilityRules(int rulesetId, CancellationToken cancellationToken);
        void SetActiveRulesetId(int rulesetId);
        Task<Ruleset> SetCustomDefaultRulesetAsync(int rulesetId);
    }
}
