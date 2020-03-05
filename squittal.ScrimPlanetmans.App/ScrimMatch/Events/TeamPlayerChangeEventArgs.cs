using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamPlayerChangeEventArgs : EventArgs
    {
        public TeamPlayerChangeEventArgs(TeamPlayerChangeMessage m)
        {
            Message = m;
        }

        public TeamPlayerChangeMessage Message { get; }
    }
}
