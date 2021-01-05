namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ScrimSeriesMatchResult
    {
        public int MatchNumber { get; set; }
        public ScrimSeriesMatchResultType ResultType { get; set; }

        public ScrimSeriesMatchResult(int seriesMatchNumber, ScrimSeriesMatchResultType matchResultType)
        {
            MatchNumber = seriesMatchNumber;
            ResultType = matchResultType;
        }
    }

    public enum ScrimSeriesMatchResultType
    {
        Win,
        Loss,
        Draw
    }
}
