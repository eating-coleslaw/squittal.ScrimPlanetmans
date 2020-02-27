using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimObjectiveTickActionEventEventArgs : EventArgs
    {
        public ScrimObjectiveTickActionEventEventArgs(ScrimObjectiveTickActionEventMessage m)
        {
            Message = m;
        }

        public ScrimObjectiveTickActionEventMessage Message { get; }
    }
}
