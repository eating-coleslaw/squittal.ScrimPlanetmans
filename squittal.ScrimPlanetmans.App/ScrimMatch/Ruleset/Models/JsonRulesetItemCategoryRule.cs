using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class JsonRulesetItemCategoryRule
    {
        public string ItemCategoryName { get; set; }
        public int ItemCategoryId { get; set; }
        public int Points { get; set; }
        public bool IsBanned { get; set; }
        public bool DeferToItemRules { get; set; }

        public ICollection<JsonRulesetItemRule> RulesetItemRules { get; set; }

        public JsonRulesetItemCategoryRule()
        {
        }

        public JsonRulesetItemCategoryRule(RulesetItemCategoryRule rule)
        {
            ItemCategoryName = rule.ItemCategory.Name;
            ItemCategoryId = rule.ItemCategoryId;
            Points = rule.Points;
            IsBanned = rule.IsBanned;
            DeferToItemRules = rule.DeferToItemRules;
        }

        public JsonRulesetItemCategoryRule(RulesetItemCategoryRule rule, ICollection<JsonRulesetItemRule> itemCategoryItemRules)
        {
            ItemCategoryName = rule.ItemCategory.Name;
            ItemCategoryId = rule.ItemCategoryId;
            Points = rule.Points;
            IsBanned = rule.IsBanned;
            DeferToItemRules = rule.DeferToItemRules;

            RulesetItemRules = itemCategoryItemRules;
        }
    }
}
