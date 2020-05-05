using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimVehicleDestructionActionEventMessage : ScrimActionEventMessage
    {
        public ScrimVehicleDestructionActionEvent DestructionEvent { get; set; }

        public ScrimVehicleDestructionActionEventMessage(ScrimVehicleDestructionActionEvent destructionEvent)
        {
            DestructionEvent = destructionEvent;

            if (DestructionEvent.ActionType == ScrimActionType.OutsideInterference)
            {
                Info = GetOutsideInterferenceInfo(DestructionEvent);
            }
            else
            {
                switch (DestructionEvent.DeathType)
                {
                    case DeathEventType.Kill:
                        Info = GetKillInfo(DestructionEvent);
                        break;

                    case DeathEventType.Teamkill:
                        Info = GetTeamkillInfo(DestructionEvent);
                        break;

                    case DeathEventType.Suicide:
                        Info = GetSuicideInfo(DestructionEvent);
                        break;
                }
            }
        }

        private string GetOutsideInterferenceInfo(ScrimVehicleDestructionActionEvent DestructionEvent)
        {
            Player player;
            string otherCharacterId;

            var weaponName = DestructionEvent.Weapon != null ? DestructionEvent.Weapon.Name : "Unknown weapon";
            var victimVehicleName = DestructionEvent.VictimVehicle != null ? DestructionEvent.VictimVehicle.Name : "Unknown vehicle";
            var actionDisplay = GetEnumValueName(DestructionEvent.ActionType);

            if (DestructionEvent.AttackerPlayer != null)
            {
                player = DestructionEvent.AttackerPlayer;
                otherCharacterId = DestructionEvent.VictimCharacterId;

                var playerName = player.NameDisplay;
                var outfitDisplay = !string.IsNullOrWhiteSpace(player.OutfitAlias)
                                        ? $"[{player.OutfitAlias}] "
                                        : string.Empty;

                return $"{actionDisplay} VEHICLE DESTROYED: {outfitDisplay}{playerName} <{weaponName}> {victimVehicleName} ({otherCharacterId})";
            }
            else
            {
                player = DestructionEvent.VictimPlayer;
                otherCharacterId = DestructionEvent.AttackerCharacterId;

                var playerName = player.NameDisplay;
                var outfitDisplay = !string.IsNullOrWhiteSpace(player.OutfitAlias)
                                        ? $"[{player.OutfitAlias}] "
                                        : string.Empty;

                return $"{actionDisplay} VEHICLE LOST: {otherCharacterId} <{weaponName}> {victimVehicleName} ({outfitDisplay}{playerName})";
            }
        }

        private string GetKillInfo(ScrimVehicleDestructionActionEvent destructionEvent)
        {
            var attacker = destructionEvent.AttackerPlayer;
            var victim = destructionEvent.VictimPlayer;

            var attackerTeam = attacker.TeamOrdinal.ToString();

            var attackerName = attacker.NameDisplay;

            var victimName = victim != null ? victim.NameDisplay : string.Empty;

            var attackerOutfit = !string.IsNullOrWhiteSpace(attacker.OutfitAlias)
                                            ? $"[{attacker.OutfitAlias}] "
                                            : string.Empty;


            var victimOutfit = (victim != null && !string.IsNullOrWhiteSpace(victim?.OutfitAlias))
                                            ? $"[{victim.OutfitAlias}] "
                                            : string.Empty;

            var actionDisplay = GetEnumValueName(destructionEvent.ActionType);
            var pointsDisplay = GetPointsDisplay(destructionEvent.Points);

            var weaponName = DestructionEvent.Weapon != null ? DestructionEvent.Weapon.Name : "Unknown weapon";
            var victimVehicleName = DestructionEvent.VictimVehicle != null ? DestructionEvent.VictimVehicle.Name : "Unknown vehicle";

            return $"Team {attackerTeam} {actionDisplay}: {pointsDisplay} {attackerOutfit}{attackerName} <{weaponName}> {victimVehicleName} ({victimOutfit}{victimName})";
        }

        private string GetTeamkillInfo(ScrimVehicleDestructionActionEvent destructionEvent)
        {
            return GetKillInfo(destructionEvent);
        }

        private string GetSuicideInfo(ScrimVehicleDestructionActionEvent destructionEvent)
        {
            var attacker = destructionEvent.AttackerPlayer;

            var attackerTeam = attacker.TeamOrdinal.ToString();

            var attackerName = attacker.NameDisplay;

            var attackerOutfit = !string.IsNullOrWhiteSpace(attacker.OutfitAlias)
                                            ? $"[{attacker.OutfitAlias}] "
                                            : string.Empty;

            var actionDisplay = GetEnumValueName(destructionEvent.ActionType);
            var pointsDisplay = GetPointsDisplay(destructionEvent.Points);

            var weaponName = DestructionEvent.Weapon != null ? DestructionEvent.Weapon.Name : "Unknown weapon";
            var victimVehicleName = DestructionEvent.VictimVehicle != null ? DestructionEvent.VictimVehicle.Name : "Unknown vehicle";

            return $"Team {attackerTeam} {actionDisplay}: {pointsDisplay} {attackerOutfit}{attackerName} ({victimVehicleName}) <{weaponName}>";
        }
    }
}
