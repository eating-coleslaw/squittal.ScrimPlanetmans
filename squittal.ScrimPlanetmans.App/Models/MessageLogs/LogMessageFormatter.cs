using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Models.MessageLogs
{
    public static class LogMessageFormatter
    {
        public static string Error(string header, string body)
        {
            return $"<span style=\"color: red;\"><span style=\"font-weight: 600;\">{header}</span>. {body}</span>";
        }
    }
}
