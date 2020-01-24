namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusZoneModel
    {
        public int ZoneId { get; set; }
        public string Code { get; set; }
        public MultiLanguageString Name { get; set; }
        public int HexSize { get; set; }
        public MultiLanguageString Description { get; set; }
    }
}
