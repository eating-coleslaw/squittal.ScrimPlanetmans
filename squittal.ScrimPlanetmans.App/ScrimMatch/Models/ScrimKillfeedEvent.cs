using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ScrimKillfeedEvent
    {
        public Player ActingPlayer { get; set; }
        public Player RecipientPlayer { get; set; }

        public DateTime Timestamp { get; set; }

        public KillfeedEventType EventType { get; set; }
        public string WeaponName { get; set; } = "Unknown Weapon";

        public int Points { get; set; } = 0;
        public string PointsDisplay => GetPointsDisplay();

        public bool? IsHeadshot { get; set; }


        public DateTime FirstRenderTime { get; set; }
        public DateTime PreviousRenderTime { get; set; }
        public bool IsExpired { get; set; } = false;

        private string GetPointsDisplay()
        {
            if (Points >= 0)
            {
                return $"+{Points}";
            }
            else
            {
                return Points.ToString();
            }
        }

    }
}
