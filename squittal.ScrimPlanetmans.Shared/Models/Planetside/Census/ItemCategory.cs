using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside
{
    public class ItemCategory
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
