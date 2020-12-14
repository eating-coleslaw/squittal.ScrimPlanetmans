using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ActiveRulesetChangeMessage
    {
        public Ruleset ActiveRuleset { get; set; }
        public Ruleset PreviousActiveRuleset { get; set; }

        public string Info { get; set; }

        public ActiveRulesetChangeMessage(Ruleset activeRuleset, Ruleset previousActiveRuleset)
        {
            ActiveRuleset = activeRuleset;
            PreviousActiveRuleset = previousActiveRuleset;

            var newNameDisplay = !string.IsNullOrWhiteSpace(ActiveRuleset.Name) ? ActiveRuleset.Name : "null";
            var oldNameDisplay = !string.IsNullOrWhiteSpace(PreviousActiveRuleset?.Name) ? PreviousActiveRuleset.Name : "null";

            Info = $"Active Ruleset Changed: {oldNameDisplay} [{PreviousActiveRuleset?.Id} => {newNameDisplay} [{ActiveRuleset.Id}]";
        }

        public ActiveRulesetChangeMessage(Ruleset activeRuleset)
        {
            ActiveRuleset = activeRuleset;

            var newNameDisplay = !string.IsNullOrWhiteSpace(ActiveRuleset.Name) ? ActiveRuleset.Name : "null";

            Info = $"Active Ruleset Changed: none => {newNameDisplay} [{ActiveRuleset.Id}]";
        }
    }
}
