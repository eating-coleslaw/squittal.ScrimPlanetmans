using Microsoft.AspNetCore.Components;
using System;

namespace squittal.ScrimPlanetmans.Models.MessageLogs
{
    public class TimestampedLogMessage
    {
        public DateTime Timestamp { get; set; }
        public MarkupString Message { get; set; }

        public TimestampedLogMessage(DateTime timestamp, MarkupString message)
        {
            Timestamp = timestamp;
            Message = message;
        }
    }
}
