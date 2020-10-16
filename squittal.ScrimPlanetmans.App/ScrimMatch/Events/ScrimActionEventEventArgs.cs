using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimActionEventEventArgs<T> : EventArgs
    {
        public ScrimActionEventEventArgs(T m)
        {
            Message = m;
        }

        public T Message { get; set; }

    }
}
