using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IItemService
    {
        Task<Item> GetItem(int itemId);
        Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId);
        Task RefreshStore();
    }
}
