using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimMessageEventArgs<T> : EventArgs
    {
        public ScrimMessageEventArgs(T m)
        {
            Message = m;

            CreatedTime = DateTime.UtcNow;
        }

        public T Message { get; set; }
        public DateTime CreatedTime { get; }
    }
}
