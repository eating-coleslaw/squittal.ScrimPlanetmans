using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
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
