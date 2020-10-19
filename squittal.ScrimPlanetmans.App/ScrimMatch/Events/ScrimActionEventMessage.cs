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
        public ScrimMessageLogLevel LogLevel { get; set; }

        public string GetEnumValueName(ScrimActionType action)
        {
            return Enum.GetName(typeof(ScrimActionType), action);
        }

        public string GetLogLevelEnumValueName(ScrimMessageLogLevel logLevel)
        {
            return Enum.GetName(typeof(ScrimMessageLogLevel), logLevel);
        }

        public MarkupString GetLogLevelInfoMarkupString()
        {
            return LogLevel switch
            {
                ScrimMessageLogLevel.EngineError => GetWarningMarkupString(Info),
                ScrimMessageLogLevel.EngineInformation => GetMutedMarkupString(Info),
                ScrimMessageLogLevel.EngineWarning => GetWarningMarkupString(Info),
                ScrimMessageLogLevel.MatchEventInformation => (MarkupString)Info,
                ScrimMessageLogLevel.MatchEventMinor => GetMutedMarkupString(Info),
                ScrimMessageLogLevel.MatchEventMajor => (MarkupString)Info,
                ScrimMessageLogLevel.MatchEventRuleBreak => GetErrorMarkupString(Info),
                ScrimMessageLogLevel.MatchEventWarning => GetWarningMarkupString(Info),
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
                return $"+{points}";
            }
            else
            {
                return $"{points}";
            }
        }
    }
}
