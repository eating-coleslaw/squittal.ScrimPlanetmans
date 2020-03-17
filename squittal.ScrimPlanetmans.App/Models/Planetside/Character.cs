using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Models.Planetside
{
    public class Character
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public bool IsOnline { get; set; }

        public string OutfitId { get; set; } = string.Empty;
        public string OutfitAlias { get; set; } = string.Empty;
        public string OutfitAliasLower { get; set; } = string.Empty;

        public int FactionId { get; set; }
        public int TitleId { get; set; }
        public int WorldId { get; set; }
        public int BattleRank { get; set; }
        public int BattleRankPercentToNext { get; set; }
        public int CertsEarned { get; set; }
        public int PrestigeLevel { get; set; }

        public World World { get; set; }
        public Faction Faction { get; set; }

        public OutfitMember OutfitMember { get; set; }
    }
}
