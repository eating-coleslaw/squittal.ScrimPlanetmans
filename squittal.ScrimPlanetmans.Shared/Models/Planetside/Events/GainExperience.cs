using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside.Events
{
    public class GainExperience
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int ExperienceId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public int Amount { get; set; }
        public int? LoadoutId { get; set; }
        public string OtherId { get; set; }
    }
}
