using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimMatchRoundConfiguration
    {
        [Required]
        public string ScrimMatchId { get; set; }

        [Required]
        public int ScrimMatchRound { get; set; }

        public string Title { get; set; }

        public int RoundSecondsTotal { get; set; }

        public int WorldId { get; set; }
        public bool IsManualWorldId { get; set; } // = false;

        public int? FacilityId { get; set; } // = -1
        public bool IsRoundEndedOnFacilityCapture { get; set; } // = false; // TODO from MatchConfiguration: move this setting to the Ruleset model

        public int? TargetPointValue { get; set; } //= 200;
        public int? InitialPoints { get; set; } //= 0;

        public int? PeriodicFacilityControlPoints { get; set; } //= 5;
        public int? PeriodicFacilityControlInterval { get; set; } //= 15;

        public bool? EnableRoundTimeLimit { get; set; } // true
        public TimerDirection? RoundTimerDirection { get; set; }  // null
        public bool? EndRoundOnPointValueReached { get; set; } // false
        public MatchWinCondition MatchWinCondition { get; set; } //MatchWinCondition.MostPoints
        public RoundWinCondition RoundWinCondition { get; set; } //RoundWinCondition.NotApplicable
        public bool? EnablePeriodicFacilityControlRewards { get; set; } //false;


        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        public World World { get; set; }
        #endregion Navigation Properties
    }
}
