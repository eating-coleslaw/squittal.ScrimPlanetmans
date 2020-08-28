using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamLockStatusChangeEventArgs : EventArgs
    {
        public TeamLockStatusChangeEventArgs(TeamLockStatusChangeMessage m)
        {
            Message = m;
        }

        public TeamLockStatusChangeMessage Message { get; }
    }
}
