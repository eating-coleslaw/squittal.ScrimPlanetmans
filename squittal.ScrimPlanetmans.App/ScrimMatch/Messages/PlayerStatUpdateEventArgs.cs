using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
{
    public class PlayerStatUpdateEventArgs : EventArgs
    {
        public PlayerStatUpdateEventArgs(PlayerStatUpdateMessage m)
        {
            Message = m;
        }

        public PlayerStatUpdateMessage Message { get; }
    }
}
