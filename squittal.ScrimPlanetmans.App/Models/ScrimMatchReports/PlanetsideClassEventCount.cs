namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class PlanetsideClassEventCount
    {
        public PlanetsideClass PlanetsideClass { get; set; }
        public int EventCount { get; set; }

        public PlanetsideClassEventCount(PlanetsideClass planetsideClass, int eventCount)
        {
            PlanetsideClass = planetsideClass;
            EventCount = eventCount;
        }
    }
}
