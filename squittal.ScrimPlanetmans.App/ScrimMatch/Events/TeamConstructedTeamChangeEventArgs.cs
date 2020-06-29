using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamConstructedTeamChangeEventArgs : EventArgs
    {
        public TeamConstructedTeamChangeEventArgs(TeamConstructedTeamChangeMessage m)
        {
            Message = m;
        }

        public TeamConstructedTeamChangeMessage Message { get; }
    }
}
