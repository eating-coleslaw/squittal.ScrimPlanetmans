using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ItemCategoryRule : ItemCategory
    {
        // Inherits: int Id (required), string Name

        [Required]
        public int Points { get; set; } = 0;

    }
}
