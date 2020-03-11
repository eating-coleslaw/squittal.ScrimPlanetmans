using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class MatchConfigurationUpdateEventArgs : EventArgs
    {
        public MatchConfigurationUpdateEventArgs(MatchConfigurationUpdateMessage m)
        {
            Message = m;
        }

        public MatchConfigurationUpdateMessage Message { get; }
    }
}
