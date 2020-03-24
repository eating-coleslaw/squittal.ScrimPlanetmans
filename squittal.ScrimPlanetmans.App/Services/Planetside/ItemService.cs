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
        private readonly CensusItemCategory _censusItemCategory;
        private readonly CensusItem _censusItem;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<ItemService> _logger;

        public string BackupSqlScriptFileName => "dbo.Item.Table.sql";


        private List<Item> _items = new List<Item>();
        private List<Item> _weapons = new List<Item>();

        private List<ItemCategory> _weaponCategories = new List<ItemCategory>();

        private List<ItemCategory> _itemCategories = new List<ItemCategory>();

        private readonly List<int> _nonWeaponItemCategoryIds = new List<int>()
        {
            99,  // Camo
            101, // Vehicles
            103, // Infantry Gear
            105, // Vehicle Gear
            106, // Armor Camo
            107, // Weapon Camo
            108, // Vehicle Camo
            133, // Implants
            134, // Consolidated Camo
            135, // VO Packs
            136, // Male VO Pack
            137, // Female VO Pack
            139, // Infantry Abilities
            140, // Vehicle Abilities
            141, // Boosts & Utilities
            142, // Consolidated Decal
            143, // Attachments
            145, // ANT Utility
            148  // ANT Harvesting Tool
        };

        private readonly List<int> _infantryItemCategoryIds = new List<int>()
        {
            2,   // Knife
            3,   // Pistol
            4,   // Shotgun,
            5,   // SMG
            6,   // LMG
            7,   // Assault Rifle
            8,   // Carbine
            11,  // Sniper Rifle
            12,  // Scout Rifle
            13,  // Rocket Launcher
            14,  // Heavy Weapon
            17,  // Grenade
            18,  // Explosive
            19,  // Battle Rifle
            24,  // Crossbow
            100, // Infantry
            102, // Infantry Weapons
            147, // Aerial Combat Weapon (i.e. rocklet rifles)
            157  // Hybrid Rifle
        };

        private readonly List<int> _groundVehicleItemCategoryIds = new List<int>()
        {
            109, // Flash Primary Weapon
            114, // Harasser Top Gunner
            118, // Lightning Primary Weapon 
            119, // Magrider Gunner Weapon
            120, // Magrider Primary Weapon
            123, // Prowler Gunner Weapon
            124, // Prowler Primary Weapon
            129, // Sunderer Front Gunner
            130, // Sunderer Rear Gunner
            131, // Vanguard Gunner Weapon
            132, // Vanguard Primary Weapon
            144  // ANT Top Turret
        };

        private readonly List<int> _airVehicleItemCategoryIds = new List<int>()
        {
            110, // Galaxy Left Weapon
            111, // Galaxy Tail Weapon
            112, // Galaxy Right Weapon
            113, // Galaxy Top Weapon
            115, // Liberator Belly Weapon
            116, // Liberator Nose Cannon
            117, // Liberator Tail Weapon
            121, // Mosquito Nose Cannon
            122, // Mosquito Wing Mount
            125, // Reaver Nose Cannon
            126, // Reaver Wing Mount
            127, // Scythe Nose Cannon
            128, // Scythe Wing Mount
            138  // Valkyrie Nose Gunner
        };

        public ItemService(IDbContextHelper dbContextHelper, CensusItemCategory censusItemCategory, CensusItem censusItem, ISqlScriptRunner sqlScriptRunner, ILogger<ItemService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusItemCategory = censusItemCategory;
            _censusItem = censusItem;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<Item> GetItem(int itemId)
        {
            //IEnumerable<CensusItemModel> censusItems;

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

        public async Task<IEnumerable<int>> GetItemCategoryIdsAsync()
        {
            if (_itemCategories == null || !_itemCategories.Any())
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                _itemCategories = await dbContext.ItemCategories.ToListAsync();

                //return await dbContext.ItemCategories.Select(ic => ic.Id).ToListAsync();
            }

            return _itemCategories.Select(ic => ic.Id).ToList();
        }


        public async Task SetUpItemsListAsync()
        {
            if (_items == null || _items.Count == 0)
            {
                //var censusItems = await _censusItem.GetAllItems();

                //if (censusItems != null)
                //{
                //    _items = censusItems.Select(ConvertToDbModel).ToList();
                //}

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

        public ItemCategory GetWeaponItemCategory(int itemCategoryId)
        {
            return _weaponCategories.FirstOrDefault(w => w.Id == itemCategoryId);
        }

        public IEnumerable<ItemCategory> GetWeaponItemCategories()
        {
            return _weaponCategories.ToList();
        }

        public async Task<IEnumerable<int>> GetWeaponItemCategoryIdsAsync()
        {
            if (_weaponCategories == null || !_weaponCategories.Any())
            {
                await SetUpWeaponCategoriesListAsync();
            }

            return GetWeaponItemCategoryIds();
        }

        public IEnumerable<int> GetWeaponItemCategoryIds()
        {
            return _weaponCategories.Select(wc => wc.Id).ToList();
        }

        public async Task SetUpWeaponsListAsnyc()
        {
            if (_weapons == null || _weapons.Count == 0)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                _weapons = await dbContext.Items
                                        .Where(i => i.ItemCategoryId.HasValue && !_nonWeaponItemCategoryIds.Contains((int)i.ItemCategoryId))
                                        .ToListAsync();
            }
        }

        public async Task SetUpWeaponCategoriesListAsync()
        {
            if (_weaponCategories == null || !_weaponCategories.Any())
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                _weaponCategories = await dbContext.ItemCategories
                                            .Where(i => !_nonWeaponItemCategoryIds.Contains(i.Id))
                                            .ToListAsync();
            }
        }

        public IEnumerable<int> GetNonWeaponItemCateogryIds()
        {
            return _nonWeaponItemCategoryIds;
        }

        public async Task RefreshStore()
        {
            bool refreshStore = true;
            bool anyItems = false;
            bool anyCategories = false;

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                anyItems = await dbContext.Items.AnyAsync();

                if (anyItems == true)
                {
                    anyCategories = await dbContext.ItemCategories.AnyAsync();
                }

                refreshStore = (anyItems == false || anyCategories == false);
            }
            
            if (refreshStore != true)
            {
                return;
            }
            
            var itemCategories = await _censusItemCategory.GetAllItemCategories();

            if (itemCategories != null)
            {
                await UpsertRangeAsync(itemCategories.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Item Categories store: {itemCategories.Count()} entries");
            }

            var items = await _censusItem.GetAllItems();

            if (items != null)
            {
                await UpsertRangeAsync(items.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Items store: {itemCategories.Count()} entries");
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
        
        private async Task UpsertRangeAsync(IEnumerable<ItemCategory> censusEntities)
        {
            var createdEntities = new List<ItemCategory>();

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var storedEntities = await dbContext.ItemCategories.ToListAsync();

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
                        dbContext.ItemCategories.Update(storeEntity);
                    }
                }

                if (createdEntities.Any())
                {
                    await dbContext.ItemCategories.AddRangeAsync(createdEntities);
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

        private static ItemCategory ConvertToDbModel(CensusItemCategoryModel itemCategory)
        {
            return new ItemCategory
            {
                Id = itemCategory.ItemCategoryId,
                Name = itemCategory.Name.English
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
