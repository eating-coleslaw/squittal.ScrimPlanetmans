using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IItemService : ICountableStoreService, IUpdateable
    {
        Task<Item> GetItem(int itemId);
        Task<IEnumerable<int>> GetItemCategoryIdsAsync();
        Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId);
        IEnumerable<int> GetNonWeaponItemCateogryIds();
        Item GetWeaponFromItemId(int id);
        //Task RefreshStore();
        Task SetUpItemsListAsync();
        Task SetUpWeaponsListAsnyc();
    }
}
