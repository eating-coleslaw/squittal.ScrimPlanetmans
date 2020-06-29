using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ConstructedTeamMemberChangeEventArgs : EventArgs
    {
        public ConstructedTeamMemberChangeEventArgs(ConstructedTeamMemberChangeMessage m)
        {
            Message = m;
        }

        public ConstructedTeamMemberChangeMessage Message { get; }
    }
}
