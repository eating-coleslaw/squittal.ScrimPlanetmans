using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class MatchTimerStatus
    {
        public int SecondsMax { get; set; }
        public int SecondsRemaining { get; set; }
        public int SecondsElapsed { get; set; }

        public bool IsRunning { get; set; }

        public MatchTimerState State { get; set; }

        public string TimeRemainingDisplay { get => GetDigitalDisplay(SecondsRemaining); }
        public string TimeElapsedDisplay { get => GetDigitalDisplay(SecondsElapsed); }


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
        Resuming = 8
    };
}
