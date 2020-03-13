using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimFacilityControlActionEventEventArgs : EventArgs
    {
        public ScrimFacilityControlActionEventEventArgs(ScrimFacilityControlActionEventMessage m)
        {
            Message = m;
        }

        public ScrimFacilityControlActionEventMessage Message { get; }
    }
}
