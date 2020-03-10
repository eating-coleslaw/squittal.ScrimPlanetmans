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
        public int FacilityId { get => GetFacilityIdFromString(); }
        public string FacilityIdString { get; set; } = "-1";

        public bool EndRoundOnFacilityCapture { get; set; } = false; // TODO: move this setting to the Ruleset model

        private int GetFacilityIdFromString()
        {
            if (int.TryParse(FacilityIdString, out int intId))
            {
                return intId;
            }
            else
            {
                return -1;
            }

        }
    }
}
