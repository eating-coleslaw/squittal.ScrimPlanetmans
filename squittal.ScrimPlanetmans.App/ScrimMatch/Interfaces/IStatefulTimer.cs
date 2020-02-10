using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IStatefulTimer
    {
        void Start();
        void Pause();
        void Reset();
        void Stop();
        void Halt();
        void Resume();
    }
}
