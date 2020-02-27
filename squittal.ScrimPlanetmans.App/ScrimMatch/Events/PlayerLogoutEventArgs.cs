using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
{
    public class PlayerLogoutEventArgs : EventArgs
    {
        public PlayerLogoutEventArgs(PlayerLogoutMessage m)
        {
            Message = m;
        }

        public PlayerLogoutMessage Message { get; }
    }
}
