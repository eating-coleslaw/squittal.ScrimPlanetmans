using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IFacilityService : ICountableStore, ILocallyBackedStore, IUpdateable
    {
        Task<MapRegion> GetMapRegionAsync(int mapRegionId);
        Task<MapRegion> GetMapRegionFromFacilityIdAsync(int facilityId);
        Task<MapRegion> GetMapRegionFromFacilityNameAsync(string facilityName);

        Task<MapRegion> GetMapRegionsByFacilityTypeAsync(int facilityTypeId);

        IEnumerable<MapRegion> GetScrimmableMapRegions();
        MapRegion GetScrimmableMapRegionFromFacilityId(int facilityId);

        //Task RefreshStore();
        Task SetUpScrimmableMapRegionsAsync();
    }
}
