using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside.Events
{
    public class PlayerFacilityDefend
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public string OutfitId { get; set; }

        public MapRegion Facility { get; set; }
    }
}
