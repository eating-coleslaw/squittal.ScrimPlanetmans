namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryPlayerStats : ScrimMatchReportStats
    {
        public string ScrimMatchId { get; set; }
        public string CharacterId { get; set; }
        public int TeamOrdinal { get; set; }
        public string NameDisplay { get; set; }
        public string NameFull { get; set; }
        public int FactionId { get; set; }
        public int WorldId { get; set; }
        public int PrestigeLevel { get; set; }
        //public bool IsFromOutfit { get; set; }
        //public string OutfitId { get; set; }
        //public string OutfitTag { get; set; }
        //public bool IsFromConstructedTeam { get; set; }
        //public int? ConstructedTeamId { get; set; }
    }
}
