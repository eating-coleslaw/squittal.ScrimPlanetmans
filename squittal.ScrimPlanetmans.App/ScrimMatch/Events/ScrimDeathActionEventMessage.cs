using squittal.ScrimPlanetmans.Models.MessageLogs;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimDeathActionEventMessage : ScrimActionEventMessage
    {
        public ScrimDeathActionEvent DeathEvent { get; set; }

        public ScrimDeathActionEventMessage(ScrimDeathActionEvent deathEvent)
        {
            DeathEvent = deathEvent;

            Timestamp = deathEvent.Timestamp;

            if (deathEvent.ActionType == ScrimActionType.OutsideInterference)
            {
                LogLevel = ScrimMessageLogLevel.MatchEventWarning;

                Info = GetOutsideInterferenceInfo(deathEvent);
            }
            else
            {
                LogLevel = deathEvent.IsBanned ? ScrimMessageLogLevel.MatchEventRuleBreak : ScrimMessageLogLevel.MatchEventMajor;

                switch (deathEvent.DeathType)
                {
                    case DeathEventType.Kill:
                        Info = GetKillInfo(deathEvent);
                        break;

                    case DeathEventType.Teamkill:
                        Info = GetTeamkillInfo(deathEvent);
                        break;

                    case DeathEventType.Suicide:
                        Info = GetSuicideInfo(deathEvent);
                        break;
                }
            }
        }

        private string GetOutsideInterferenceInfo(ScrimDeathActionEvent deathEvent)
        {
            Player player;
            string otherCharacterId;

            var weaponName = deathEvent.Weapon != null ? deathEvent.Weapon.Name : "Unknown weapon";
            var actionDisplay = GetEnumValueName(deathEvent.ActionType);

            if (deathEvent.AttackerPlayer != null)
            {
                player = deathEvent.AttackerPlayer;
                otherCharacterId = deathEvent.VictimCharacterId;

                var playerName = player.NameDisplay;
                var outfitDisplay = !string.IsNullOrWhiteSpace(player.OutfitAlias)
                                        ? $"[{player.OutfitAlias}] "
                                        : string.Empty;

                return $"{actionDisplay} KILL: {outfitDisplay}{playerName} {{{weaponName}}} {otherCharacterId}";
            }
            else
            {
                player = deathEvent.VictimPlayer;
                otherCharacterId = deathEvent.AttackerCharacterId;

                var playerName = player.NameDisplay;
                var outfitDisplay = !string.IsNullOrWhiteSpace(player.OutfitAlias)
                                        ? $"[{player.OutfitAlias}] "
                                        : string.Empty;

                return $"{actionDisplay} DEATH: {otherCharacterId} {{{weaponName}}} {outfitDisplay}{playerName}";
            }
        }

        private string GetKillInfo(ScrimDeathActionEvent deathEvent)
        {
            var attacker = deathEvent.AttackerPlayer;
            var victim = deathEvent.VictimPlayer;

            var attackerTeam = attacker.TeamOrdinal.ToString();

            var attackerName = attacker.NameDisplay;
            var victimName = victim.NameDisplay;

            var attackerOutfit = !string.IsNullOrWhiteSpace(attacker.OutfitAlias)
                                            ? $"[{attacker.OutfitAlias}] "
                                            : string.Empty;

            var victimOutfit = !string.IsNullOrWhiteSpace(victim.OutfitAlias)
                                            ? $"[{victim.OutfitAlias}] "
                                            : string.Empty;

            var actionDisplay = GetEnumValueName(deathEvent.ActionType);
            var pointsDisplay = GetPointsDisplay(deathEvent.Points);
            var weaponName = deathEvent.Weapon != null ? deathEvent.Weapon.Name : "Unknown weapon";
            var headshot = deathEvent.IsHeadshot ? "_" : "^";

            var bannedDisplay = deathEvent.IsBanned ? "RULE BREAK - " : string.Empty;

            return $"{bannedDisplay}Team {attackerTeam} {actionDisplay}: {pointsDisplay} {attackerOutfit}{attackerName} {headshot} {{{weaponName}}} {victimOutfit}{victimName}";
        }

        private string GetTeamkillInfo(ScrimDeathActionEvent deathEvent)
        {
            return GetKillInfo(deathEvent);
        }

        private string GetSuicideInfo(ScrimDeathActionEvent deathEvent)
        {
            var attacker = deathEvent.AttackerPlayer;

            var attackerTeam = attacker.TeamOrdinal.ToString();

            var attackerName = attacker.NameDisplay;

            var attackerOutfit = !string.IsNullOrWhiteSpace(attacker.OutfitAlias)
                                            ? $"[{attacker.OutfitAlias}] "
                                            : string.Empty;

            var actionDisplay = GetEnumValueName(deathEvent.ActionType);
            var pointsDisplay = GetPointsDisplay(deathEvent.Points);
            var weaponName = deathEvent.Weapon != null ? deathEvent.Weapon.Name : "Unknown weapon";

            var bannedDisplay = deathEvent.IsBanned ? "RULE BREAK - " : string.Empty;

            return $"{bannedDisplay}Team {attackerTeam} {actionDisplay}: {pointsDisplay} {attackerOutfit}{attackerName} {{{weaponName}}}";
        }

        //private string GetEnumValueName(ScrimActionType action)
        //{
        //    return Enum.GetName(typeof(ScrimActionType), action);
        //}

        //private string GetPointsDisplay(int points)
        //{
        //    if (points >= 0)
        //    {
        //        return $"+{points.ToString()}";
        //    }
        //    else
        //    {
        //        return $"{points.ToString()}";
        //    }
        //}
    }
}
