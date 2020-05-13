using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class VehicleClass
    {
        [Required]
        public VehicleType Class { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public enum VehicleType
    {
        Unknown = 0,
        Flash,
        Harasser,
        ANT,
        Sunderer,
        Lightning,
        MBT,
        Interceptor,
        ESF,
        Valkyrie,
        Liberator,
        Galaxy,
        Bastion
    }
}
