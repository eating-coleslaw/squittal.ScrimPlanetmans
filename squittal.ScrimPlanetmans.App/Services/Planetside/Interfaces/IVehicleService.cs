using squittal.ScrimPlanetmans.Models.Planetside;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IVehicleService : ILocallyBackedCensusStore
    {
        Task<Vehicle> GetVehicleInfoAsync(int vehicleId);

        Vehicle GetScrimVehicleInfo(int vehicleId);

        Task SetUpScrimmableVehicleInfosList();
    }
}
