using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public class CensusProfile
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusProfile(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusProfileModel>> GetAllProfilesAsync()
        {
            var query = _queryFactory.Create("profile");
            query.SetLanguage("en");

            query.ShowFields("profile_id", "profile_type_id", "faction_id", "name", "image_id");

            return await query.GetBatchAsync<CensusProfileModel>();
        }
    }
}
