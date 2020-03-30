using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services
{
    public interface IUpdateable
    {
        Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false);
    }
}
