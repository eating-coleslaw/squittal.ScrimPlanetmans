using System;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ScrimAction
    {
        [Required]
        public ScrimActionType Action { get; set; }

        [Required]
        public string Name { get; set; } //=> Enum.GetName(typeof(ScrimAction), Action);
        
        public string Description { get; set; }

    }

    public enum ScrimActionType
    {
        // Objectives: 10-19
        FirstBaseCapture = 10,
        SubsequentBaseCapture = 11,
        PointControl = 12,
        PointDefend = 13,

        // Infantry: 20 - 39
        InfantryKillInfantry = 20,
        InfantryKillMax = 21,
        InfantryKillVehicle = 22,
        InfantryTeamkillInfantry = 23,
        InfantryTeamkillMax = 24,
        InfantryTeamkillVehicle = 25,
        InfantrySuicide = 26,
        InfantryDeath = 27,
        InfantryTeamkillDeath = 28,

        // Maxes: 40 - 59
        MaxKillInfantry = 40,
        MaxKillMax = 41,
        MaxKillVehicle = 42,
        MaxTeamkillMax = 43,
        MaxTeamkillInfantry = 44,
        MaxTeamkillVehicle = 45,
        MaxDeath = 46,
        MaxSuicide = 47,
        MaxTeamkillDeath = 48,

        // Support: 60 - 79
        ReviveInfantry = 60,
        ReviveMax = 61,
        InfantryTakeRevive = 62,
        MaxTakeRevive = 63,
        DamageAssist = 64,
        UtilityAssist = 65,

        // Vehicles: 80 - 99        
        VehicleKillInfantry = 80,
        VehicleKillMax = 81,
        VehicleKillVehicle = 82,
        VehicleTeamkillInfantry = 83,
        VehicleTeamkillMax = 84,
        VehicleTeamkillVehicle = 85,
        VehicleSuicide = 86,
        VehicleDeath = 87,
        VehicleTeamkillDeath = 88,
    };
}
