using squittal.ScrimPlanetmans.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public interface IWebsocketMonitor : IStatefulHostedService
    {
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
        void EnableEventStoring();
        void DisableEventStoring();
    }
}
