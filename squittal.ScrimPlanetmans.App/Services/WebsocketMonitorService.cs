using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Models;
using System.Collections.Generic;
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

        public void AddCharacterSubscriptions(IEnumerable<string> characterIds)
        {
            _monitor.AddCharacterSubscriptions(characterIds);
        }

        public void RemoveCharacterSubscription(string characterId)
        {
            _monitor.RemoveCharacterSubscription(characterId);
        }

        public void RemoveCharacterSubscriptions(IEnumerable<string> characterIds)
        {
            _monitor.RemoveCharacterSubscriptions(characterIds);
        }

        public void RemoveAllCharacterSubscriptions()
        {
            _monitor.RemoveAllCharacterSubscriptions();
        }
    }
}
