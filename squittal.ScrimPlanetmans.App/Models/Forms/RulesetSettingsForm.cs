using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Models.Forms
{
    public class RulesetSettingsForm
    {
        public int RulesetId { get; set; }

        public string RulesetName { get; set; }
        public int DefaultRoundLength { get; set; }
        public string DefaultMatchTitle { get; set; }

        public bool UseCompactOverlayLayout { get; set; }
        public OverlayStatsDisplayType OverlayStatsDisplayType { get; set; }

        public ShowStatusPanelScoresSelectOptions ShowOverlayStatusPanelScoresSelection { get; set; }
        public bool? ShowOverlayStatusPanelScores => ConvertToNullableBool(ShowOverlayStatusPanelScoresSelection);


        private readonly bool _useCompactOverlayLayoutDefault = false;
        private readonly OverlayStatsDisplayType _overlayStatsDisplayTypeDefault = OverlayStatsDisplayType.InfantryScores;
        private readonly ShowStatusPanelScoresSelectOptions _showOverlayStatusPanelScoresSelectionDefault = ShowStatusPanelScoresSelectOptions.UseStatsDisplayDefault;


        public RulesetSettingsForm(Ruleset ruleset)
        {
            RulesetId = ruleset.Id;
            RulesetName = ruleset.Name;

            DefaultRoundLength = ruleset.DefaultRoundLength;
            DefaultMatchTitle = ruleset.DefaultMatchTitle;

            if (ruleset.RulesetOverlayConfiguration != null)
            {
                UseCompactOverlayLayout = ruleset.RulesetOverlayConfiguration.UseCompactLayout;
                OverlayStatsDisplayType = ruleset.RulesetOverlayConfiguration.StatsDisplayType;
                ShowOverlayStatusPanelScoresSelection = ConvertToSelectOption(ruleset.RulesetOverlayConfiguration.ShowStatusPanelScores);
            }
            else
            {
                UseCompactOverlayLayout = _useCompactOverlayLayoutDefault;
                OverlayStatsDisplayType = _overlayStatsDisplayTypeDefault;
                ShowOverlayStatusPanelScoresSelection = _showOverlayStatusPanelScoresSelectionDefault;
            }
        }

        public void SetProperties(Ruleset ruleset)
        {
            RulesetId = ruleset.Id;
            RulesetName = ruleset.Name;

            DefaultRoundLength = ruleset.DefaultRoundLength;
            DefaultMatchTitle = ruleset.DefaultMatchTitle;

            if (ruleset.RulesetOverlayConfiguration != null)
            {
                UseCompactOverlayLayout = ruleset.RulesetOverlayConfiguration.UseCompactLayout;
                OverlayStatsDisplayType = ruleset.RulesetOverlayConfiguration.StatsDisplayType;
                ShowOverlayStatusPanelScoresSelection = ConvertToSelectOption(ruleset.RulesetOverlayConfiguration.ShowStatusPanelScores);
            }
            else
            {
                UseCompactOverlayLayout = _useCompactOverlayLayoutDefault;
                OverlayStatsDisplayType = _overlayStatsDisplayTypeDefault;
                ShowOverlayStatusPanelScoresSelection = _showOverlayStatusPanelScoresSelectionDefault;
            }
        }

        public Ruleset GetRuleset()
        {
            return new Ruleset
            {
                Id = RulesetId,
                Name = RulesetName,
                DefaultRoundLength = DefaultRoundLength,
                DefaultMatchTitle = DefaultMatchTitle
            };
        }

        public RulesetOverlayConfiguration GetOverlayConfiguration()
        {
            return new RulesetOverlayConfiguration
            {
                RulesetId = RulesetId,
                UseCompactLayout = UseCompactOverlayLayout,
                StatsDisplayType = OverlayStatsDisplayType,
                ShowStatusPanelScores = ShowOverlayStatusPanelScores
            };
        }

        private ShowStatusPanelScoresSelectOptions ConvertToSelectOption(bool? showStatusPanelScore)
        {
            switch (showStatusPanelScore)
            {
                case true:
                    return ShowStatusPanelScoresSelectOptions.Yes;

                case false:
                    return ShowStatusPanelScoresSelectOptions.No;

                case null:
                    return ShowStatusPanelScoresSelectOptions.UseStatsDisplayDefault;
            }
        }

        private bool? ConvertToNullableBool(ShowStatusPanelScoresSelectOptions showOverlayStatusPanelScoresSelection)
        {
            switch (showOverlayStatusPanelScoresSelection)
            {
                case ShowStatusPanelScoresSelectOptions.Yes:
                    return true;

                case ShowStatusPanelScoresSelectOptions.No:
                    return false;

                case ShowStatusPanelScoresSelectOptions.UseStatsDisplayDefault:
                    return null;

                default:
                    return null;
            }
        }
    }


    public enum ShowStatusPanelScoresSelectOptions
    {
        Yes,
        No,
        UseStatsDisplayDefault
    }
}
