using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ConstructedTeamFormInfo : ConstructedTeamInfo
    {
        public string StringId { get; set; }

        public IEnumerable<Character> Characters { get; set; }
    }
}
