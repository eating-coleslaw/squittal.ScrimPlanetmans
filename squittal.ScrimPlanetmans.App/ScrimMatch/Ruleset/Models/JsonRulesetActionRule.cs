using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class JsonRulesetActionRule
    {
        public string ScrimActionTypeName { get; set; }
        public ScrimActionType ScrimActionType { get; set; }
        public int Points { get; set; }
        public bool DeferToItemCategoryRules { get; set; }

        public JsonRulesetActionRule()
        {
        }

        public JsonRulesetActionRule(RulesetActionRule rule)
        {
            ScrimActionTypeName = Enum.GetName(typeof(ScrimActionType), rule.ScrimActionType);
            ScrimActionType = rule.ScrimActionType;
            Points = rule.Points;
            DeferToItemCategoryRules = rule.DeferToItemCategoryRules;
        }
    }
}
