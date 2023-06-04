using squittal.ScrimPlanetmans.ScrimMatch.Timers;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class MatchTimerTickMessage
    {
        public string TimerName { get; private set; }
        public TimerState State { get; private set; }
        public TimerDirection Direction { get; private set; }
        public bool IsRunning { get; private set; }

        public int? SecondsMax { get; private set; }
        public int? SecondsRemaining { get; private set; }
        public int SecondsElapsed { get; private set; }

        public string TimerDisplay { get; private set; }

        public string Info { get; set; } = string.Empty;
        
        public MatchTimerTickMessage(StatefulTimer statefulTimer)
        {
            IsRunning = statefulTimer.IsRunning;
            State = statefulTimer.State;

            SecondsMax = statefulTimer.Tracker.SecondsMax;
            SecondsRemaining = statefulTimer.Tracker.SecondsRemaining;
            SecondsElapsed = statefulTimer.Tracker.SecondsElapsed;

            TimerDisplay = statefulTimer.Tracker.ToString();
        }
    }
}
