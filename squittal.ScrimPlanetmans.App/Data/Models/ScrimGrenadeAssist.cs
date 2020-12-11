using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimGrenadeAssist
    {
        [Required]
        public string ScrimMatchId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string AttackerCharacterId { get; set; }

        [Required]
        public string VictimCharacterId { get; set; }

        [Required]
        public int ScrimMatchRound { get; set; }

        public ScrimActionType ActionType { get; set; }

        // Technically, different teams can have players from the same faction
        public int AttackerTeamOrdinal { get; set; }
        public int VictimTeamOrdinal { get; set; }

        public int? AttackerLoadoutId { get; set; }
        public int? VictimLoadoutId { get; set; }

        public int ExperienceGainId { get; set; }
        public int ExperienceGainAmount { get; set; }

        public int? ZoneId { get; set; }
        public int WorldId { get; set; }

        public int Points { get; set; }

        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        public ScrimMatchParticipatingPlayer AttackerParticipatingPlayer { get; set; }
        public ScrimMatchParticipatingPlayer VictimParticipatingPlayer { get; set; }
        public Zone Zone { get; set; }
        public World World { get; set; }
        #endregion Navigation Properties
    }
}
