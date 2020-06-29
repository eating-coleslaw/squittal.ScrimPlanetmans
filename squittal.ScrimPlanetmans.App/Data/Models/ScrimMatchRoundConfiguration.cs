using squittal.ScrimPlanetmans.Models.Planetside;
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

        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        public World World { get; set; }
        #endregion Navigation Properties
    }
}
