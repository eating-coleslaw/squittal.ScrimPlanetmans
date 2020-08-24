using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IFacilityService : ILocallyBackedCensusStore
    {
        Task<MapRegion> GetMapRegionAsync(int mapRegionId);
        Task<MapRegion> GetMapRegionFromFacilityIdAsync(int facilityId);
        Task<MapRegion> GetMapRegionFromFacilityNameAsync(string facilityName);

        Task<MapRegion> GetMapRegionsByFacilityTypeAsync(int facilityTypeId);

        Task SetUpScrimmableMapRegionsAsync();
        Task<MapRegion> GetScrimmableMapRegionFromFacilityIdAsync(int facilityId);
        Task<IEnumerable<MapRegion>> GetScrimmableMapRegionsAsync();
    }
}
