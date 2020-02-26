using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IStatefulTimer
    {
        event EventHandler<MatchTimerTickEventArgs> RaiseMatchTimerTickEvent;

        //MatchTimerStatus Status { get; }

        void Configure(TimeSpan timeSpan);
        void Start();
        void Pause();
        void Reset();
        void Stop();
        void Halt();
        void Resume();
    }
}
