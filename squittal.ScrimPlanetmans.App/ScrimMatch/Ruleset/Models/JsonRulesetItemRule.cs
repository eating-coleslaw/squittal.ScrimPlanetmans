namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class JsonRulesetItemRule
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Points { get; set; }
        public bool IsBanned { get; set; }

        public JsonRulesetItemRule()
        {
        }

        public JsonRulesetItemRule(RulesetItemRule rule)
        {
            ItemId = rule.ItemId;
            ItemName = rule.Item.Name;
            Points = rule.Points;
            IsBanned = rule.IsBanned;
        }
    }
}
