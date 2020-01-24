namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusCharacterStatModel
    {
        public string CharacterId { get; set; }
        public string StatName { get; set; }
        public int ProfileId { get; set; }
        public int ValueDaily { get; set; }
        public int ValueWeekly { get; set; }
        public int ValueMonthly { get; set; }
        public int ValueForever { get; set; }
    }
}
