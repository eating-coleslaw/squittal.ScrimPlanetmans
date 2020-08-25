namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class JsonRulesetFacilityRule
    {
        public string FacilityName { get; set; }
        public int FacilityId { get; set; }
        public int MapRegionId { get; set; }

        public JsonRulesetFacilityRule()
        {
        }

        public JsonRulesetFacilityRule(RulesetFacilityRule rule)
        {
            FacilityName = rule.MapRegion.FacilityName;
            FacilityId = rule.FacilityId;
            MapRegionId = rule.MapRegionId;
        }
    }
}
