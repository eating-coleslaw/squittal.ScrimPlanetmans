using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IItemService : ICountableStore, ILocallyBackedStore, IUpdateable
    {
        Task<Item> GetItem(int itemId);
        Task<IEnumerable<int>> GetItemCategoryIdsAsync();
        Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId);
        IEnumerable<int> GetNonWeaponItemCateogryIds();
        Item GetWeaponItem(int id);
        IEnumerable<ItemCategory> GetWeaponItemCategories();
        ItemCategory GetWeaponItemCategory(int itemCategoryId);
        IEnumerable<int> GetWeaponItemCategoryIds();
        Task<IEnumerable<int>> GetWeaponItemCategoryIdsAsync();

        //Task RefreshStore();
        Task SetUpItemsListAsync();
        Task SetUpWeaponCategoriesListAsync();
        Task SetUpWeaponsListAsnyc();
    }
}
