using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IZoneService : ILocallyBackedCensusStore
    {
        Task<IEnumerable<Zone>> GetAllZones();
        Task<IEnumerable<Zone>> GetAllZonesAsync();
        Task<Zone> GetZoneAsync(int zoneId);
        Task SetupZonesMap();
    }
}
