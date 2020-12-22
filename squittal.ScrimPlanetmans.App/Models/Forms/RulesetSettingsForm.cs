using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public RulesetSettingsForm(Ruleset ruleset)
        {
            RulesetId = ruleset.Id;
            RulesetName = ruleset.Name;

            DefaultRoundLength = ruleset.DefaultRoundLength;
            DefaultMatchTitle = ruleset.DefaultMatchTitle;

            UseCompactOverlayLayout = ruleset.RulesetOverlayConfiguration.UseCompactLayout;
            OverlayStatsDisplayType = ruleset.RulesetOverlayConfiguration.StatsDisplayType;
            ShowOverlayStatusPanelScoresSelection = ConvertToSelectOption(ruleset.RulesetOverlayConfiguration.ShowStatusPanelScores);
        }

        public void SetProperties(Ruleset ruleset)
        {
            RulesetId = ruleset.Id;
            RulesetName = ruleset.Name;

            DefaultRoundLength = ruleset.DefaultRoundLength;
            DefaultMatchTitle = ruleset.DefaultMatchTitle;

            UseCompactOverlayLayout = ruleset.RulesetOverlayConfiguration.UseCompactLayout;
            OverlayStatsDisplayType = ruleset.RulesetOverlayConfiguration.StatsDisplayType;
            ShowOverlayStatusPanelScoresSelection = ConvertToSelectOption(ruleset.RulesetOverlayConfiguration.ShowStatusPanelScores);
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
                ShowStatusPanelScores = ShowOverlayStatusPanelScores //ConvertToNullableBool(ShowOverlayStatusPanelScoresSelection)
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
