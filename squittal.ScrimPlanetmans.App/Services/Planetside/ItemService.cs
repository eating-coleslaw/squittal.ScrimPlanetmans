using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.Planetside;
using System;
using System.Collections.Concurrent;
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

        //private List<Item> _items = new List<Item>();
        //private List<Item> _weapons = new List<Item>();

        private ConcurrentDictionary<int, Item> _itemsMap { get; set; } = new ConcurrentDictionary<int, Item>();
        private ConcurrentDictionary<int, Item> _weaponsMap { get; set; } = new ConcurrentDictionary<int, Item>();


        public ItemService(IDbContextHelper dbContextHelper, IItemCategoryService itemCategoryService,
            CensusItem censusItem, ISqlScriptRunner sqlScriptRunner, ILogger<ItemService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _itemCategoryService = itemCategoryService;
            _censusItem = censusItem;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<Item> GetItemAsync(int itemId)
        {
            if (_itemsMap == null || _itemsMap.Count == 0)
            {
                await SetUpItemsMapAsync();
            }

            _itemsMap.TryGetValue(itemId, out var item);

            return item;
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId)
        {
            if (_itemsMap == null || _itemsMap.Count == 0)
            {
                await SetUpItemsMapAsync();
            }

            return _itemsMap.Values.Where(i => i.ItemCategoryId == categoryId && i.ItemCategoryId.HasValue).ToList();
        }

        public async Task SetUpItemsMapAsync()
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeItems = await dbContext.Items.ToListAsync();

                foreach (var itemId in _itemsMap.Keys)
                {
                    if (!storeItems.Any(i => i.Id == itemId))
                    {
                        _itemsMap.TryRemove(itemId, out var removedItem);
                    }
                }

                foreach (var item in storeItems)
                {
                    if (_itemsMap.ContainsKey(item.Id))
                    {
                        _itemsMap[item.Id] = item;
                    }
                    else
                    {
                        _itemsMap.TryAdd(item.Id, item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting up Items Map: {ex}");
            }
        }

        public async Task<Item> GetWeaponItemAsync(int id)
        {
            // TODO: handle "Unknown" weapon deaths/kills, like Fatalities
            
            if (_weaponsMap == null || _weaponsMap.Count == 0)
            {
                await SetUpWeaponsMapAsync();
            }

            _weaponsMap.TryGetValue(id, out var item);

            return item;
        }

        public async Task SetUpWeaponsMapAsync()
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var nonWeaponItemCategoryIds = _itemCategoryService.GetNonWeaponItemCateogryIds();

                var storeWeapons = await dbContext.Items
                                        .Where(i => i.ItemCategoryId.HasValue && !nonWeaponItemCategoryIds.Contains((int)i.ItemCategoryId))
                                        .ToListAsync();

                foreach (var weaponId in _weaponsMap.Keys)
                {
                    if (!storeWeapons.Any(i => i.Id == weaponId))
                    {
                        _weaponsMap.TryRemove(weaponId, out var removedItem);
                    }
                }

                foreach (var weapon in storeWeapons)
                {
                    if (_weaponsMap.ContainsKey(weapon.Id))
                    {
                        _weaponsMap[weapon.Id] = weapon;
                    }
                    else
                    {
                        _weaponsMap.TryAdd(weapon.Id, weapon);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting up Weapons Map: {ex}");
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
                    await SetUpWeaponsMapAsync();

                    return;
                }
            }

            var success = await RefreshStoreFromCensus();

            if (!success && canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }

            //await SetUpItemsListAsync();
            await SetUpWeaponsMapAsync();
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
