using squittal.ScrimPlanetmans.Models.Planetside.Events;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchScorer
    {
        Task<int> ScoreDeathEvent(ScrimDeathActionEvent death);
        Task<int> ScoreVehicleDestructionEvent(ScrimVehicleDestructionActionEvent destruction);
        int ScoreFacilityControlEvent(ScrimFacilityControlActionEvent control);
        void HandlePlayerLogin(PlayerLogin login);
        void HandlePlayerLogout(PlayerLogout login);
        Task SetActiveRulesetAsync();
        Task<int> ScoreReviveEvent(ScrimReviveActionEvent revive);
        Task<int> ScoreAssistEvent(ScrimAssistActionEvent assist);
        Task<int> ScoreObjectiveTickEvent(ScrimObjectiveTickActionEvent objective);
    }
}
