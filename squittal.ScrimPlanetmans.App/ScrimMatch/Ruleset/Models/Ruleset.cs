using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class Ruleset
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }

        public bool IsDefault { get; set; }
        public bool IsCustomDefault { get; set; }

        public string SourceFile { get; set; }

        public string DefaultMatchTitle { get; set; } = string.Empty;
        public int DefaultRoundLength { get; set; } = 900;
        public bool DefaultEndRoundOnFacilityCapture { get; set; } = false;


        public ICollection<RulesetActionRule> RulesetActionRules { get; set; }
        public ICollection<RulesetItemCategoryRule> RulesetItemCategoryRules { get; set; }
        public ICollection<RulesetItemRule> RulesetItemRules { get; set; }
        public ICollection<RulesetFacilityRule> RulesetFacilityRules { get; set; }

        public RulesetOverlayConfiguration RulesetOverlayConfiguration { get; set; }

    }
}
