using squittal.ScrimPlanetmans.Data.Models;
using System;

namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchInfo
    {
        public string ScrimMatchId { get; set; }
        public DateTime StartTime { get; set; }
        public string Title { get; set; }

        //public string FirstTeamTag { get; set; }
        //public string SecondTeamTag { get; set; }

        public int RoundCount { get; set; }

        // World & Facility correspond to last round's configuration
        //public bool IsManualWorldId { get; set; } = false;
        public int WorldId { get; set; }
        public string WorldName { get; set; }
        public int? FacilityId { get; set; }
        public string FacilityName { get; set; }

        //public bool EndRoundOnFacilityCapture { get; set; } = false;

        public ScrimMatchInfo()
        {

        }

        public ScrimMatchInfo(Data.Models.ScrimMatch scrimMatch, ScrimMatchRoundConfiguration lastRoundConfiguration, string firstTeamTag, string secondTeamTag)
        {
            ScrimMatchId = scrimMatch.Id;
            StartTime = scrimMatch.StartTime;
            Title = scrimMatch.Title;

            //FirstTeamTag = firstTeamTag;
            //SecondTeamTag = secondTeamTag;

            RoundCount = lastRoundConfiguration.ScrimMatchRound;
            //IsManualWorldId = lastRoundConfiguration.IsManualWorldId;
            WorldId = lastRoundConfiguration.WorldId;
            FacilityId = lastRoundConfiguration.FacilityId;
            //EndRoundOnFacilityCapture = lastRoundConfiguration.IsRoundEndedOnFacilityCapture;
        }
    }
}
