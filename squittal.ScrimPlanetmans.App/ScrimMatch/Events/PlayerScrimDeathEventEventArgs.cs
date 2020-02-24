using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
{
    public class PlayerScrimDeathEventEventArgs : EventArgs
    {
        public PlayerScrimDeathEventEventArgs(PlayerScrimDeathEventMessage m)
        {
            Message = m;
        }

        public PlayerScrimDeathEventMessage Message { get; }
    }
}
