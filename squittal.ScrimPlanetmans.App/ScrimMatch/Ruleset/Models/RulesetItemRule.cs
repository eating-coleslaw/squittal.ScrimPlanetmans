using squittal.ScrimPlanetmans.Models.Planetside;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class RulesetItemRule
    {
        [Required]
        public int RulesetId { get; set; }

        [Required]
        public int ItemId { get; set; }

        public int ItemCategoryId { get; set; }

        public int Points { get; set; }

        public bool IsBanned { get; set; }

        public bool DeferToPlanetsideClassSettings { get; set; }

        public bool InfiltratorIsBanned { get; set; }
        public int InfiltratorPoints { get; set; }

        public bool LightAssaultIsBanned { get; set; }
        public int LightAssaultPoints { get; set; }

        public bool MedicIsBanned { get; set; }
        public int MedicPoints { get; set; }

        public bool EngineerIsBanned { get; set; }
        public int EngineerPoints { get; set; }

        public bool HeavyAssaultIsBanned { get; set; }
        public int HeavyAssaultPoints { get; set; }

        public bool MaxIsBanned { get; set; }
        public int MaxPoints { get; set; }

        public Ruleset Ruleset { get; set; }
        public Item Item { get; set; }
        public ItemCategory ItemCategory { get; set; }


        public void SetPlanetsideClassSettings(PlanetsideClassRuleSettings settings)
        {
            InfiltratorIsBanned = settings.InfiltratorIsBanned;
            InfiltratorPoints = settings.InfiltratorPoints;
            LightAssaultIsBanned = settings.LightAssaultIsBanned;
            LightAssaultPoints = settings.LightAssaultPoints;
            MedicIsBanned = settings.MedicIsBanned;
            MedicPoints = settings.MedicPoints;
            EngineerIsBanned = settings.EngineerIsBanned;
            EngineerPoints = settings.EngineerPoints;
            HeavyAssaultIsBanned = settings.HeavyAssaultIsBanned;
            HeavyAssaultPoints = settings.HeavyAssaultPoints;
            MaxIsBanned = settings.MaxIsBanned;
            MaxPoints = settings.MaxPoints;
        }

        public PlanetsideClassRuleSettings GetPlanetsideClassRuleSettings()
        {
            return new PlanetsideClassRuleSettings(this);
        }
    }
}
