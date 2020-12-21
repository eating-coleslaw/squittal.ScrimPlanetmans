using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class RulesetOverlayConfiguration
    {
        [Required]
        public int RulesetId { get; set; }
        public bool UseCompactLayout { get; set; }
        public OverlayStatsDisplayType StatsDisplayType { get; set; }
        public bool? ShowStatusPanelScores { get; set; }

        public Ruleset Ruleset { get; set; }
    }
}
