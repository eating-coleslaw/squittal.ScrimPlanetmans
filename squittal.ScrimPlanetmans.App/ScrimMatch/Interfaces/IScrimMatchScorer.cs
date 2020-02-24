using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchScorer
    {
        int ScoreDeathEvent(PlayerScrimDeathEvent death);
        int ScoreDeathEvent(Death death);
        int ScoreFacilityControlEvent(FacilityControl control);
        int ScorePlayerFacilityCaptureEvent(PlayerFacilityCapture capture);
        int ScorePlayerFacilityDefendEvent(PlayerFacilityDefend defense);
        int ScoreGainExperienceEvent(GainExperience expGain);
        void HandlePlayerLogin(PlayerLogin login);
        void HandlePlayerLogout(PlayerLogout login);
        Task SetActiveRuleset();
    }
}
