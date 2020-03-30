using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services
{
    public interface IUpdateable
    {
        //string ServiceName { get; }
        //TimeSpan UpdateInterval { get; }
        Task RefreshStore(bool onlyQueryCensusIfEmpty = false);
    }
}
