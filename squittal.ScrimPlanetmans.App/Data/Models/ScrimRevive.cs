using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimRevive
    {
        [Required]
        public string ScrimMatchId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string MedicCharacterId { get; set; }

        [Required]
        public string RevivedCharacterId { get; set; }

        [Required]
        public int ScrimMatchRound { get; set; }

        public ScrimActionType ActionType { get; set; }

        // Technically, different teams can have players from the same faction
        public int MedicTeamOrdinal { get; set; }
        public int RevivedTeamOrdinal { get; set; }

        public int? MedicLoadoutId { get; set; }
        public int? RevivedLoadoutId { get; set; }

        public int ExperienceGainId { get; set; }
        public int ExperienceGainAmount { get; set; }

        public int? ZoneId { get; set; }
        public int WorldId { get; set; }

        public int Points { get; set; }

        public ScrimActionType EnemyActionType {get; set;}
        public int EnemyPoints { get; set; }
        public string LastKilledByCharacterId { get; set; }

        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        public ScrimMatchParticipatingPlayer MedicParticipatingPlayer { get; set; }
        public ScrimMatchParticipatingPlayer RevivedParticipatingPlayer { get; set; }
        public ScrimMatchParticipatingPlayer LastKilledByParticipatingPlayer { get; set; }
        public Zone Zone { get; set; }
        public World World { get; set; }
        #endregion Navigation Properties
    }
}
