using squittal.ScrimPlanetmans.Models.Planetside;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class RulesetFacilityRule
    {
        [Required]
        public int RulesetId { get; set; }
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public int MapRegionId { get; set; }

        public Ruleset Ruleset { get; set; }
        public MapRegion MapRegion { get; set; }
    }
}
