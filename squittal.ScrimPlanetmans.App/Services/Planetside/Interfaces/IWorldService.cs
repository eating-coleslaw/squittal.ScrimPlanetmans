using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IWorldService // : IUpdateable
    {
        Task<IEnumerable<World>> GetAllWorldsAsync();
        IEnumerable<World> GetAllWorlds();
        Task<World> GetWorldAsync(int worldId);
        Task RefreshStore();
        Task SetupWorldsList();
    }
}
