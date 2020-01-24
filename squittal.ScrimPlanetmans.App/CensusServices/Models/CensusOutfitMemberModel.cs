using System;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusOutfitMemberModel
    {
        public string CharacterId { get; set; }
        public string OutfitId { get; set; }
        public DateTime MemberSinceDate { get; set; }
        public string Rank { get; set; }
        public int RankOrdinal { get; set; }
    }
}
