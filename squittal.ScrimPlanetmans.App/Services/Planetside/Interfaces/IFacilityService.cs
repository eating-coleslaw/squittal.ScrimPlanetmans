using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IFacilityService
    {
        Task<MapRegion> GetMapRegion(int mapRegionId);
        Task<MapRegion> GetMapRegionFromFacilityId(int facilityId);
        Task<MapRegion> GetMapRegionFromFacilityName(string facilityName);

        Task<MapRegion> GetMapRegionsByFacilityType(int facilityTypeId);

        IEnumerable<MapRegion> GetScrimmableMapRegions();

        Task RefreshStore();
        Task SetUpScimmableMapRegionsAsync();
    }
}
