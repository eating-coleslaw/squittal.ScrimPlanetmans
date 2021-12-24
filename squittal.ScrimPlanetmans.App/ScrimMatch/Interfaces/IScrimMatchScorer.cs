using squittal.ScrimPlanetmans.Models.Planetside.Events;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchScorer
    {
        Task<ScrimEventScoringResult> ScoreDeathEvent(ScrimDeathActionEvent death);
        Task<ScrimEventScoringResult> ScoreVehicleDestructionEvent(ScrimVehicleDestructionActionEvent destruction);
        ScrimEventScoringResult ScoreFacilityControlEvent(ScrimFacilityControlActionEvent control);
        void HandlePlayerLogin(PlayerLogin login);
        void HandlePlayerLogout(PlayerLogout login);
        Task SetActiveRulesetAsync();
        Task<ScrimEventScoringResult> ScoreReviveEvent(ScrimReviveActionEvent revive);
        Task<ScrimEventScoringResult> ScoreAssistEvent(ScrimAssistActionEvent assist);
        Task<ScrimEventScoringResult> ScoreObjectiveTickEvent(ScrimObjectiveTickActionEvent objective);
        void ScorePeriodicFacilityControlTick(int controllingTeamOrdinal);
    }
}
