namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class PlanetsideClassRuleSettings
    {
        public bool InfiltratorIsBanned { get; set; } = false;
        public int InfiltratorPoints { get; set; } = 0;

        public bool LightAssaultIsBanned { get; set; } = false;
        public int LightAssaultPoints { get; set; } = 0;

        public bool MedicIsBanned { get; set; } = false;
        public int MedicPoints { get; set; } = 0;

        public bool EngineerIsBanned { get; set; } = false;
        public int EngineerPoints { get; set; } = 0;

        public bool HeavyAssaultIsBanned { get; set; } = false;
        public int HeavyAssaultPoints { get; set; } = 0;

        public bool MaxIsBanned { get; set; } = false;
        public int MaxPoints { get; set; } = 0;

        public PlanetsideClassRuleSettings()
        {
        }

        public PlanetsideClassRuleSettings(RulesetItemCategoryRule rule)
        {
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }

        public PlanetsideClassRuleSettings(RulesetItemRule rule)
        {
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }

        public PlanetsideClassRuleSettings(JsonRulesetItemCategoryRule rule)
        {
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }

        public PlanetsideClassRuleSettings(JsonRulesetItemRule rule)
        {
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }
    }
}
