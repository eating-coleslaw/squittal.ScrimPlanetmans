using squittal.ScrimPlanetmans.Shared.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public abstract class ScrimActionEvent
    {

        public DateTime Timestamp { get; set; }
        public ScrimActionType ActionType { get; set; }

        public int? ZoneId { get; set; }
    }

    public class ScrimDeathActionEvent : ScrimActionEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }
        public ScrimActionWeaponInfo Weapon { get; set; }

        public string AttackerCharacterId { get; set; }
        public string VictimCharacterId { get; set; }

        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? VictimLoadoutId { get; set; }
        public bool IsHeadshot { get; set; }

        public int Points { get; set; }
        public DeathEventType DeathType { get; set; }
    }

    public class ScrimReviveActionEvent : ScrimActionEvent
    {
        public Player MedicPlayer { get; set; }
        public Player RevivedPlayer { get; set; }
        public ScrimActionExperienceGainInfo ExperienceGain { get; set; }

        public int? MedicLoadoutId { get; set; }

        public int Points { get; set; }
    }

    public class ScrimDamageAssistActionEvent : ScrimActionEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }
        public ScrimActionExperienceGainInfo ExperienceGain { get; set; }

        public int? AttackerLoadoutId { get; set; }

        public int Points { get; set; }
    }

    public class ScrimUtilityAssistActionEvent : ScrimActionEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }
        public ScrimActionExperienceGainInfo ExperienceGain { get; set; }

        public int? AttackerLoadoutId { get; set; }

        public int Points { get; set; }
    }

    public class ScrimLoginActionEvent : ScrimActionEvent
    {
        public Player Player { get; set; }

        public ScrimLoginActionEvent()
        {
            ActionType = ScrimActionType.Login;
        }
    }

    public class ScrimLogoutActionEvent : ScrimActionEvent
    {
        public Player Player { get; set; }

        public ScrimLogoutActionEvent()
        {
            ActionType = ScrimActionType.Logout;
        }
    }

    public class ScrimActionWeaponInfo
    {
        public int Id { get; set; }
        public int ItemCategoryId { get; set; }
        public string Name { get; set; }
        public bool IsVehicleWeapon { get; set; }
    }

    public class ScrimActionExperienceGainInfo
    {
        public int Id { get; set; }
        public int Amount { get; set; }
    }
}
