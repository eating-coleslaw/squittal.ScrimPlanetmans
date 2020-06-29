using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimKillfeedEventEventArgs : EventArgs
    {
        public ScrimKillfeedEventEventArgs(ScrimKillfeedEventMessage m)
        {
            Message = m;
        }

        public ScrimKillfeedEventMessage Message { get; }
    }
}
