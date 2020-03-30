using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public interface IItemCategoryService : ICountableStore, ILocallyBackedStore, IUpdateable
    {
        //Task<int> GetCensusCountAsync();
        //Task<int> GetStoreCountAsync();
        //Task RefreshStore(bool onlyQueryCensusIfEmpty); // TODO: for testing only!
    }
}