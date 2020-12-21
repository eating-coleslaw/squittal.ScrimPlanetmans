using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class JsonRulesetOverlayConfiguration
    {
        public bool? UseCompactLayout { get; set; }
        public string StatsDisplayTypeName { get; set; }
        public OverlayStatsDisplayType? StatsDisplayType { get; set; }
        public bool? ShowStatusPanelScores { get; set; }

        public JsonRulesetOverlayConfiguration()
        {
        }

        public JsonRulesetOverlayConfiguration(RulesetOverlayConfiguration configuration)
        {
            StatsDisplayTypeName = Enum.GetName(typeof(OverlayStatsDisplayType), configuration.StatsDisplayType);
            StatsDisplayType = configuration.StatsDisplayType;
            UseCompactLayout = configuration.UseCompactLayout;
            ShowStatusPanelScores = configuration.ShowStatusPanelScores;
        }
    }
}
