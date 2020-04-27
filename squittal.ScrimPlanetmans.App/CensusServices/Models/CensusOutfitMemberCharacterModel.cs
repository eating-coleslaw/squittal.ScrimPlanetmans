using static squittal.ScrimPlanetmans.CensusServices.Models.CensusCharacterModel;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusOutfitMemberCharacterModel : CensusOutfitMemberModel
    {
        public CharacterName Name { get; set; }
        //public int OnlineStatus { get; set; }
        public string OnlineStatus { get; set; }
        public int PrestigeLevel { get; set; }

        public string OutfitAlias { get; set; }
        public string OutfitAliasLower { get; set; }
    }
}
