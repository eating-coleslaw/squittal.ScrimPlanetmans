using squittal.ScrimPlanetmans.Models.Planetside;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimMatchParticipatingPlayer
    {
        [Required]
        public string ScrimMatchId { get; set; }

        [Required]
        public string CharacterId { get; set; }

        [Required]
        public int TeamOrdinal { get; set; }

        
        public string NameFull { get; set; }
        public string NameDisplay { get; set; }


        public int FactionId { get; set; }
        public int WorldId { get; set; }
        public int PrestigeLevel { get; set; }

        public bool IsFromOutfit { get; set; }
        public string OutfitId { get; set; }
        public string OutfitAlias { get; set; }

        public bool IsFromConstructedTeam { get; set; }
        public int? ConstructedTeamId { get; set; }


        #region Navigation Properties
        public ScrimMatch ScrimMatch { get; set; }
        public Faction Faction { get; set; }
        public World World { get; set; }
        public ConstructedTeam ConstructedTeam { get; set; }
        #endregion Navigation Properties
    }
}
