using squittal.ScrimPlanetmans.Shared.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public abstract class PlayerScrimEvent
    {

        public DateTime Timestamp { get; set; }
        public ScrimActionType ActionType { get; set; }

        public int? ZoneId { get; set; }
    }

    public class PlayerScrimDeathEvent : PlayerScrimEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }
        public PlayerScrimWeapon Weapon { get; set; }

        public string AttackerCharacterId { get; set; }
        public string VictimCharacterId { get; set; }

        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? VictimLoadoutId { get; set; }
        public bool IsHeadshot { get; set; }

        public int Points { get; set; }
        public DeathEventType DeathType { get; set; }
    }

    public class PlayerScrimReviveEvent : PlayerScrimEvent
    {
        public Player MedicPlayer { get; set; }
        public Player RevivedPlayer { get; set; }
        public PlayerScrimExperienceGain ExperienceGain { get; set; }

        public int? MedicLoadoutId { get; set; }

        public int Points { get; set; }
    }

    public class PlayerScrimDamageAssistEvent : PlayerScrimEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }
        public PlayerScrimExperienceGain ExperienceGain { get; set; }

        public int? AttackerLoadoutId { get; set; }

        public int Points { get; set; }
    }

    public class PlayerScrimUtilityAssistEvent : PlayerScrimEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }
        public PlayerScrimExperienceGain ExperienceGain { get; set; }

        public int? AttackerLoadoutId { get; set; }

        public int Points { get; set; }
    }

    public class PlayerScrimLoginEvent : PlayerScrimEvent
    {
        public Player Player { get; set; }

        public PlayerScrimLoginEvent()
        {
            ActionType = ScrimActionType.Login;
        }
    }

    public class PlayerScrimLogoutEvent : PlayerScrimEvent
    {
        public Player Player { get; set; }

        public PlayerScrimLogoutEvent()
        {
            ActionType = ScrimActionType.Logout;
        }
    }

    public class PlayerScrimWeapon
    {
        public int Id { get; set; }
        public int ItemCategoryId { get; set; }
        public string Name { get; set; }
    }

    public class PlayerScrimExperienceGain
    {
        public int Id { get; set; }
        public int Amount { get; set; }
    }
}
