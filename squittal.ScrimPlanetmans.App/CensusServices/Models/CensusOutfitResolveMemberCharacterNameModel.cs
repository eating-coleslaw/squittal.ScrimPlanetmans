using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusOutfitResolveMemberCharacterNameModel : CensusOutfitModel
    {
        public IEnumerable<CensusOutfitMemberCharacterNameModel> Members { get; set; }
    }
}
