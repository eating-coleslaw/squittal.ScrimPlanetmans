namespace squittal.ScrimPlanetmans.CensusStream.Models
{
    public class FacilityControlPayload : PayloadBase
    {
        public int FacilityId { get; set; }
        public int NewFactionId { get; set; }
        public int OldFactionId { get; set; }
        public int DurationHeld { get; set; }
        public string OutfitId { get; set; }
    }
}
