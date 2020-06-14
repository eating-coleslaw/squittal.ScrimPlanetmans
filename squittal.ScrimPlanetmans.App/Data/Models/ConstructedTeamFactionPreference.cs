using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ConstructedTeamFactionPreference
    {
        [Required]
        public int ConstructedTeamId { get; set; }

        [Required]
        public int PreferenceOrdinalValue { get; set; }

        [Required]
        public int FactionId { get; set; }

        public ConstructedTeam ConstructedTeam { get; set; }
    }
}
