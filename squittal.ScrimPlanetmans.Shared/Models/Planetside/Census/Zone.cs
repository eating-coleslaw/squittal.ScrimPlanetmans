using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside
{
    public class Zone
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int? HexSize { get; set; }
    }
}
