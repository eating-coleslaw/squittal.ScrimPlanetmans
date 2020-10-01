using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IItemService : ILocallyBackedCensusStore
    {
        Task<IEnumerable<Item>> GetAllWeaponItemsAsync();
        Task<Item> GetItemAsync(int itemId);
        Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId);
        Task<Item> GetWeaponItemAsync(int id);

        Task SetUpItemsMapAsync();
        Task SetUpWeaponsMapAsync();
    }
}
