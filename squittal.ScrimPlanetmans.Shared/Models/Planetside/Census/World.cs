using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside
{
    public class World
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
