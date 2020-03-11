using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public interface IWebsocketMonitor : IStatefulHostedService
    {
        //Task OnApplicationStartup(CancellationToken cancellationToken);
        //Task OnApplicationShutdown(CancellationToken cancellationToken);

        Task Subscribe(CancellationToken cancellationToken);

        void AddCharacterSubscriptions(IEnumerable<string> characterIds);
        void RemoveCharacterSubscription(string characterId);
        void RemoveCharacterSubscriptions(IEnumerable<string> characterIds);
        void RemoveAllCharacterSubscriptions();
        Task<ServiceState> GetStatus();
        void EnableScoring();
        void DisableScoring();
        void AddCharacterSubscription(string characterId);
        void SetFacilitySubscription(int facilityId);
        void SetWorldSubscription(int worldId);
    }
}
