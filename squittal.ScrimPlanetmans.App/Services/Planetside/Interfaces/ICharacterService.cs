using squittal.ScrimPlanetmans.Models.Planetside;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface ICharacterService
    {
        Task<Character> GetCharacterAsync(string characterId);
        Task<Character> GetCharacterByNameAsync(string characterName);
        Task<OutfitMember> GetCharacterOutfitAsync(string characterId);
    }
}
