namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusProfileModel
    {
        public int ProfileId { get; set; }
        public int ProfileTypeId { get; set; }
        public int FactionId { get; set; }
        public MultiLanguageString Name { get; set; }
        public int ImageId { get; set; }
    }
}
