using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class MatchTimerTickEventArgs : EventArgs
    {
        public MatchTimerTickEventArgs(MatchTimerTickMessage m)
        {
            Message = m;
        }

        public MatchTimerTickMessage Message { get; }
    }
}
