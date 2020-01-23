using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Models;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services
{
    public class WebsocketMonitorService
    {
        private readonly IWebsocketMonitor _monitor;

        public WebsocketMonitorService(IWebsocketMonitor monitor)
        {
            _monitor = monitor;
        }

        public async Task StartStreamSubscription()
        {
            await _monitor.Subscribe(CancellationToken.None);
        }

        public async Task StopStreamSubscription()
        {
            await _monitor.StopAsync(CancellationToken.None);
        }

        public async Task<ServiceState> GetStatus()
        {
            var status = await _monitor.GetStateAsync(CancellationToken.None);

            if (status == null)
            {
                return null;
            }

            return status;
        }
    }
}
