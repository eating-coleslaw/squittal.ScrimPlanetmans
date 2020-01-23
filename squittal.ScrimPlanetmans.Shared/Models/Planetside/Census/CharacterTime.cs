using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Shared.Models.Planetside
{
    public class CharacterTime
    {
        [Required]
        public string CharacterId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LastSaveDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int MinutesPlayed { get; set; }

        public Character Character { get; set; }
    }
}
