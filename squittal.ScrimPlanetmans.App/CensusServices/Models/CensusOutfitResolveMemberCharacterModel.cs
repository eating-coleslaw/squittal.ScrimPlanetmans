using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusOutfitResolveMemberCharacterModel : CensusOutfitModel
    {
        public IEnumerable<CensusOutfitMemberCharacterModel> Members { get; set; }
    }
}
