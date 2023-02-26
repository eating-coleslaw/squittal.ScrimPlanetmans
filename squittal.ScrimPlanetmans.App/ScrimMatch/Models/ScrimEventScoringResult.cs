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

    public class ScrimEventScoringReviveResult : ScrimEventScoringResult
    {
        public ScrimEventScoringResult Result { get; set; }
        public ScrimEventScoringResult EnemyResult { get; set; }
        public ScrimActionType EnemyActionType { get; set; }
        public Player LastKilledByPlayer { get; set; }

        public ScrimEventScoringReviveResult(ScrimEventScoringResult medicResult, ScrimEventScoringResult enemyResult, ScrimActionType enemyActionType, Player lastKilledByPlayer)
        {
            Result = medicResult;
            EnemyResult = enemyResult;
            EnemyActionType = enemyActionType;
            LastKilledByPlayer = lastKilledByPlayer;
        }
    }
}
