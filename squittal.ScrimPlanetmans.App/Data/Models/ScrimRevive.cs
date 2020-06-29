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

        //public string MedicNameFull { get; set; }
        //public int MedicFactionId { get; set; }
        public int? MedicLoadoutId { get; set; }
        //public string MedicOutfitId { get; set; }
        //public string MedicOutfitAlias { get; set; }
        //public bool MedicIsOutfitless { get; set; }

        // public int MedicConstructedTeamID { get; set; }
        // public bool IsMedicFromConstructedTeam { get; set; }

        //public string RevivedNameFull { get; set; }
        //public int RevivedFactionId { get; set; }
        public int? RevivedLoadoutId { get; set; }
        //public string RevivedOutfitId { get; set; }
        //public string RevivedOutfitAlias { get; set; }
        //public bool RevivedIsOutfitless { get; set; }

        // public int RevivedConstructedTeamID { get; set; }
        // public bool IsRevivedFromConstructedTeam { get; set; }

        public int ExperienceGainId { get; set; }
        public int ExperienceGainAmount { get; set; }

        public int? ZoneId { get; set; }
        public int WorldId { get; set; }

        public int Points { get; set; }
        //public int? MedicResultingPoints { get; set; }
        //public int? MedicResultingNetScore { get; set; }
        //public int? RevivedResultingPoints { get; set; }
        //public int? RevivedResultingNetScore { get; set; }

        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        public ScrimMatchParticipatingPlayer MedicParticipatingPlayer { get; set; }
        public ScrimMatchParticipatingPlayer RevivedParticipatingPlayer { get; set; }
        //public Faction MedicFaction { get; set; }
        //public Faction RevivedFaction { get; set; }
        public Zone Zone { get; set; }
        public World World { get; set; }
        #endregion Navigation Properties
    }
}
