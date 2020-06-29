using squittal.ScrimPlanetmans.Models.Planetside.Events;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
//using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchScorer
    {
        //int ScoreDeathEvent(ScrimDeathActionEvent death);
        Task<int> ScoreDeathEvent(ScrimDeathActionEvent death);
        //int ScoreVehicleDestructionEvent(ScrimVehicleDestructionActionEvent destruction);
        Task<int> ScoreVehicleDestructionEvent(ScrimVehicleDestructionActionEvent destruction);
        //int ScoreDeathEvent(Death death);
        //int ScoreFacilityControlEvent(FacilityControl control, out bool controlCounts);
        int ScoreFacilityControlEvent(ScrimFacilityControlActionEvent control);
        //int ScorePlayerFacilityCaptureEvent(PlayerFacilityCapture capture);
        //int ScorePlayerFacilityDefendEvent(PlayerFacilityDefend defense);
        //int ScoreGainExperienceEvent(GainExperience expGain);
        void HandlePlayerLogin(PlayerLogin login);
        void HandlePlayerLogout(PlayerLogout login);
        Task SetActiveRuleset();
        //int ScoreReviveEvent(ScrimReviveActionEvent revive);
        Task<int> ScoreReviveEvent(ScrimReviveActionEvent revive);
        //int ScoreAssistEvent(ScrimAssistActionEvent assist);
        Task<int> ScoreAssistEvent(ScrimAssistActionEvent assist);
        //int ScoreObjectiveTickEvent(ScrimObjectiveTickActionEvent objective);
        Task<int> ScoreObjectiveTickEvent(ScrimObjectiveTickActionEvent objective);
    }
}
