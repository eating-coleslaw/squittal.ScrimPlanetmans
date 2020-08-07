using squittal.ScrimPlanetmans.Data.Models;
using System;
using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchInfo
    {
        public string ScrimMatchId { get; set; }
        public DateTime StartTime { get; set; }
        public string Title { get; set; }

        public Dictionary<int, string> TeamAliases { get; set; } = new Dictionary<int, string>()
        {
            { 1, "???" },
            { 2, "???" }
        };

        public int RoundCount { get; set; }

        // World & Facility correspond to last round's configuration
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

            SetTeamAliases();

            RoundCount = lastRoundConfiguration.ScrimMatchRound;
            WorldId = lastRoundConfiguration.WorldId;
            FacilityId = lastRoundConfiguration.FacilityId;
        }

        public void SetTeamAliases()
        {
            if (string.IsNullOrWhiteSpace(ScrimMatchId))
            {
                return;
            }

            var idParts = ScrimMatchId.Split("_");

            if (!TeamAliases.TryAdd(1, idParts[1]))
            {
                TeamAliases[1] = idParts[1];
            }

            if (!TeamAliases.TryAdd(2, idParts[2]))
            {
                TeamAliases[2] = idParts[2];
            }
        }
    }
}
