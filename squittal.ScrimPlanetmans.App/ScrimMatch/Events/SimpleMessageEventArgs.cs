using squittal.ScrimPlanetmans.Models.MessageLogs;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class SimpleMessageEventArgs : EventArgs
    {
        public SimpleMessageEventArgs(string s)
        {
            Message = s;

            CreatedTime = DateTime.UtcNow;

            LogLevel = ScrimMessageLogLevel.EngineInformation;
        }

        public SimpleMessageEventArgs(string s, ScrimMessageLogLevel logLevel)
        {
            Message = s;

            CreatedTime = DateTime.UtcNow;

            LogLevel = logLevel;
        }

        public string Message { get; }
        public DateTime CreatedTime { get; }

        public ScrimMessageLogLevel LogLevel { get; }
    }
}
