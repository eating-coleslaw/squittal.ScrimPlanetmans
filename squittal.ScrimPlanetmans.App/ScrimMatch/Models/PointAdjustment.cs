using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class PointAdjustment
    {
        public int Points { get; set; }

        public PointAdjustmentType AdjustmentType
        {
            get
            {
                return Points >= 0 ? PointAdjustmentType.Bonus : PointAdjustmentType.Penalty;
            }
        }

        public string Rationale { get; set; }

        public DateTime Timestamp { get; set; }
    }

    public enum PointAdjustmentType
    {
        Penalty,
        Bonus
    }
}
