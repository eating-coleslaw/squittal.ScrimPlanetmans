using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services
{
    public class PlanetsideDataService
    {
        private readonly IOutfitService _outfits;
        private readonly ICharacterService _characters;

        public PlanetsideDataService(IOutfitService outfits, ICharacterService characters)
        {
            _outfits = outfits;
            _characters = characters;
        }

        public async Task<Outfit> GetOutfitByAlias(string alias)
        {
            return await _outfits.GetOutfitByAlias(alias);
        }

        public async Task<Character> GetCharacterById(string id)
        {
            return await _characters.GetCharacterAsync(id);
        }

        public async Task<OutfitMember> GetCharacterOutfit(string id)
        {
            return await _characters.GetCharacterOutfitAsync(id);
        }

        public async Task<IEnumerable<Character>> GetOutfitMembersByAlias(string alias)
        {
            return await _outfits.GetOutfitMembersByAlias(alias);
        }
    }
}
