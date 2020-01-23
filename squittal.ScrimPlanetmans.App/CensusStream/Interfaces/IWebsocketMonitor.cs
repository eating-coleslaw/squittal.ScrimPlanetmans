using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public interface IWebsocketMonitor : IStatefulHostedService
    {
        //Task OnApplicationStartup(CancellationToken cancellationToken);
        //Task OnApplicationShutdown(CancellationToken cancellationToken);
        Task Subscribe(CancellationToken cancellationToken);
    }
}
