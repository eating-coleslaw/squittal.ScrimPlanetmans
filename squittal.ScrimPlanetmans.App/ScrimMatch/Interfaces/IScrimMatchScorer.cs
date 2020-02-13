using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimMatchScorer
    {
        int ScoreDeathEvent(Death death);
        int ScoreFacilityControlEvent(FacilityControl control);
        int ScorePlayerFacilityCaptureEvent(PlayerFacilityCapture capture);
        int ScorePlayerFacilityDefendEvent(PlayerFacilityDefend defense);
        int ScoreGainExperienceEvent(GainExperience expGain);
    }
}
