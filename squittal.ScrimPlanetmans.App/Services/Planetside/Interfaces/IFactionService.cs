using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IFactionService : ICountableStore, ILocallyBackedStore, IUpdateable
    {
        Task<IEnumerable<Faction>> GetAllFactionsAsync();
        string GetFactionAbbrevFromId(int factionId);
        Task<Faction> GetFactionAsync(int factionId);
        //Task RefreshStore();
    }
}
