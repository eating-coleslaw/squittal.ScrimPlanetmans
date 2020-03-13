using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamStatUpdateEventArgs : EventArgs
    {
        public TeamStatUpdateEventArgs(TeamStatUpdateMessage m)
        {
            Message = m;
        }

        public TeamStatUpdateMessage Message { get; }
    }
}
