using System;
using System.Collections.Generic;
using System.Text;

namespace squittal.ScrimPlanetmans.Models.ScrimEngine
{
    public class MatchConfiguration
    {
        public string Title { get; set; } = "PS2 Scrims";
        public int RoundSecondsTotal { get; set; } = 900;
        public int RoundStartCountdownSeconds { get; set; } = 5;
        public string TeamAlias1 { get; set; }
        public string TeamAlias2 { get; set; }
        public string CensusServiceKey { get; set; } = "example";
    }
}
