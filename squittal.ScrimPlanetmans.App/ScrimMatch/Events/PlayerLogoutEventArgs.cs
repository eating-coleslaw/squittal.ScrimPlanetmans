using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
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
