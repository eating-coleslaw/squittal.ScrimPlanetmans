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
using System.Threading;
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

        private ConcurrentDictionary<int, Item> ItemsMap { get; set; } = new ConcurrentDictionary<int, Item>();
        private readonly SemaphoreSlim _itemMapSetUpSemaphore = new SemaphoreSlim(1);
        
        private ConcurrentDictionary<int, Item> WeaponsMap { get; set; } = new ConcurrentDictionary<int, Item>();
        private readonly SemaphoreSlim _weaponMapSetUpSemaphore = new SemaphoreSlim(1);
        
        public string BackupSqlScriptFileName => "CensusBackups\\dbo.Item.Table.sql";

        public event EventHandler<StoreRefreshMessageEventArgs> RaiseStoreRefreshEvent;
        public delegate void StoreRefreshMessageEventHandler(object sender, StoreRefreshMessageEventArgs e);

        protected virtual void OnRaiseStoreRefreshEvent(StoreRefreshMessageEventArgs e)
        {
            RaiseStoreRefreshEvent?.Invoke(this, e);
        }

        private void SendStoreRefreshEventMessage(StoreRefreshSource refreshSource)
        {
            OnRaiseStoreRefreshEvent(new StoreRefreshMessageEventArgs(refreshSource));
        }

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
            if (ItemsMap == null || ItemsMap.Count == 0)
            {
                await SetUpItemsMapAsync();
            }

            ItemsMap.TryGetValue(itemId, out var item);

            return item;
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryIdAsync(int categoryId)
        {
            if (ItemsMap == null || ItemsMap.Count == 0)
            {
                await SetUpItemsMapAsync();
            }

            return ItemsMap.Values.Where(i => i.ItemCategoryId == categoryId && i.ItemCategoryId.HasValue).ToList();
        }

        public async Task SetUpItemsMapAsync()
        {
            await _itemMapSetUpSemaphore.WaitAsync();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeItems = await dbContext.Items.ToListAsync();

                foreach (var itemId in ItemsMap.Keys)
                {
                    if (!storeItems.Any(i => i.Id == itemId))
                    {
                        ItemsMap.TryRemove(itemId, out var removedItem);
                    }
                }

                foreach (var item in storeItems)
                {
                    if (ItemsMap.ContainsKey(item.Id))
                    {
                        ItemsMap[item.Id] = item;
                    }
                    else
                    {
                        ItemsMap.TryAdd(item.Id, item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting up Items Map: {ex}");
            }
            finally
            {
                _itemMapSetUpSemaphore.Release();
            }
        }

        public async Task<IEnumerable<Item>> GetAllWeaponItemsAsync()
        {
            if (WeaponsMap == null || WeaponsMap.Count == 0)
            {
                await SetUpWeaponsMapAsync();
            }

            if (WeaponsMap == null || WeaponsMap.Count == 0)
            {
                return null;
            }

            return WeaponsMap.Values.ToList();
        }

        public async Task<Item> GetWeaponItemAsync(int id)
        {
            // TODO: handle "Unknown" weapon deaths/kills, like Fatalities
            
            if (WeaponsMap == null || WeaponsMap.Count == 0)
            {
                await SetUpWeaponsMapAsync();
            }

            WeaponsMap.TryGetValue(id, out var item);

            return item;
        }

        public async Task SetUpWeaponsMapAsync()
        {
            await _weaponMapSetUpSemaphore.WaitAsync();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var nonWeaponItemCategoryIds = _itemCategoryService.GetNonWeaponItemCateogryIds();

                var storeWeapons = await dbContext.Items
                                        .Where(i => i.ItemCategoryId.HasValue && !nonWeaponItemCategoryIds.Contains((int)i.ItemCategoryId))
                                        .ToListAsync();

                foreach (var weaponId in WeaponsMap.Keys)
                {
                    if (!storeWeapons.Any(i => i.Id == weaponId))
                    {
                        WeaponsMap.TryRemove(weaponId, out var removedItem);
                    }
                }

                foreach (var weapon in storeWeapons)
                {
                    if (WeaponsMap.ContainsKey(weapon.Id))
                    {
                        WeaponsMap[weapon.Id] = weapon;
                    }
                    else
                    {
                        WeaponsMap.TryAdd(weapon.Id, weapon);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting up Weapons Map: {ex}");
            }
            finally
            {
                _weaponMapSetUpSemaphore.Release();
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
                    await SetUpWeaponsMapAsync();

                    return;
                }
            }

            var success = await RefreshStoreFromCensus();

            if (!success && canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }

            await SetUpWeaponsMapAsync();

            SendStoreRefreshEventMessage(StoreRefreshSource.CensusApi);
        }

        public async Task<bool> RefreshStoreFromCensus()
        {
            IEnumerable<CensusItemModel> items = new List<CensusItemModel>();

            try
            {
                items = await _censusItem.GetAllWeaponItems();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all weapon Items. Refreshing store from backup...");
                return false;
            }

            if (items != null && items.Any())
            {
                await UpsertRangeAsync(items.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Items store: {items.Count()} entries");

                //SendStoreRefreshEventMessage(StoreRefreshSource.CensusApi);

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
                var storedRuleEntities = await dbContext.RulesetItemRules.ToListAsync();

                foreach (var censusEntity in censusEntities)
                {
                    // First, update the Item entities
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

                    // Then, update any Item Rule entities. It's simpler to just do this in bulk here,
                    // instead of handling it in RulesetDataService
                    var ruleStoreEntities = storedRuleEntities.Where(rule => rule.ItemId == censusEntity.Id).ToList();
                    foreach (var ruleStoreEntity in ruleStoreEntities)
                    {
                        if (ruleStoreEntity.ItemCategoryId != censusEntity.ItemCategoryId)
                        {
                            ruleStoreEntity.ItemCategoryId = censusEntity.ItemCategoryId.GetValueOrDefault();
                            dbContext.RulesetItemRules.Update(ruleStoreEntity);
                        }
                    }
                }

                if (createdEntities.Any())
                {
                    await dbContext.Items.AddRangeAsync(createdEntities);
                }

                await dbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"Finished upserting Items");
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
            return await _censusItem.GetWeaponItemsCount();
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

            //SendStoreRefreshEventMessage(StoreRefreshSource.BackupSqlScript);
        }
    }
}
