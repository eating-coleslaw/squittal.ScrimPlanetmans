using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class RulesetRuleChangeEventArgs : EventArgs
    {
        public RulesetRuleChangeEventArgs(RulesetRuleChangeMessage m)
        {
            Message = m;
        }

        public RulesetRuleChangeMessage Message { get; }
    }
}
