namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class EndRoundCheckerMessage
    {
        public EndRoundReason EndRoundReason { get; set; }
        public int? TeamOrdinal { get; set; }
        public string Info { get; set; } = string.Empty;
    }

    public enum EndRoundReason
    {
        TimeLimit,
        FacilityCapture,
        PointTargetReached
    }
}
