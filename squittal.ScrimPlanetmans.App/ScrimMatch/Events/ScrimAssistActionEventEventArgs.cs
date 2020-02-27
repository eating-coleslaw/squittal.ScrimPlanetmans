using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
{
    public class ScrimAssistActionEventEventArgs : EventArgs
    {
        public ScrimAssistActionEventEventArgs(ScrimAssistActionEventMessage m)
        {
            Message = m;
        }

        public ScrimAssistActionEventMessage Message { get; }
    }
}
