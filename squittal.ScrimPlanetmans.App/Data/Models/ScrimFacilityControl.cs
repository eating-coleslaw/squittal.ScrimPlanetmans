using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimFacilityControl
    {
        [Required]
        public string ScrimMatchId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
        
        [Required]
        public int FacilityId { get; set; }
        
        [Required]
        public int ControllingTeamOrdinal { get; set; }

        [Required]
        public int ScrimMatchRound { get; set; }

        public ScrimActionType ActionType { get; set; }
        public FacilityControlType ControlType { get; set; }

        //public string FacilityName { get; set; }

        public int ControllingFactionId { get; set; }

        public int? ZoneId { get; set; }
        public int WorldId { get; set; }

        public int Points { get; set; }

        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        public Faction ControllingFaction { get; set; }
        public Zone Zone { get; set; }
        public World World { get; set; }
        #endregion Navigation Properties
    }
}
