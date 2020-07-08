using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ConstructedTeam
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Alias { get; set; }

        // TODO: Add migration for this
        public bool IsHiddenFromSelection { get; set; }

        //public int FirstPreferredFactionId { get; set; }
        //public int SecondPreferredFactionId { get; set; }
        //public int ThirdPreferredFactionId { get; set; }

        //public IEnumerable<ConstructedTeamFactionPreference> FactionPreferences { get; set; }
        public IEnumerable<ConstructedTeamPlayerMembership> PlayerMemberships { get; set; }
    }
}
