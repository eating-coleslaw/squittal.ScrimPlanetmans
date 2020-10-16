namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ScrimEventScoringResult
    {
        public ScrimEventScorePointsSource PointsSource { get; set; }

        public int Points { get; set; }

        public bool IsBanned { get; set; }

        public ScrimEventScoringResult()
        {
        }

        public ScrimEventScoringResult(ScrimEventScorePointsSource pointsSource, int points, bool isBanned)
        {
            PointsSource = pointsSource;
            Points = points;
            IsBanned = isBanned;
        }
    }
}
