using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamFactionChangeEventArgs : EventArgs
    {
        public TeamFactionChangeEventArgs(TeamFactionChangeMessage m)
        {
            Message = m;
        }

        public TeamFactionChangeMessage Message { get; }
    }
}
