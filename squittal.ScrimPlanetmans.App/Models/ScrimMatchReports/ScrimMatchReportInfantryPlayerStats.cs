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

        public int RevivesGiven { get; set; }
        public int RevivesTaken { get; set; }
        public int KillsUndoneByRevive { get; set; }
        public int KillsUndoneByRevivePoints { get; set; }

        public int SecuredKills => Kills - KillsUndoneByRevive;
        public int ConfirmedDeaths => Deaths - RevivesTaken;
    }
}
