using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IOverlayStateService
    {
        Guid GetActivePeriodicPointsProgressBarGuid();
        void SetActivePeriodicPointsProgressBarGuid(Guid newGuid);
    }
}
