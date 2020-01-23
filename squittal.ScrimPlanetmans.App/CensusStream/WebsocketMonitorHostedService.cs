using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketMonitorHostedService : IHostedService
    {
        private readonly IWebsocketMonitor _service;

        public WebsocketMonitorHostedService(IWebsocketMonitor service)
        {
            _service = service;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _service.OnApplicationStartup(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _service.OnApplicationShutdown(cancellationToken);
        }
    }
}
