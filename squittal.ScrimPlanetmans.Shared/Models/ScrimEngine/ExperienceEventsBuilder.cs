using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace squittal.ScrimPlanetmans.Shared.Models
{
    public static class ExperienceEventsBuilder
    {
        public static int[] ReviveIds =
        {
            7,      // Revive (75xp)
            53      // Squad Revive (100xp) 
        };

        public static int[] SpawnIds =
        {
            56,     // Squad Spawn (10xp)
            223    // Sunderer Spawn Bonus (5xp) - DOESN'T RETURN WHO SPAWNED
        };

        public static int[] PointControlIds =
        {
            15,     // Control Point - Defend (100xp) (killing someone on/from point?)
            16,     // Control Point - Attack (100xp) (killing someone on/from point?)
            272,    // Convert Capture Point (25xp)
            556,    // Objective Pulse Defend (50xp)
            557     // Objective Pulse Capture (100xp)
        };

        public static int[] DamageAssistIds =
        {
            2,      // Kill Player Assist (100xp)
            335,    // Savior Kill (Non MAX) (25xp)
            371,    // Kill Player Priority Assist (150xp)
            372     // Kill Player High Priority Assist (300xp)
        };

        public static int[] UtilityAssistIds =
        {
            5,      // Heal Assis (5xp)
            438,    // Shield Repair (10xp)
            439,    // Squad Shield Repair (15xp)
            550,    // Concussion Grenade Assist (50xp)
            551,    // Concussion Grenade Squad Assist (75xp)
            552,    // EMP Grenade Assist (50xp)
            553,    // EMP Grenade Squad Assist (75xp)
            554,    // Flashbang Assist (50xp)
            555,    // Flashbang Squad Assist (75xp)
            1393,   // Hardlight Cover - Blocking Exp (placeholder until code is done) (50xp)
            1394,   // Draw Fire Award (25xp)
            36,     // Spot Kill (20xp)
            54      // Squad Spot Kill (30xp)
        };

        public static int[] BannedIds =
        {
            293,    // Motion Detect (10xp)
            294,    // Squad Motion Spot (15xp)
            593,    // Bounty Kill Bonus (250xp)
            594,    // Bounty Kill Cashed In (400xp)
            594,    // Bounty Kill Cashed In (400xp)
            595,    // Bounty Kill Streak (595xp)
            582     // Kill Assist - Spitfire Turret (25xp)
        };

        public static IEnumerable<int> GetAllExperienceIds()
        {
            var xpIdCount = ReviveIds.Length
                            + PointControlIds.Length
                            + DamageAssistIds.Length
                            + UtilityAssistIds.Length;

            var allIds = new int[xpIdCount];

            int insertIndex = 0;

            Array.Copy(ReviveIds, allIds, ReviveIds.Length);

            insertIndex += ReviveIds.Length;

            Array.Copy(PointControlIds, 0, allIds, insertIndex, PointControlIds.Length);
            
            insertIndex += PointControlIds.Length;

            Array.Copy(DamageAssistIds, 0, allIds, insertIndex, DamageAssistIds.Length);
            
            insertIndex += DamageAssistIds.Length;

            Array.Copy(UtilityAssistIds, 0, allIds, insertIndex, UtilityAssistIds.Length);

            return allIds;
        }

        public static IEnumerable<string> GetExperienceEvents()
        {
            var events = new List<string>();

            var allIds = GetAllExperienceIds();

            events = allIds.Select(id => $"GainExperience_experience_id_{id.ToString()}").ToList();

            return events;
        }

        public static ExperienceType GetExperienceTypeFromId(int experienceId)
        {
            if (ReviveIds.Contains(experienceId))
            {
                return ExperienceType.Revive;
            }
            else if (PointControlIds.Contains(experienceId))
            {
                return ExperienceType.PointControl;
            }
            else if (DamageAssistIds.Contains(experienceId))
            {
                return ExperienceType.DamageAssist;
            }
            else if (UtilityAssistIds.Contains(experienceId))
            {
                return ExperienceType.UtilityAssist;
            }
            else
            {
                return ExperienceType.Unknown;
            }
        }
    }

    public enum ExperienceType
    {
        Revive,
        PointControl,
        DamageAssist,
        UtilityAssist,
        Unknown = 99
    }
}
