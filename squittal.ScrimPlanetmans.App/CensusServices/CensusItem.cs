using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public class CensusItem
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusItem(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusItemModel>> GetAllItems()
        {
            var query = _queryFactory.Create("item");
            query.SetLanguage("en");

            query.ShowFields("item_id", "item_type_id", "item_category_id", "is_vehicle_weapon", "name", "description", "faction_id", "max_stack_size", "image_id");

            return await query.GetBatchAsync<CensusItemModel>();
        }

        public async Task<IEnumerable<CensusItemModel>> GetAllWeaponItems()
        {
            var query = _queryFactory.Create("item");
            query.SetLanguage("en");

            query.ShowFields("item_id", "item_type_id", "item_category_id", "is_vehicle_weapon", "name", "description", "faction_id", "max_stack_size", "image_id");

            query.Where("item_category_id").NotEquals(99);
            query.Where("item_category_id").NotEquals(101);
            query.Where("item_category_id").NotEquals(103);
            query.Where("item_category_id").NotEquals(105);
            query.Where("item_category_id").NotEquals(106);
            query.Where("item_category_id").NotEquals(107);
            query.Where("item_category_id").NotEquals(108);
            query.Where("item_category_id").NotEquals(133);
            query.Where("item_category_id").NotEquals(134);
            query.Where("item_category_id").NotEquals(135);
            query.Where("item_category_id").NotEquals(136);
            query.Where("item_category_id").NotEquals(137);
            query.Where("item_category_id").NotEquals(139);
            query.Where("item_category_id").NotEquals(140);
            query.Where("item_category_id").NotEquals(141);
            query.Where("item_category_id").NotEquals(142);
            query.Where("item_category_id").NotEquals(143);
            query.Where("item_category_id").NotEquals(145);
            query.Where("item_category_id").NotEquals(146);
            query.Where("item_category_id").NotEquals(148);

            return await query.GetBatchAsync<CensusItemModel>();
        }
        
        public async Task<int> GetItemsCount()
        {
            var results = await GetAllItems();

            return results.Count();
        }

        public async Task<int> GetWeaponItemsCount()
        {
            var results = await GetAllWeaponItems();

            return results.Count();
        }
    }
}
