using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ConstructedTeamFormInfo : ConstructedTeamInfo
    {
        public IEnumerable<Character> Characters { get; set; }
    }
}
