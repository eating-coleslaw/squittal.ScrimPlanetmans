using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class MatchTimerStatus
    {
        //public int SecondsMax { get => _secondsMax; }

        //public int SecondsRemaining { get => _secondsRemaining; }
        //public int SecondsElapsed { get => _secondsElapsed; }

        private int _secondsMax;
        private int _secondsRemaining;
        private int _secondsElapsed;

        public bool IsRunning { get; set; }

        public MatchTimerState State { get; set; }

        public string TimeRemainingDisplay { get => GetDigitalDisplay(GetSecondsRemaining()); }
        public string TimeElapsedDisplay { get => GetDigitalDisplay(GetSecondsElapsed()); }

        public void ConfigureTimer(int secondsMax)
        {
            _secondsMax = secondsMax;
            _secondsRemaining = _secondsMax;
            _secondsElapsed = 0;
        }

        public int DecrementRemaining()
        {
            Interlocked.Decrement(ref _secondsRemaining);
            return _secondsRemaining;
        }

        public int IncrementElapsed()
        {
            Interlocked.Increment(ref _secondsElapsed);
            return _secondsElapsed;
        }
        
        public int ForceZeroRemaining()
        {
            Interlocked.Add(ref _secondsRemaining, -_secondsRemaining);
            return _secondsRemaining;
        }

        public int ForceMaxElapsed()
        {
            Interlocked.Add(ref _secondsElapsed, _secondsRemaining);
            return _secondsElapsed;
        }

        public int GetSecondsMax()
        {
            return _secondsMax;
        }

        public int GetSecondsRemaining()
        {
            return _secondsRemaining;
        }

        public int GetSecondsElapsed()
        {
            return _secondsElapsed;
        }

        private string GetDigitalDisplay(int totalSeconds)
        {
            int totalMinutes = GetTotalMinutes(totalSeconds);

            string minutesDisplay = GetDisplayMinutes(totalMinutes);
            string secondsDisplay = GetDisplaySeconds(GetPartialSeconds(totalSeconds, totalMinutes));

            return $"{minutesDisplay}:{secondsDisplay}";
        }

        private int GetTotalMinutes(int totalSeconds)
        {
            return (totalSeconds / 60);
        }

        private int GetPartialSeconds(int totalSeconds, int totalMinutes)
        {
            return totalSeconds - (totalMinutes * 60);
        }

        private string GetDisplayMinutes(int totalMinutes)
        {
            return (totalMinutes > 9) ? $"{totalMinutes}" : $"0{totalMinutes}";
        }

        private string GetDisplaySeconds(int partialSeconds)
        {
            return (partialSeconds > 9) ? $"{partialSeconds}" : $"0{partialSeconds}";
        }
    }

    public enum MatchTimerState
    {
        Starting = 1,
        Running = 2,
        Paused = 3,
        Stopping = 4,
        Stopped = 5,
        Halting = 6,
        Initialized = 7,
        Resuming = 8,
        Configuring = 9,
        Configured = 10,
        Uninitialized = 11
    };
}
