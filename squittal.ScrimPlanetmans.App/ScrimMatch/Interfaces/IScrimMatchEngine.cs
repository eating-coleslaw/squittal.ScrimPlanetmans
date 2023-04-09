using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchEngine
    {
        MatchConfiguration MatchConfiguration { get; set; }
        Ruleset MatchRuleset { get; }
        
        Task Start();
        Task InitializeNewMatch();
        void ConfigureMatch(MatchConfiguration configuration);
        Task InitializeNewRound();
        void StartRound();
        void PauseRound();
        void ResumeRound();
        Task EndRound();
        Task ResetRound();
        Task ClearMatch(bool isRematch);
        MatchTimerTickMessage GetLatestTimerTickMessage();
        bool IsRunning();
        int GetCurrentRound();
        MatchState GetMatchState();

        void SubmitPlayersList();
        string GetMatchId();
        PeriodicPointsTimerStateMessage GetLatestPeriodicPointsTimerTickMessage();
        ScrimFacilityControlActionEventMessage GetLatestFacilityControlMessage();
        int? GetFacilityControlTeamOrdinal();
    }
}
