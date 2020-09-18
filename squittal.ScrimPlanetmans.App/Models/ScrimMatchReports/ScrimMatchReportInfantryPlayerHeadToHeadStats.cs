namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryPlayerHeadToHeadStats : ScrimMatchReportInfantryHeadToHeadStats
    {
        public string ScrimMatchId { get; set; }
        public int PlayerTeamOrdinal { get; set; }
        public string PlayerCharacterId { get; set; }
        public string PlayerNameDisplay { get; set; }
        public string PlayerNameFull { get; set; }
        public int PlayerFactionId { get; set; }
        public int PlayerPrestigeLevel { get; set; }
        public int OpponentTeamOrdinal { get; set; }
        public string OpponentCharacterId { get; set; }
        public string OpponentNameDisplay { get; set; }
        public string OpponentNameFull { get; set; }
        public int OpponentFactionId { get; set; }
        public int OpponentPrestigeLevel { get; set; }
    }
}
