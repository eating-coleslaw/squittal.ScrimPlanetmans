using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IOutfitService
    {
        Task<Outfit> GetOutfitAsync(string outfitId);
        Task<Outfit> GetOutfitByAlias(string alias);
        Task<OutfitMember> UpdateCharacterOutfitMembership(Character character);

    }
}
