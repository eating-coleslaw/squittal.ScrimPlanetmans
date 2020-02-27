using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
{
    public class ScrimReviveActionEventEventArgs : EventArgs
    {
        public ScrimReviveActionEventEventArgs(ScrimReviveActionEventMessage m)
        {
            Message = m;
        }

        public ScrimReviveActionEventMessage Message { get; }
    }
}
