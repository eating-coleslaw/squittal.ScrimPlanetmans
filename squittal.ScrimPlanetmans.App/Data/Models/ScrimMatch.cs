using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.Data.Models
{
    public class ScrimMatch
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public string Title { get; set; }
    }
}
