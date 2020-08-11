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
        public bool IsActive { get; set; }
        public bool IsCustomDefault { get; set; }
        public bool IsDefault { get; set; }

        //public bool IsFavorite { get; set; }

        //public int DefaultRoundTime { get; set; }
        //public int DefaultRounds { get; set; }

        public ICollection<RulesetItemCategoryRule> RulesetItemCategoryRules { get; set; }
        public ICollection<RulesetActionRule> RulesetActionRules { get; set; }
        //public ICollection<RulesetFacility> Facilities { get; set; }

    }
}
