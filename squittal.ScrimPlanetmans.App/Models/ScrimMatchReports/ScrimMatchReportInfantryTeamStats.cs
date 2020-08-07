namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryTeamStats : ScrimMatchReportStats
    {
        public string ScrimMatchId { get; set; }
        public int TeamOrdinal { get; set; }
        public int PointAdjustments { get; set; }
        public int FacilityCapturePoints { get; set; }
    }
}
