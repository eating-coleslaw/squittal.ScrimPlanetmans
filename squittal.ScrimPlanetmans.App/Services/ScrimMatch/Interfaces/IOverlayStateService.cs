using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IOverlayStateService
    {
        void InactivatePeriodicPointsProgressBarGuid(Guid guid);
        bool IsActivePeriodicPointsProgressBarGuid(Guid guid);
        void SetActivePeriodicPointsProgressBarGuid(Guid newGuid);
    }
}
