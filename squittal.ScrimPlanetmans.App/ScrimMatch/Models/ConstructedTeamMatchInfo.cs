using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ConstructedTeamMatchInfo : ConstructedTeamInfo
    {
        public int? TeamOrdinal { get; set; }
        public int? ActiveFactionId { get; set; }

        public int OnlineMembersCount { get; set; } = 0;
        public int MatchTeamMembersCount { get; set; } = 0;
        public int TotalMembersCount { get; set; } = 0;

        public IEnumerable<Player> Players { get; set; }
    }
}
