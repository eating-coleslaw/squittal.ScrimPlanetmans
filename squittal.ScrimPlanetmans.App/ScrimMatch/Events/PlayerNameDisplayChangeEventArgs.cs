using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class PlayerNameDisplayChangeEventArgs : EventArgs
    {
        public PlayerNameDisplayChangeEventArgs(PlayerNameDisplayChangeMessage m)
        {
            Message = m;
        }

        public PlayerNameDisplayChangeMessage Message { get; }
    }
}
