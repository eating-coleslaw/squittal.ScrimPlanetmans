namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryPlayerWeaponStats : ScrimMatchReportInfantryWeaponStats
    {
        public string ScrimMatchId { get; set; }
        public string CharacterId { get; set; }
        public int TeamOrdinal { get; set; }
        public string NameDisplay { get; set; }
        public string NameFull { get; set; }
        public int FactionId { get; set; }
        public int PrestigeLevel { get; set; }

        public int WeaponId { get; set; }
        public string WeaponName { get; set; }
        public int WeaponFactionId { get; set; }
    }
}
