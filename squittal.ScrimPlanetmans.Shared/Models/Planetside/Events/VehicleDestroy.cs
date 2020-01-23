using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside.Events
{
    public class VehicleDestroy
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int? VehicleId { get; set; }
        [Required]
        public string AttackerCharacterId { get; set; }
        [Required]
        public int? AttackerVehicleId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public int? FacilityId { get; set; }
        public int? FactionId { get; set; }

        /*
        public Character Character { get; set; }
        public Character AttackerCharacter { get; set; }
        public Vehicle AttackerVehicle { get; set; }
        public Vehicle VictimVehicle { get; set; }
        public Item AttackerWeapon { get; set; }
        public MapRegion Facility { get; set; }
        */
    }
}
