using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimMatchTeamResult
    {
        [Required]
        public string ScrimMatchId { get; set; }
        [Required]
        public int TeamOrdinal { get; set; }

        public int Points { get; set; }
        public int NetScore { get; set; }

        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Headshots { get; set; }
        public int HeadshotDeaths { get; set; }
        public int Suicides { get; set; }
        public int Teamkills { get; set; }
        public int TeamkillDeaths { get; set; }

        public int RevivesGiven { get; set; }
        public int RevivesTaken { get; set; }

        public int DamageAssists { get; set; }
        public int UtilityAssists { get; set; }

        public int DamageAssistedDeaths { get; set; }
        public int UtilityAssistedDeaths { get; set; }

        public int ObjectiveCaptureTicks { get; set; }
        public int ObjectiveDefenseTicks { get; set; }

        public int BaseDefenses { get; set; }
        public int BaseCaptures { get; set; }

        public ICollection<ScrimMatchTeamPointAdjustment> PointAdjustments { get; set; }
    }
}
