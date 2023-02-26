using squittal.ScrimPlanetmans.Models.Planetside;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public abstract class ScrimActionEvent
    {

        public DateTime Timestamp { get; set; }
        public ScrimActionType ActionType { get; set; }
        public bool IsBanned { get; set; } = false;

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

    public class ScrimExperienceGainActionEvent : ScrimActionEvent
    {
        public ScrimActionExperienceGainInfo ExperienceGainInfo { get; set; }

        public ExperienceType ExperienceType { get; set; }

        public int? LoadoutId { get; set; }

        public int Points { get; set; }
    }

    public class ScrimReviveActionEvent : ScrimExperienceGainActionEvent
    {
        public Player MedicPlayer { get; set; }
        public Player RevivedPlayer { get; set; }
        public Player LastKilledByPlayer { get; set; }

        public string MedicCharacterId { get; set; }
        public string RevivedCharacterId { get; set; }
        public string LastKilledByCharacterId { get; set; }

        public int EnemyPoints { get; set; }
        public ScrimActionType EnemyActionType { get; set; }

        public ScrimReviveActionEvent(ScrimExperienceGainActionEvent baseExperienceEvent)
        {
            Timestamp = baseExperienceEvent.Timestamp;
            ZoneId = baseExperienceEvent.ZoneId;
            ExperienceGainInfo = baseExperienceEvent.ExperienceGainInfo;
            ExperienceType = baseExperienceEvent.ExperienceType;
            LoadoutId = baseExperienceEvent.LoadoutId;
        }
    }

    public class ScrimAssistActionEvent : ScrimExperienceGainActionEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }

        public string AttackerCharacterId { get; set; }
        public string VictimCharacterId { get; set; }

        public ScrimAssistActionEvent(ScrimExperienceGainActionEvent baseExperienceEvent)
        {
            Timestamp = baseExperienceEvent.Timestamp;
            ZoneId = baseExperienceEvent.ZoneId;
            ExperienceGainInfo = baseExperienceEvent.ExperienceGainInfo;
            ExperienceType = baseExperienceEvent.ExperienceType;
            LoadoutId = baseExperienceEvent.LoadoutId;
        }
    }

    public class ScrimUtilityAssistActionEvent : ScrimExperienceGainActionEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }

        public string AttackerCharacterId { get; set; }
        public string VictimCharacterId { get; set; }

        public ScrimUtilityAssistActionEvent(ScrimExperienceGainActionEvent baseExperienceEvent)
        {
            Timestamp = baseExperienceEvent.Timestamp;
            ZoneId = baseExperienceEvent.ZoneId;
            ExperienceGainInfo = baseExperienceEvent.ExperienceGainInfo;
            ExperienceType = baseExperienceEvent.ExperienceType;
            LoadoutId = baseExperienceEvent.LoadoutId;
        }
    }

    public class ScrimObjectiveTickActionEvent : ScrimExperienceGainActionEvent
    {
        public Player Player { get; set; }

        public string PlayerCharacterId { get; set; }

        // TODO: Experience IDs of 15 & 16 (Control Point - Attack / Defend seem to populate other_id with
        // an opposing player, but not sure what it means at the moment
        // public Player OtherPlayer { get; set; } 

        //public ScrimActionExperienceGainInfo ExperienceGain { get; set; }

        //public int? LoadoutId { get; set; }

        //public int Points { get; set; }
        public ScrimObjectiveTickActionEvent(ScrimExperienceGainActionEvent baseExperienceEvent)
        {
            Timestamp = baseExperienceEvent.Timestamp;
            ZoneId = baseExperienceEvent.ZoneId;
            ExperienceGainInfo = baseExperienceEvent.ExperienceGainInfo;
            ExperienceType = baseExperienceEvent.ExperienceType;
            LoadoutId = baseExperienceEvent.LoadoutId;
        }
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

    public class ScrimFacilityControlActionEvent : ScrimActionEvent
    {
        public int FacilityId { get; set; }
        public int WorldId { get; set; }
        public string FacilityName { get; set; } = string.Empty;

        public FacilityControlType ControlType { get; set; }
        public int ControllingTeamOrdinal { get; set; }
        
        public int Points { get; set; }


        public int? NewFactionId { get; set; }
        public int? OldFactionId { get; set; }
        public int DurationHeld { get; set; }
        public string OutfitId { get; set; }
    }

    public class ScrimActionWeaponInfo
    {
        public int Id { get; set; }
        public int? ItemCategoryId { get; set; }
        public string Name { get; set; } = "Unknown weapon";
        public bool IsVehicleWeapon { get; set; } = false;
    }

    public class ScrimActionExperienceGainInfo
    {
        public int Id { get; set; }
        public int Amount { get; set; }
    }

    public class ScrimVehicleDestructionActionEvent : ScrimActionEvent
    {
        public Player AttackerPlayer { get; set; }
        public Player VictimPlayer { get; set; }

        public ScrimActionWeaponInfo Weapon { get; set; }

        public ScrimActionVehicleInfo AttackerVehicle { get; set; }
        public ScrimActionVehicleInfo VictimVehicle { get; set; }

        public string AttackerCharacterId { get; set; }
        public string VictimCharacterId { get; set; }

        public int? AttackerLoadoutId { get; set; }

        public int? VictimFactionId { get; set; }

        public int Points { get; set; }
        public DeathEventType DeathType { get; set; }
    }

    public class ScrimActionVehicleInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public VehicleType Type { get; set; }

        public ScrimActionVehicleInfo(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return;
            }

            Id = vehicle.Id;
            Name = vehicle.Name;

            Type = GetVehicleType(vehicle.Id);
        }

        public static VehicleType GetVehicleType(int vehicleId)
        {
            if (vehicleId == 1 || vehicleId == 2010 || vehicleId == 2125)
            {
                return VehicleType.Flash;
            }
            else if (vehicleId == 2)
            {
                return VehicleType.Sunderer;
            }
            else if (vehicleId == 3)
            {
                return VehicleType.Lightning;
            }
            else if (vehicleId == 4 || vehicleId == 5 || vehicleId == 6)
            {
                return VehicleType.MBT;
            }
            else if (vehicleId == 7 || vehicleId == 8 || vehicleId == 9)
            {
                return VehicleType.ESF;
            }
            else if (vehicleId == 10)
            {
                return VehicleType.Liberator;
            }
            else if (vehicleId == 11)
            {
                return VehicleType.Galaxy;
            }
            else if (vehicleId == 12)
            {
                return VehicleType.Harasser;
            }
            else if (vehicleId == 14)
            {
                return VehicleType.Valkyrie;
            }
            else if (vehicleId == 15)
            {
                return VehicleType.ANT;
            }
            else if (vehicleId == 2019)
            {
                return VehicleType.Bastion;
            }
            else if (vehicleId == 2122 || vehicleId == 2123 || vehicleId == 2124)
            {
                return VehicleType.Interceptor;
            }
            else
            {
                return VehicleType.Unknown;
            }
        }
    }
}
