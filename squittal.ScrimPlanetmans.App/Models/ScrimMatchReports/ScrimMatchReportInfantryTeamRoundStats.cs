namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryTeamRoundStats : ScrimMatchReportStats
    {
        public string ScrimMatchId { get; set; }
        public int ScrimMatchRound { get; set; }
        public int TeamOrdinal { get; set; }
        public int FacilityCapturePoints { get; set; }

        public int GrenadeAssists { get; set; }
        public int SpotAssists { get; set; }

        public int GrenadeTeamAssists { get; set; }

        public int Revives { get; set; }
        public int EnemyRevivesAllowed { get; set; }

        public int PeriodicControlTicks { get; set; }
        public int PeriodicControlTickPoints { get; set; }

        public int SecuredKills => Kills - EnemyRevivesAllowed;
        public int ConfirmedDeaths => Deaths - Revives;
    }
}
