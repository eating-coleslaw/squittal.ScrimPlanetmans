using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
{
    public class ScrimReviveActionEventMessage : ScrimActionEventMessage
    {
        public ScrimReviveActionEvent ReviveEvent { get; set; }

        public ScrimReviveActionEventMessage(ScrimReviveActionEvent reviveEvent)
        {
            ReviveEvent = reviveEvent;

            if (reviveEvent.ActionType == ScrimActionType.OutsideInterference)
            {
                Info = GetOutsideInterferenceInfo(reviveEvent);
            }
            else
            {
                Info = GetReviveInfo(reviveEvent);
            }
        }

        private string GetOutsideInterferenceInfo(ScrimReviveActionEvent reviveEvent)
        {
            var actionDisplay = GetEnumValueName(reviveEvent.ActionType);

            Player player;
            string otherCharacterId;

            if (reviveEvent.MedicPlayer != null)
            {
                player = reviveEvent.MedicPlayer;
                otherCharacterId = reviveEvent.RevivedCharacterId;

                var playerName = player.NameDisplay;
                var outfitDisplay = !string.IsNullOrWhiteSpace(player.OutfitAlias)
                                        ? $"[{player.OutfitAlias}] "
                                        : string.Empty;

                return $"{actionDisplay} REVIVE GIVEN: {outfitDisplay}{playerName} [revived] {otherCharacterId}";
            }
            else
            {
                player = reviveEvent.RevivedPlayer;
                otherCharacterId = reviveEvent.MedicCharacterId;

                var playerName = player.NameDisplay;
                var outfitDisplay = !string.IsNullOrWhiteSpace(player.OutfitAlias)
                                        ? $"[{player.OutfitAlias}] "
                                        : string.Empty;

                return $"{actionDisplay} REVIVED TAKEN: {otherCharacterId} [revived] {outfitDisplay}{playerName}";
            }
        }

        private string GetReviveInfo(ScrimReviveActionEvent reviveEvent)
        {
            var medic = reviveEvent.MedicPlayer;
            var revived = reviveEvent.RevivedPlayer;

            var medicTeam = medic.TeamOrdinal.ToString();

            var medicName = medic.NameDisplay;
            var revivedName = revived.NameDisplay;

            var medicOutfit = !string.IsNullOrWhiteSpace(medic.OutfitAlias)
                                            ? $"[{medic.OutfitAlias}] "
                                            : string.Empty;

            var revivedOutfit = !string.IsNullOrWhiteSpace(revived.OutfitAlias)
                                            ? $"[{revived.OutfitAlias}] "
                                            : string.Empty;

            var actionDisplay = GetEnumValueName(reviveEvent.ActionType);

            return $"Team {medicTeam} {actionDisplay}: {medicOutfit}{medicName} [revived] {revivedOutfit}{revivedName}";
        }
    }
}
