using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services
{
    public interface ICountableStore
    {
        Task<int> GetCensusCountAsync();
        Task<int> GetStoreCountAsync();
    }
}
