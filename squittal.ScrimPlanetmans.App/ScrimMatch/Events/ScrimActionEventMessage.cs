using Microsoft.AspNetCore.Components;
using squittal.ScrimPlanetmans.Models.MessageLogs;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimActionEventMessage
    {
        public string Info { get; set; }
        public DateTime Timestamp { get; set; }
        public EventMessageLogLevel LogLevel { get; set; }

        public string GetEnumValueName(ScrimActionType action)
        {
            return Enum.GetName(typeof(ScrimActionType), action);
        }

        public string GetLogLevelEnumValueName(EventMessageLogLevel logLevel)
        {
            return Enum.GetName(typeof(EventMessageLogLevel), logLevel);
        }

        public MarkupString GetLogLevelInfoMarkupString()
        {
            return LogLevel switch
            {
                EventMessageLogLevel.EngineError => GetWarningMarkupString(Info),
                EventMessageLogLevel.EngineInformation => GetMutedMarkupString(Info),
                EventMessageLogLevel.EngineWarning => GetWarningMarkupString(Info),
                EventMessageLogLevel.MatchEventInformation => (MarkupString)Info,
                EventMessageLogLevel.MatchEventMinor => GetMutedMarkupString(Info),
                EventMessageLogLevel.MatchEventMajor => (MarkupString)Info,
                EventMessageLogLevel.MatchEventRuleBreak => GetErrorMarkupString(Info),
                EventMessageLogLevel.MatchEventWarning => GetWarningMarkupString(Info),
                _ => (MarkupString)Info
            };
        }

        private MarkupString GetMutedMarkupString(string info)
        {
            return (MarkupString)$"<span style=\"color: var(--sq-gray); font-weight: 400;\">{info}</span>";
        }

        private MarkupString GetWarningMarkupString(string info)
        {
            //return (MarkupString)$"<span style=\"color: var(--sq-red); font-weight: 500;\">{info}</span>";
            return (MarkupString)$"<span style=\"color: var(--sq-ov-semantic-red); font-weight: 500;\">{info}</span>";
        }

        private MarkupString GetErrorMarkupString(string info)
        {
            return (MarkupString)$"<span style=\"color: red; font-weight: 700;\">{info}</span>";
        }

        public string GetPointsDisplay(int points)
        {
            if (points >= 0)
            {
                return $"+{points.ToString()}";
            }
            else
            {
                return $"{points.ToString()}";
            }
        }
    }
}
