namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusVehicleModel
    {
        public int VehicleId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int Cost { get; set; }
        public int CostResourceId { get; set; }
    }
}
