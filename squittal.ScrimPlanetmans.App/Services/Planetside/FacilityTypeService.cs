using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.Data;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class FacilityTypeService : IFacilityTypeService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusFacility _censusFacility;
        private readonly ILogger<FacilityTypeService> _logger;

        public FacilityTypeService(IDbContextHelper dbContextHelper, CensusFacility censusFacility, ILogger<FacilityTypeService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusFacility = censusFacility;
            _logger = logger;
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusFacility.GetFacilityTypesCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.FacilityTypes.CountAsync();
        }
    }
}
