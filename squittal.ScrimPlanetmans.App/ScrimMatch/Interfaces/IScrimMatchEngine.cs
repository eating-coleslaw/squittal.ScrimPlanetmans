using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Events;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchEngine
    {
        MatchConfiguration MatchConfiguration { get; set; }
        
        void Start();
        void InitializeNewMatch();
        void ConfigureMatch(MatchConfiguration configuration);
        void InitializeNewRound();
        void StartRound();
        void PauseRound();
        void ResumeRound();
        void EndRound();
        void ResetRound();
        void ClearMatch();
        MatchTimerTickMessage GetLatestTimerTickMessage();
        void SubmitPlayersList();
    }
}
