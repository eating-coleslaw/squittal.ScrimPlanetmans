using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public class CensusFacility
    {
        public readonly ICensusQueryFactory _queryFactory;

        public CensusFacility(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusMapRegionModel>> GetAllMapRegions()
        {
            var query = _queryFactory.Create("map_region");

            query.ShowFields("map_region_id", "zone_id", "facility_id", "facility_name", "facility_type_id", "facility_type");

            return await query.GetBatchAsync<CensusMapRegionModel>();
        }

        public async Task<IEnumerable<CensusFacilityTypeModel>> GetAllFacilityTypes()
        {
            var query = _queryFactory.Create("facility_type");

            query.ShowFields("facility_type_id", "description");

            query.SetLimit(100);

            return await query.GetBatchAsync<CensusFacilityTypeModel>();
        }
    }
}
