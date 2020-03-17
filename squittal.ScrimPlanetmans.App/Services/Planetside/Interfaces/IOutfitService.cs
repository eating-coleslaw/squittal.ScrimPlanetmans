using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IOutfitService
    {
        Task<Outfit> GetOutfitAsync(string outfitId);
        Task<Outfit> GetOutfitByAlias(string alias);
        Task<IEnumerable<Character>> GetOutfitMembersByAlias(string alias);
        Task<OutfitMember> UpdateCharacterOutfitMembership(Character character);

    }
}
