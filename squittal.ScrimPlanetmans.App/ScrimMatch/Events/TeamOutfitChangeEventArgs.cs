using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamOutfitChangeEventArgs : EventArgs
    {
        public TeamOutfitChangeEventArgs(TeamOutfitChangeMessage m)
        {
            Message = m;
        }

        public TeamOutfitChangeMessage Message { get; }
    }
}
