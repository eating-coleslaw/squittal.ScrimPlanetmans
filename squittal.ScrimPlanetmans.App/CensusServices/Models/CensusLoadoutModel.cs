using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusLoadoutModel
    {
        public int LoadoutId { get; set; }
        public int ProfileId { get; set; }
        public int FactionId { get; set; }
        public string CodeName { get; set; }
    }
}
