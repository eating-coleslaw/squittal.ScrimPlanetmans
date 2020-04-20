using squittal.ScrimPlanetmans.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IVehicleService : ILocallyBackedCensusStore
    {
        Task<VehicleInfo> GetVehicleInfoAsync(int vehicleId);

        VehicleInfo GetScrimVehicleInfo(int vehicleId);

        Task SetUpScrimmableVehicleInfosList();
    }
}
