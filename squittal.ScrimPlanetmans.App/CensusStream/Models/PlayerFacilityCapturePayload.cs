namespace squittal.ScrimPlanetmans.CensusStream.Models
{
    public class PlayerFacilityCapturePayload : PayloadBase
    {
        public string CharacterId { get; set; }
        public int FacilityId { get; set; }
        public string OutfitId { get; set; }
    }
}
