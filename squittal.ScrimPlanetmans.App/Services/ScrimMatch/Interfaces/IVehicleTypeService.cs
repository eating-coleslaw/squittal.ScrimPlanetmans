using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IVehicleTypeService
    {
        Task SeedVehicleClasses();
        IEnumerable<VehicleType> GetVehicleTypes();
    }
}
