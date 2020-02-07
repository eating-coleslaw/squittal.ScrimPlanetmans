using squittal.ScrimPlanetmans.Hubs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
