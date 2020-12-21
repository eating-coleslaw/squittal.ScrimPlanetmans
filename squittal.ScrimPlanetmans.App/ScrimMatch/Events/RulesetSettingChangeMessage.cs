using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class RulesetSettingChangeMessage
    {
        public Ruleset Ruleset { get; set; }
        public List<RulesetSettingChange> ChangedSettings { get; set; } = new List<RulesetSettingChange>();
        public string Info => GetInfoString();

        public RulesetSettingChangeMessage(Ruleset ruleset, Ruleset previousRuleset)
        {
            Ruleset = ruleset;

            CalculateSettingChanges(ruleset, previousRuleset);
        }

        private void CalculateSettingChanges(Ruleset ruleset, Ruleset previousRuleset)
        {
            if (ruleset.DefaultMatchTitle != previousRuleset.DefaultMatchTitle)
            {
                ChangedSettings.Add(RulesetSettingChange.DefaultMatchTitle);
            }

            if (ruleset.DefaultRoundLength != previousRuleset.DefaultRoundLength)
            {
                ChangedSettings.Add(RulesetSettingChange.DefaultRoundLength);
            }

            //if (ruleset.UseCompactOverlay != previousRuleset.UseCompactOverlay)
            //{
            //    ChangedSettings.Add(RulesetSettingChange.UseCompactOverlay);
            //}

            //if (ruleset.OverlayStatsDisplayType != previousRuleset.OverlayStatsDisplayType)
            //{
            //    ChangedSettings.Add(RulesetSettingChange.OverlayStatsDisplayType);
            //}
        }

        private string GetInfoString()
        {
            if (Ruleset == null)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder($"Settings changed for Ruleset {Ruleset.Name} [{Ruleset.Id}]: ");

            if (ChangedSettings == null || !ChangedSettings.Any())
            {
                stringBuilder.Append("none");

                return stringBuilder.ToString();
            }

            var isFirst = true;
            foreach (var setting in ChangedSettings)
            {
                var settingString = isFirst ? GetEnumValueName(setting) : $", {GetEnumValueName(setting)}";

                stringBuilder.Append(settingString);

                isFirst = false;
            }

            return stringBuilder.ToString();
        }

        private string GetEnumValueName(RulesetSettingChange type)
        {
            return Enum.GetName(typeof(RulesetSettingChange), type);
        }
    }

    public enum RulesetSettingChange
    {
        //IsCustomDefault,
        DefaultMatchTitle,
        DefaultRoundLength//,
        //UseCompactOverlay,
        //OverlayStatsDisplayType
    }
}
