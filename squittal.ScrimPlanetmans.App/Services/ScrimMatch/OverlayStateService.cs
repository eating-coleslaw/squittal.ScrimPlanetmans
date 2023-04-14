using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{

    public class OverlayStateService : IOverlayStateService
    {
        public ILogger<OverlayStateService> _logger;

        private Guid _activePeriodicPointsProgressBarGuid;
        private readonly AutoResetEvent _activePeriodicPointsProgressBarGuidLock = new AutoResetEvent(true);


        public OverlayStateService(ILogger<OverlayStateService> logger)
        {
            _logger = logger;
        }

        public void SetActivePeriodicPointsProgressBarGuid(Guid newGuid)
        {
            _activePeriodicPointsProgressBarGuidLock.WaitOne();
            _activePeriodicPointsProgressBarGuid = newGuid;
            _activePeriodicPointsProgressBarGuidLock.Set();
        }

        public Guid GetActivePeriodicPointsProgressBarGuid()
        {
            return _activePeriodicPointsProgressBarGuid;
        }
    }
}
