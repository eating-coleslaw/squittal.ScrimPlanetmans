using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.Data;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class ItemCategoryService : IItemCategoryService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusItemCategory _censusItemCategory;
        private readonly ILogger<ItemCategoryService> _logger;

        public ItemCategoryService(IDbContextHelper dbContextHelper, CensusItemCategory censusItemCategory, ILogger<ItemCategoryService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusItemCategory = censusItemCategory;
            _logger = logger;
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusItemCategory.GetItemCategoriesCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.ItemCategories.CountAsync();
        }
    }
}
