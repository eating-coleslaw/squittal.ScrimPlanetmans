using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimSpotAssist
    {
        [Required]
        public string ScrimMatchId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string SpotterCharacterId { get; set; }

        [Required]
        public string VictimCharacterId { get; set; }

        [Required]
        public int ScrimMatchRound { get; set; }

        public ScrimActionType ActionType { get; set; }

        // Technically, different teams can have players from the same faction
        public int SpotterTeamOrdinal { get; set; }
        public int VictimTeamOrdinal { get; set; }

        //public string SpotterNameFull { get; set; }
        //public int SpotterFactionId { get; set; }
        public int? SpotterLoadoutId { get; set; }
        //public string SpotterOutfitId { get; set; }
        //public string SpotterOutfitAlias { get; set; }
        //public bool SpotterIsOutfitless { get; set; }

        // public int SpotterConstructedTeamID { get; set; }
        // public bool IsSpotterFromConstructedTeam { get; set; }

        //public string VictimNameFull { get; set; }
        //public int VictimFactionId { get; set; }
        public int? VictimLoadoutId { get; set; }
        //public string VictimOutfitId { get; set; }
        //public string VictimOutfitAlias { get; set; }
        //public bool VictimIsOutfitless { get; set; }

        // public int VictimConstructedTeamID { get; set; }
        // public bool IsVictimFromConstructedTeam { get; set; }

        public int ExperienceGainId { get; set; }
        public int ExperienceGainAmount { get; set; }

        public int? ZoneId { get; set; }
        public int WorldId { get; set; }

        public int Points { get; set; }
        //public int? SpotterResultingPoints { get; set; }
        //public int? SpotterResultingNetScore { get; set; }
        //public int? VictimResultingPoints { get; set; }
        //public int? VictimResultingNetScore { get; set; }

        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        public ScrimMatchParticipatingPlayer SpotterParticipatingPlayer { get; set; }
        public ScrimMatchParticipatingPlayer VictimParticipatingPlayer { get; set; }
        //public Faction SpotterFaction { get; set; }
        //public Faction VictimFaction { get; set; }
        public Zone Zone { get; set; }
        public World World { get; set; }
        #endregion Navigation Properties
    }
}
