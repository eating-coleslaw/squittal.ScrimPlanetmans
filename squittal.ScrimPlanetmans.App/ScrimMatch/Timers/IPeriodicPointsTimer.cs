using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Timers
{
    public interface IPeriodicPointsTimer
    {
        //event EventHandler<ScrimMessageEventArgs<PeriodicPointsTimerStateMessage>> RaisePeriodicPointsTimerTickEvent;
        bool IsRunning { get; }
        TimerState State { get; }

        void Configure(TimeSpan? timeSpan);
        void Start();
        void Pause();
        void Reset();
        void Restart();
        void Stop();
        void Halt();
        void Resume();
        bool CanStart();
    }
}
