using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
{
    public class ScrimObjectiveTickActionEventMessage : ScrimActionEventMessage
    {
        public ScrimObjectiveTickActionEvent ObjectiveTickEvent { get; set; }

        public ScrimObjectiveTickActionEventMessage(ScrimObjectiveTickActionEvent objectiveTickEvent)
        {
            ObjectiveTickEvent = objectiveTickEvent;

            Info = GetInfo(objectiveTickEvent);
        }

        private string GetInfo(ScrimObjectiveTickActionEvent objectiveTickEvent)
        {
            var attacker = objectiveTickEvent.Player;

            var attackerTeam = attacker.TeamOrdinal.ToString();

            var attackerName = attacker.NameDisplay;

            var attackerOutfit = !string.IsNullOrWhiteSpace(attacker.OutfitAlias)
                                            ? $"[{attacker.OutfitAlias}] "
                                            : string.Empty;

            var actionDisplay = GetEnumValueName(objectiveTickEvent.ActionType);
            var pointsDisplay = GetPointsDisplay(objectiveTickEvent.Points);

            return $"Team {attackerTeam} {actionDisplay}: {pointsDisplay} {attackerOutfit}{attackerName}";
        }
    }
}
