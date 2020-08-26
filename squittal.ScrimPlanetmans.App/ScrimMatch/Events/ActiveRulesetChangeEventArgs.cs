using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ActiveRulesetChangeEventArgs : EventArgs
    {
        public ActiveRulesetChangeEventArgs(ActiveRulesetChangeMessage m)
        {
            Message = m;
        }

        public ActiveRulesetChangeMessage Message { get; }
    }
}
