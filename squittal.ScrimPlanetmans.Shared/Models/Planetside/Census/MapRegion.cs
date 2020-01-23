using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside
{
    public class MapRegion
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int FacilityId { get; set; }

        public int ZoneId { get; set; }
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public string FacilityType { get; set; }
        public float? XPos { get; set; }
        public float? YPos { get; set; }
    }
}
