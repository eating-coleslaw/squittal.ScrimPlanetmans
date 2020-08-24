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
        public DateTime DateCreated { get; set; } // TODO: Date of first save (?)

        public DateTime? DateLastModified { get; set; }

        //TODO: Actually set these values
        //public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCustomDefault { get; set; }

        //public string FileName { get; set; }
        // public string SourceFileName { get; set; }

        //public bool IsFavorite { get; set; }

        public string DefaultMatchTitle { get; set; } = string.Empty;
        public int DefaultRoundLength { get; set; } = 900;
        //public int DefaultRounds { get; set; } == 2

        public ICollection<RulesetItemCategoryRule> RulesetItemCategoryRules { get; set; }
        public ICollection<RulesetActionRule> RulesetActionRules { get; set; }
        public ICollection<RulesetFacilityRule> RulesetFacilityRules { get; set; }

    }
}
