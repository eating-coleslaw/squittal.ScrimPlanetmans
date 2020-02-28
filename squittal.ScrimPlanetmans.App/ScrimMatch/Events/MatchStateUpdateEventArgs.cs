using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class MatchStateUpdateEventArgs : EventArgs
    {
        public MatchStateUpdateEventArgs(MatchStateUpdateMessage m)
        {
            Message = m;
        }

        public MatchStateUpdateMessage Message { get; }
    }
}
