using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimPeriodicControlTick
    {
        [Required]
        public string ScrimMatchId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public int ScrimMatchRound { get; set; }

        [Required]
        public int TeamOrdinal { get; set; }

        [Required]
        public int Points { get; set; }

        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        #endregion Navigation Properties

    }
}
