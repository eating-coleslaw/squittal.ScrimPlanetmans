using System;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusOutfitModel
    {
        public string OutfitId { get; set; }
        public string Name { get; set; }
        public string NameLower { get; set; }
        public string Alias { get; set; }
        public string AliasLower { get; set; }
        public DateTime TimeCreated { get; set; }
        public string LeaderCharacterId { get; set; }
        public int MemberCount { get; set; }
    }
}
