using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.Models
{
    public class VehicleInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<int> Factions { get; set; }
    }
}
