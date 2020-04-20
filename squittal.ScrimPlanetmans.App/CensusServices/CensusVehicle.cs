using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public class CensusVehicle
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusVehicle(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusVehicleModel>> GetAllVehicles()
        {
            //var query = _queryFactory.Create("vehicle");
            //query.SetLanguage("en");

            //query.ShowFields("vehicle_id", "name", "description", "cost", "cost_resource_id");

            //return await query.GetBatchAsync<CensusVehicleModel>();

            var query = _queryFactory.Create("vehicle");
            query.SetLanguage("en");

            query.ShowFields("vehicle_id", "name", "description", "type_id", "type_name", "cost", "cost_resource_id", "image_id");

            return await query.GetBatchAsync<CensusVehicleModel>();
        }

        public async Task<IEnumerable<CensusVehicleFactionModel>> GetAllVehicleFactions()
        {
            var query = _queryFactory.Create("vehicle_faction");
            query.SetLanguage("en");

            query.ShowFields("vehicle_id", "faction_id");

            return await query.GetBatchAsync<CensusVehicleFactionModel>();
        }

        public async Task<int> GetVehiclesCount()
        {
            var results = await GetAllVehicles();

            return results.Count();
        }

        public async Task<int> GetVehicleFactionsCount()
        {
            var results = await GetAllVehicleFactions();

            return results.Count();
        }

    }
}
