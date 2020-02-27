using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
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
