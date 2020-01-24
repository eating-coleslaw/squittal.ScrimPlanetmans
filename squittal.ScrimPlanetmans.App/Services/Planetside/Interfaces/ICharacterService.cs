using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface ICharacterService
    {
        Task<Character> GetCharacterAsync(string characterId);
        Task<OutfitMember> GetCharactersOutfitAsync(string characterId);
        //Task<Character> GetOrAddCharacterAsync(string characterId);
        //Task<string> GetCharacterNameFromIdAsync(string characterId);
        //Task RefreshCharacterStore(string characterId)
    }
}
