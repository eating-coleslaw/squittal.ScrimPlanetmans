using squittal.ScrimPlanetmans.Models.Planetside;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class RulesetItemRule
    {
        [Required]
        public int RulesetId { get; set; }

        [Required]
        public int ItemId { get; set; }

        public int ItemCategoryId { get; set; }

        public int Points { get; set; }

        public bool IsBanned { get; set; }

        public Ruleset Ruleset { get; set; }
        public Item Item { get; set; }
        public ItemCategory ItemCategory { get; set; }
    }
}
