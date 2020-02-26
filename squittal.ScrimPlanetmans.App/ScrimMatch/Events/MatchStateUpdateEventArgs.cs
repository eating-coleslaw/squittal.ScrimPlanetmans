using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class MatchStateUpdateEventArgs : EventArgs
    {
        public MatchStateUpdateEventArgs(string s)
        {
            Message = s;
        }

        public string Message { get; }
    }
}
