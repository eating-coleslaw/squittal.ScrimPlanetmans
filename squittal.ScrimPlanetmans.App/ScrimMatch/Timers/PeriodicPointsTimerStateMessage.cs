using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Timers
{
    public class PeriodicPointsTimerStateMessage
    {
        public Guid Guid { get; private set; }
        public string TimerName { get; private set; }
        public TimerState State { get; private set; }
        public TimerDirection Direction { get; private set; }
        public bool IsRunning { get; private set; }
        public int PeriodSeconds { get; private set; }

        //public int? SecondsMax { get; private set; }
        //public int? SecondsRemaining { get; private set; }
        //public int SecondsElapsed { get; private set; }

        //public string TimerDisplay { get; private set; }

        public bool PeriodElapsed { get; private set; }
        public DateTime Timestamp { get; private set; }

        public DateTime? LastElapsedTime { get; private set; } = null;
        public DateTime? LastPausedTime { get; private set; } = null;
        public DateTime? LastResumedTime { get; private set; } = null;

        public PeriodicPointsTimerStateMessage(PeriodicPointsTimer timer, bool periodElapsed = false)
        {
            Guid = Guid.NewGuid();

            TimerName = timer.TimerName;
            IsRunning = timer.IsRunning;
            State = timer.State;
            PeriodSeconds = timer.PeriodSeconds;

            LastElapsedTime = timer.LastElapsedTime;
            LastPausedTime = timer.LastPausedTime;
            LastResumedTime = timer.LastResumedTime;

            //SecondsMax = timer.Tracker.SecondsMax;
            //SecondsRemaining = timer.Tracker.SecondsRemaining;
            //SecondsElapsed = timer.Tracker.SecondsElapsed;

            //TimerDisplay = timer.Tracker.ToString();

            PeriodElapsed = periodElapsed;
            Timestamp = DateTime.UtcNow;
        }
    }
}
