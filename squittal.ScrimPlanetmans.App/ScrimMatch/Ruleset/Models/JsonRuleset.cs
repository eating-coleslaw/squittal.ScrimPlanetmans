using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class JsonRuleset
    {
        //public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastModified { get; set; }

        public bool IsDefault { get; set; }
        //public bool IsCustomDefault { get; set; }

        public string FileName { get; set; }
        //public string SourceFileName { get; set; }

        public string DefaultMatchTitle { get; set; } = string.Empty;
        public int DefaultRoundLength { get; set; } = 900;

        public ICollection<JsonRulesetItemCategoryRule> RulesetItemCategoryRules { get; set; }
        public ICollection<JsonRulesetActionRule> RulesetActionRules { get; set; }
        public ICollection<JsonRulesetFacilityRule> RulesetFacilityRules { get; set; }

        public JsonRuleset()
        {
        }

        public JsonRuleset(Ruleset ruleset, string fileName)
        {
            //Id = ruleset.Id;
            Name = ruleset.Name;
            DateCreated = ruleset.DateCreated;
            DateLastModified = ruleset.DateLastModified;
            IsDefault = ruleset.IsDefault;
            FileName = fileName;
            //SourceFileName = ruleset.SourceFileName;
            DefaultMatchTitle = ruleset.DefaultMatchTitle;
            DefaultRoundLength = ruleset.DefaultRoundLength;

            if (ruleset.RulesetItemCategoryRules.Any())
            {
                RulesetItemCategoryRules = ruleset.RulesetItemCategoryRules.Select(r => new JsonRulesetItemCategoryRule(r)).ToArray();
            }
            
            if (ruleset.RulesetActionRules.Any())
            {
                RulesetActionRules = ruleset.RulesetActionRules.Select(r => new JsonRulesetActionRule(r)).ToArray();
            }
            
            if (ruleset.RulesetFacilityRules.Any())
            {
                RulesetFacilityRules = ruleset.RulesetFacilityRules.Select(r => new JsonRulesetFacilityRule(r)).ToArray();
            }
        }
    }
}
