using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamAliasChangeEventArgs : EventArgs
    {
        public TeamAliasChangeEventArgs(TeamAliasChangeMessage m)
        {
            Message = m;
        }

        public TeamAliasChangeMessage Message { get; }
    }
}
