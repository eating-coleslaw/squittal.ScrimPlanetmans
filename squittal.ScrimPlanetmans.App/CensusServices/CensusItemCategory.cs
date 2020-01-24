using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public class CensusItemCategory
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusItemCategory(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusItemCategoryModel>> GetAllItemCategories()
        {
            var query = _queryFactory.Create("item_category");
            query.SetLanguage("en");

            query.ShowFields("item_category_id", "name");

            return await query.GetBatchAsync<CensusItemCategoryModel>();
        }
    }
}
