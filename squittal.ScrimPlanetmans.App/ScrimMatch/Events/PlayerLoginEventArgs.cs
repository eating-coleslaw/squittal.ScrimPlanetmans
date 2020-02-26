using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class PlayerLoginEventArgs : EventArgs
    {
        public PlayerLoginEventArgs(PlayerLoginMessage m)
        {
            Message = m;
        }

        public PlayerLoginMessage Message { get; }
    }
}
