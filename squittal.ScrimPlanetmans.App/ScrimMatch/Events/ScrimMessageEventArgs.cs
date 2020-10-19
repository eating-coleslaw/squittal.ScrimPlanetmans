using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimMessageEventArgs<T> : EventArgs
    {
        public ScrimMessageEventArgs(T m)
        {
            Message = m;
        }

        public T Message { get; set; }

    }
}
