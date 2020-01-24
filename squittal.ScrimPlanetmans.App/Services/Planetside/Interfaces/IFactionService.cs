using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IFactionService
    {
        Task<IEnumerable<Faction>> GetAllFactionsAsync();
        Task<Faction> GetFactionAsync(int factionId);
        //Task RefreshStore();
    }
}
