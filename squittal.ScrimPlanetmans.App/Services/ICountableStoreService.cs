using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services
{
    public interface ICountableStoreService
    {
        Task<int> GetCensusCountAsync();
        Task<int> GetStoreCountAsync();

        //Task RefreshStore();
    }
}
