using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class SimpleMessageEventArgs : EventArgs
    {
        public SimpleMessageEventArgs(string s)
        {
            Message = s;
        }

        public string Message { get; }
    }
}
