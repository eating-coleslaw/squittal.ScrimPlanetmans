using System;
using System.Threading;

namespace squittal.ScrimPlanetmans.ScrimMatch.Timers
{
    public class TimeTracker
    {
        public int? SecondsMax { get; private set; }

        public int SecondsElapsed => _secondsElapsed;
        private int _secondsElapsed;


        public int? SecondsRemaining
        {
            get
            {
                if (HasTimeLimit)
                {
                    return SecondsMax.Value - SecondsElapsed;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool HasTimeLimit => SecondsMax.HasValue;


        public TimerDirection Direction => _direction;
        private TimerDirection _direction;

        public TimeTracker()
        {
        }

        public TimeTracker(int? secondsMax = null)
        {
            _direction = secondsMax.HasValue ? TimerDirection.Down : TimerDirection.Up;

            SecondsMax = secondsMax;

            _secondsElapsed = 0;
        }

        public void Configure(int? secondsMax = null)
        {
            _direction = secondsMax.HasValue ? TimerDirection.Down : TimerDirection.Up;

            SecondsMax = secondsMax;

            _secondsElapsed = 0;
        }

        public void StepForward()
        {
            IncrementElapsed();
        }

        private int IncrementElapsed()
        {
            Interlocked.Increment(ref _secondsElapsed);
            return _secondsElapsed;
        }

        public void ForceZeroRemaining()
        {
            if (HasTimeLimit)
            {
                Interlocked.Add(ref _secondsElapsed, SecondsRemaining.Value);
            }
            else
            {
                throw new InvalidOperationException("cannot force remaing time to zero when HasTimeLimit is false");
            }

        }

        public override string ToString()
        {
            int totalMinutes;

            if (!HasTimeLimit)
            {
                totalMinutes = SecondsElapsed;
            }
            else
            {
                totalMinutes = SecondsRemaining.Value;
            }

            return GetDigitalDisplay(totalMinutes);
        }

        private static string GetDigitalDisplay(int totalSeconds)
        {
            int totalMinutes = GetTotalMinutes(totalSeconds);

            string minutesDisplay = GetDisplayMinutes(totalMinutes);
            string secondsDisplay = GetDisplaySeconds(GetPartialSeconds(totalSeconds, totalMinutes));

            return $"{minutesDisplay}:{secondsDisplay}";
        }

        private static int GetTotalMinutes(int totalSeconds)
        {
            return totalSeconds / 60;
        }

        private static int GetPartialSeconds(int totalSeconds, int totalMinutes)
        {
            return totalSeconds - totalMinutes * 60;
        }

        private static string GetDisplayMinutes(int totalMinutes)
        {
            return totalMinutes > 9 ? $"{totalMinutes}" : $"0{totalMinutes}";
        }

        private static string GetDisplaySeconds(int partialSeconds)
        {
            return partialSeconds > 9 ? $"{partialSeconds}" : $"0{partialSeconds}";
        }
    }
}
