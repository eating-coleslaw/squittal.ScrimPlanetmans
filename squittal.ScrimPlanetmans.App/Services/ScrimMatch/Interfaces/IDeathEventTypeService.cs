using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IDeathEventTypeService
    {
        Task SeedDeathTypes();
        IEnumerable<DeathEventType> GetDeathEventTypes();
    }
}
