using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
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
