using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IStatefulTimer
    {
        MatchTimerStatus Status { get; }

        void Configure(TimeSpan timeSpan);
        void Start();
        void Pause();
        void Reset();
        void Stop();
        void Halt();
        void Resume();
    }
}
