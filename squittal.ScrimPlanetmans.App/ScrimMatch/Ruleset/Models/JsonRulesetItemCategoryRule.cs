namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class JsonRulesetItemCategoryRule
    {
        public string ItemCategoryName { get; set; }
        public int ItemCategoryId { get; set; }
        public int Points { get; set; }
        
        public JsonRulesetItemCategoryRule()
        {
        }

        public JsonRulesetItemCategoryRule(RulesetItemCategoryRule rule)
        {
            ItemCategoryName = rule.ItemCategory.Name;
            ItemCategoryId = rule.ItemCategoryId;
            Points = rule.Points;
        }
    }
}
