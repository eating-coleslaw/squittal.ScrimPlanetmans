using squittal.ScrimPlanetmans.Models.MessageLogs;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class SimpleMessageEventArgs : EventArgs
    {
        public SimpleMessageEventArgs(string s)
        {
            Message = s;

            Timestamp = DateTime.UtcNow;

            LogLevel = ScrimMessageLogLevel.EngineInformation;
        }

        public SimpleMessageEventArgs(string s, ScrimMessageLogLevel logLevel)
        {
            Message = s;

            Timestamp = DateTime.UtcNow;

            LogLevel = logLevel;
        }

        public string Message { get; }
        public DateTime Timestamp { get; }

        public ScrimMessageLogLevel LogLevel { get; }
    }
}
