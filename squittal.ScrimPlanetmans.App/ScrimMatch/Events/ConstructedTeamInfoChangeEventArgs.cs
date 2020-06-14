using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ConstructedTeamInfoChangeEventArgs : EventArgs
    {
        public ConstructedTeamInfoChangeEventArgs(ConstructedTeamInfoChangeMessage m)
        {
            Message = m;
        }

        public ConstructedTeamInfoChangeMessage Message { get; }
    }
}
