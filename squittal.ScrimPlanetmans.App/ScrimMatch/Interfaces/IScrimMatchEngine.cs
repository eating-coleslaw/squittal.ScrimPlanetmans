using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchEngine
    {
        MatchConfiguration MatchConfiguration { get; set; }
        Ruleset MatchRuleset { get; }
        
        //void Start();
        Task Start();
        Task InitializeNewMatch();
        //void InitializeNewMatch();
        void ConfigureMatch(MatchConfiguration configuration);
        Task InitializeNewRound();
        void StartRound();
        void PauseRound();
        void ResumeRound();
        //void EndRound();
        Task EndRound();
        //void ResetRound();
        Task ResetRound();
        //void ClearMatch();
        Task ClearMatch();
        MatchTimerTickMessage GetLatestTimerTickMessage();
        bool IsRunning();
        int GetCurrentRound();
        MatchState GetMatchState();

        void SubmitPlayersList();
        string GetMatchId();
    }
}
