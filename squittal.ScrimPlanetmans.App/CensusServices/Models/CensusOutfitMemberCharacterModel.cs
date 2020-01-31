using static squittal.ScrimPlanetmans.CensusServices.Models.CensusCharacterModel;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusOutfitMemberCharacterModel : CensusOutfitMemberModel
    {
        public CharacterName Name { get; set; }
        public int OnlineStatus { get; set; }
    }
}
