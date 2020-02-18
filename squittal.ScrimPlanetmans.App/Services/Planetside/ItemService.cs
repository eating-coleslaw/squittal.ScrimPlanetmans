using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
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
        private readonly ILogger<ItemService> _logger;

        private List<Item> _items = new List<Item>();
        private List<ItemCategory> _itemCategories = new List<ItemCategory>();

        public ItemService(IDbContextHelper dbContextHelper, CensusItemCategory censusItemCategory, CensusItem censusItem, ILogger<ItemService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusItemCategory = censusItemCategory;
            _censusItem = censusItem;
            _logger = logger;
        }

        public async Task<Item> GetItem(int itemId)
        {
            //IEnumerable<CensusItemModel> censusItems;

            if (_items == null || _items.Count == 0)
            {
                //return _items.FirstOrDefault(i => i.Id == itemId);
                await SetUpItemsListAsync();
            }

            //censusItems = await _censusItem.GetAllItems();

            //if (censusItems == null)
            //{
            //    return null;
            //}

            //_items = censusItems.Select(ConvertToDbModel).ToList();

            //await SetUpItemsListAsync();

            return _items.FirstOrDefault(i => i.Id == itemId);

            /*
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Items.FirstOrDefaultAsync(i => i.Id == itemId);
            }
            */
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryId(int categoryId)
        {
            if (_items == null || _items.Count == 0)
            {
                //return _items.FirstOrDefault(i => i.Id == itemId);
                await SetUpItemsListAsync();
            }

            return _items.Where(i => i.ItemCategoryId == categoryId && i.ItemCategoryId.HasValue)
                         .ToList();

            /*
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Items
                                .Where(i => i.ItemCategoryId == categoryId && i.ItemCategoryId.HasValue)
                                .ToListAsync();
            }
            */
        }

        public async Task<IEnumerable<int>> GetItemCategoryIdsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.ItemCategories.Select(ic => ic.Id).ToListAsync();
        }


        private async Task SetUpItemsListAsync()
        {
            if (_items == null || _items.Count == 0)
            {
                var censusItems = await _censusItem.GetAllItems();

                if (censusItems != null)
                {
                    _items = censusItems.Select(ConvertToDbModel).ToList();
                }
            }
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
        
    }
}
