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

        public bool DeferToPlanetsideClassSettings { get; set; }

        public bool InfiltratorIsBanned { get; set; }
        public int InfiltratorPoints { get; set; }

        public bool LightAssaultIsBanned { get; set; }
        public int LightAssaultPoints { get; set; }

        public bool MedicIsBanned { get; set; }
        public int MedicPoints { get; set; }

        public bool EngineerIsBanned { get; set; }
        public int EngineerPoints { get; set; }

        public bool HeavyAssaultIsBanned { get; set; }
        public int HeavyAssaultPoints { get; set; }

        public bool MaxIsBanned { get; set; }
        public int MaxPoints { get; set; }

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

            DeferToPlanetsideClassSettings = rule.DeferToPlanetsideClassSettings;
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }

        public JsonRulesetItemCategoryRule(RulesetItemCategoryRule rule, ICollection<JsonRulesetItemRule> itemCategoryItemRules)
        {
            ItemCategoryName = rule.ItemCategory.Name;
            ItemCategoryId = rule.ItemCategoryId;
            Points = rule.Points;
            IsBanned = rule.IsBanned;
            DeferToItemRules = rule.DeferToItemRules;

            DeferToPlanetsideClassSettings = rule.DeferToPlanetsideClassSettings;
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;

            RulesetItemRules = itemCategoryItemRules;
        }
    }
}
