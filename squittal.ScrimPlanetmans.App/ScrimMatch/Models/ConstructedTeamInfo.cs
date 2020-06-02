using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ConstructedTeamInfo
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public IEnumerable<ConstructedTeamFactionPreference> FactionPreferences { get; set; }
        //public IEnumerable<Player> Players { get; set; } // For Scrim Match Display
        //public IEnumerable<Character> Characters { get; set; } // For Constructor Form Display
    }
}
