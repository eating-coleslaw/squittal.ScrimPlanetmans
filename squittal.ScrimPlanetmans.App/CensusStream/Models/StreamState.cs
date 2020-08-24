// Credit to Lampjaw @ Voidwell.DaybreakGames
using System;

namespace squittal.ScrimPlanetmans.CensusStream.Models
{
    public class StreamState
    {
        public DateTime LastStateChangeTime { get; set; }
        public object Contents { get; set; }
    }
}

