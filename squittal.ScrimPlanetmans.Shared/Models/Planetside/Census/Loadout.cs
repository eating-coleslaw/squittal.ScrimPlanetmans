using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside
{
    public class Loadout
    {
        [Required]
        public int Id { get; set; }

        public int ProfileId { get; set; }
        public int FactionId { get; set; }
        public string CodeName { get; set; }

        public Profile Profile { get; set; }
    }
}
