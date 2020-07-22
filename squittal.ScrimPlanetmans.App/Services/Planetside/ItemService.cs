using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class ItemService : IItemService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly CensusItem _censusItem;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<ItemService> _logger;

        public string BackupSqlScriptFileName => "dbo.Item.Table.sql";

        private List<Item> _items = new List<Item>();
        private List<Item> _weapons = new List<Item>();


        public ItemService(IDbContextHelper dbContextHelper, IItemCategoryService itemCategoryService,
            CensusItem censusItem, ISqlScriptRunner sqlScriptRunner, ILogger<ItemService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _itemCategoryService = itemCategoryService;
            _censusItem = censusItem;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<Item> GetItem(int itemId)
        {
            if (_items == null || _items.Count == 0)
            {
                await SetUpItemsListAsync();
            }

            return _items.FirstOrDefault(i => i.Id == itemId);
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId)
        {
            if (_items == null || _items.Count == 0)
            {
                await SetUpItemsListAsync();
            }

            return _items.Where(i => i.ItemCategoryId == categoryId && i.ItemCategoryId.HasValue)
                         .ToList();
        }

        public async Task SetUpItemsListAsync()
        {
            if (_items == null || _items.Count == 0)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                _items = await dbContext.Items.ToListAsync();
            }
        }

        public Item GetWeaponItem(int id)
        {
            // TODO: handle "Unknown" weapon deaths/kills, like Fatalities
            
            return _weapons.FirstOrDefault(w => w.Id == id);
        }

        public async Task SetUpWeaponsListAsnyc()
        {
            if (_weapons == null || _weapons.Count == 0)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var nonWeaponItemCategoryIds = _itemCategoryService.GetNonWeaponItemCateogryIds();

                _weapons = await dbContext.Items
                                        .Where(i => i.ItemCategoryId.HasValue && !nonWeaponItemCategoryIds.Contains((int)i.ItemCategoryId))
                                        .ToListAsync();
            }
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var anyItems = await dbContext.Items.AnyAsync();
                if (anyItems)
                {
                    //await SetUpItemsListAsync();
                    await SetUpWeaponsListAsnyc();

                    return;
                }
            }

            var success = await RefreshStoreFromCensus();

            if (!success && canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }

            //await SetUpItemsListAsync();
            await SetUpWeaponsListAsnyc();
        }

        public async Task<bool> RefreshStoreFromCensus()
        {
            IEnumerable<CensusItemModel> items = new List<CensusItemModel>();

            try
            {
                items = await _censusItem.GetAllItems();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all Items. Refreshing store from backup...");
                return false;
            }

            if (items != null && items.Any())
            {
                await UpsertRangeAsync(items.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Items store: {items.Count()} entries");

                return true;
            }
            else
            {
                return false;
            }
        }
        
        private async Task UpsertRangeAsync(IEnumerable<Item> censusEntities)
        {
            var createdEntities = new List<Item>();

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var storedEntities = await dbContext.Items.ToListAsync();

                foreach (var censusEntity in censusEntities)
                {
                    var storeEntity = storedEntities.FirstOrDefault(e => e.Id == censusEntity.Id);
                    if (storeEntity == null)
                    {
                        createdEntities.Add(censusEntity);
                    }
                    else
                    {
                        storeEntity = censusEntity;
                        dbContext.Items.Update(storeEntity);
                    }
                }

                if (createdEntities.Any())
                {
                    await dbContext.Items.AddRangeAsync(createdEntities);
                }

                await dbContext.SaveChangesAsync();
            }
        }
        
        private static Item ConvertToDbModel(CensusItemModel item)
        {
            return new Item
            {
                Id = item.ItemId,
                ItemTypeId = item.ItemTypeId,
                ItemCategoryId = item.ItemCategoryId,
                IsVehicleWeapon = item.IsVehicleWeapon,
                Name = item.Name?.English,
                Description = item.Description?.English,
                FactionId = item.FactionId,
                MaxStackSize = item.MaxStackSize,
                ImageId = item.ImageId
            };
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusItem.GetItemsCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Items.CountAsync();
        }

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
