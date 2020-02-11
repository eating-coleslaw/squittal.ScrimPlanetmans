using squittal.ScrimPlanetmans.ScrimMatch.Events;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchEngine
    {
        void InitializeNewMatch();
        void ConfigureMatch();
        void InitializeNewRound();
        void StartRound();
        void PauseRound();
        void ResumeRound();
        void EndRound();
        void ResetRound();
        void ClearMatch();
        MatchTimerTickMessage GetLatestTimerTickMessage();
    }
}
