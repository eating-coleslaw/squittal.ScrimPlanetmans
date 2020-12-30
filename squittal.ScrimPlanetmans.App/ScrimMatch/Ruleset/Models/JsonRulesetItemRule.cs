namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class JsonRulesetItemRule
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
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

        public JsonRulesetItemRule()
        {
        }

        public JsonRulesetItemRule(RulesetItemRule rule)
        {
            ItemId = rule.ItemId;
            ItemName = rule.Item.Name;
            Points = rule.Points;
            IsBanned = rule.IsBanned;

            DeferToPlanetsideClassSettings = rule.DeferToPlanetsideClassSettings;
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
