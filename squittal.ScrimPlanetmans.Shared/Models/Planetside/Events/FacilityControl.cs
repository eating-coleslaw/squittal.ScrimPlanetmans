using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside.Events
{
    public class FacilityControl
    {
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public int WorldId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int ZoneId { get; set; }
        public int? NewFactionId { get; set; }
        public int? OldFactionId { get; set; }
        public int DurationHeld { get; set; }
        public string OutfitId { get; set; }

        // Voidwell-Specific Stuff
        //public float? ZoneControlVs { get; set; }
        //public float? ZoneControlNc { get; set; }
        //public float? ZoneControlTr { get; set; }
        //public float? ZoneControlNs { get; set; }
        //public int? ZonePopulationVs { get; set; }
        //public int? ZonePopulationNc { get; set; }
        //public int? ZonePopulationTr { get; set; }
        //public int? ZonePopulationNs { get; set; }
    }
}
