using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IItemCategoryService : ILocallyBackedCensusStore
    {
        Task<IEnumerable<int>> GetItemCategoryIdsAsync();
        IEnumerable<int> GetNonWeaponItemCateogryIds();
        IEnumerable<ItemCategory> GetWeaponItemCategories();
        ItemCategory GetWeaponItemCategory(int itemCategoryId);
        Task<ItemCategory> GetWeaponItemCategoryAsync(int itemCategoryId);
        Task<IEnumerable<int>> GetWeaponItemCategoryIdsAsync();
        Task SetUpWeaponCategoriesListAsync();
    }
}