using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public class CensusTitle
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusTitle(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusTitleModel>> GetAllTitles()
        {
            var query = _queryFactory.Create("title");
            query.SetLanguage("en");

            query.ShowFields("title_id", "name");

            return await query.GetBatchAsync<CensusTitleModel>();
        }
    }
}
