using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ConstructedTeamPlayerMembership
    {
        [Required]
        public int ConstructedTeamId { get; set; }

        [Required]
        public string CharacterId { get; set; }

        public int FactionId { get; set; }

        public string Alias { get; set; }

        public ConstructedTeam ConstructedTeam { get; set; }
    }
}
