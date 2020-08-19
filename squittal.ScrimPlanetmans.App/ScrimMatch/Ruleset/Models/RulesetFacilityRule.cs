using squittal.ScrimPlanetmans.Models.Planetside;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class RulesetFacilityRule
    {
        [Required]
        public int RulesetId { get; set; }
        [Required]
        public int FacilityId { get; set; }

        public Ruleset Ruleset { get; set; }
        public MapRegion MapRegion { get; set; }
    }
}
