using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IWorldService : ILocallyBackedCensusStore
    {
        Task<IEnumerable<World>> GetAllWorldsAsync();
        Task<World> GetWorldAsync(int worldId);
        Task SetUpWorldsMap();
    }
}
