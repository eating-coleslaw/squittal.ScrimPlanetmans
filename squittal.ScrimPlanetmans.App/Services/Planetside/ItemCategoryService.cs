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
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<ItemCategoryService> _logger;

        public string BackupSqlScriptFileName => "dbo.ItemCategory.Table.sql";


        public ItemCategoryService(IDbContextHelper dbContextHelper, CensusItemCategory censusItemCategory, ISqlScriptRunner sqlScriptRunner, ILogger<ItemCategoryService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusItemCategory = censusItemCategory;
            _sqlScriptRunner = sqlScriptRunner;
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

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
