using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class RulesetRuleChangeMessage
    {
        public Ruleset Ruleset { get; set; }
        public RulesetRuleChangeType RuleChangeType { get; set; }
        public string Info { get; set; }

        public RulesetRuleChangeMessage(Ruleset ruleset, RulesetRuleChangeType changeType)
        {
            Ruleset = ruleset;
            RuleChangeType = changeType;
            Info = $"Rules changed for ruleset {Ruleset.Name} [{Ruleset.Id}]: {GetEnumValueName(changeType)}";
        }

        public string GetEnumValueName(RulesetRuleChangeType type)
        {
            return Enum.GetName(typeof(RulesetRuleChangeType), type);
        }
    }

    public enum RulesetRuleChangeType
    {
        ActionRule,
        ItemCategoryRule,
        FacilityRule
    }
}
