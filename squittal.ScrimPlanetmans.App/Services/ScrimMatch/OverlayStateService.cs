using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{

    public class OverlayStateService : IOverlayStateService
    {
        public ILogger<OverlayStateService> _logger;

        private readonly AutoResetEvent _activePeriodicPointsProgressBarGuidLock = new AutoResetEvent(true);
        public ConcurrentDictionary<Guid, int> _activePeriodicPointsProgressBarGuids;


        public OverlayStateService(ILogger<OverlayStateService> logger)
        {
            _logger = logger;

            _activePeriodicPointsProgressBarGuids = new ConcurrentDictionary<Guid, int>();
        }

        public void SetActivePeriodicPointsProgressBarGuid(Guid newGuid)
        {
            _activePeriodicPointsProgressBarGuidLock.WaitOne();
            //_activePeriodicPointsProgressBarGuid = newGuid;
            
            _activePeriodicPointsProgressBarGuids.TryAdd(newGuid, 1);

            _activePeriodicPointsProgressBarGuidLock.Set();
        }

        public bool IsActivePeriodicPointsProgressBarGuid(Guid guid)
        {
            return _activePeriodicPointsProgressBarGuids.ContainsKey(guid);
        }

        public void InactivatePeriodicPointsProgressBarGuid(Guid guid)
        {
            _activePeriodicPointsProgressBarGuids.TryRemove(guid, out var _);
        }
    }
}
