using squittal.ScrimPlanetmans.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside.Events
{
    public class Death
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string AttackerCharacterId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public int? CharacterLoadoutId { get; set; }
        public int? AttackerFireModeId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public int? VehicleId { get; set; }
        public bool IsHeadshot { get; set; }
        public bool IsCritical { get; set; }

        // Non-census stream properties
        public int? CharacterFactionId { get; set; }
        public string CharacterOutfitId { get; set; }
        public int? CharacterTeamOrdinal { get; set; }
        public int? AttackerFactionId { get; set; }
        public string AttackerOutfitId { get; set; }
        public int? AttackerTeamOrdinal { get; set; }

        public DeathEventType DeathEventType { get; set; }

        // Navigation Properties
        public Character Character { get; set; }
        public Character AttackerCharacter { get; set; }
        public Faction AttackerFaction { get; set; }
        public Faction CharacterFaction { get; set; }
        public Outfit AttackerOutfit { get; set; }
        public Outfit CharacterOutfit { get; set; }
        public Item AttackerWeapon { get; set; }
    }
}
