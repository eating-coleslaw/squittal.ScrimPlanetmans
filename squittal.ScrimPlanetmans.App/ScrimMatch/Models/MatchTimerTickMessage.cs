namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class MatchTimerTickMessage
    {
        public MatchTimerStatus MatchTimerStatus { get; set; }

        public string Info { get; set; } = string.Empty;
        
        public MatchTimerTickMessage(MatchTimerStatus matchTimerStatus)
        {
            MatchTimerStatus = matchTimerStatus;
        }
    }
}
