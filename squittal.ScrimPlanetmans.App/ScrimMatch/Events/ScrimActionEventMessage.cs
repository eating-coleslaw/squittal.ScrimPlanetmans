using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimActionEventMessage
    {
        public string Info { get; set; }

        public string GetEnumValueName(ScrimActionType action)
        {
            return Enum.GetName(typeof(ScrimActionType), action);
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
