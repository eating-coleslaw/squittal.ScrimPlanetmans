using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimVehicleDestructionActionEventEventArgs : EventArgs
    {
        public ScrimVehicleDestructionActionEventEventArgs(ScrimVehicleDestructionActionEventMessage m)
        {
            Message = m;
        }

        public ScrimVehicleDestructionActionEventMessage Message { get; }
    }
}
