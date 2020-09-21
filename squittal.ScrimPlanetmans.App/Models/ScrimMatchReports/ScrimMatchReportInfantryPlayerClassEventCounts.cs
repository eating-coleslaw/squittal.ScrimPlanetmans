using System.Collections.Generic;
using System.Linq;

namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryPlayerClassEventCounts
    {
        public string ScrimMatchId { get; set; }
        public int TeamOrdinal { get; set; }
        public string CharacterId { get; set; }
        public string NameDisplay { get; set; }
        public string NameFull { get; set; }
        public int FactionId { get; set; }
        public int WorldId { get; set; }
        public int PrestigeLevel { get; set; }
        public int EventsAsHeavyAssault { get; set; }
        public int EventsAsInfiltrator { get; set; }
        public int EventsAsLightAssault { get; set; }
        public int EventsAsMedic { get; set; }
        public int EventsAsEngineer { get; set; }
        public int EventsAsMax { get; set; }
        public int KillsAsHeavyAssault { get; set; }
        public int KillsAsInfiltrator { get; set; }
        public int KillsAsLightAssault { get; set; }
        public int KillsAsMedic { get; set; }
        public int KillsAsEngineer { get; set; }
        public int KillsAsMax { get; set; }
        public int DeathsAsHeavyAssault { get; set; }
        public int DeathsAsInfiltrator { get; set; }
        public int DeathsAsLightAssault { get; set; }
        public int DeathsAsMedic { get; set; }
        public int DeathsAsEngineer { get; set; }
        public int DeathsAsMax { get; set; }
        public int DamageAssistsAsHeavyAssault { get; set; }
        public int DamageAssistsAsInfiltrator { get; set; }
        public int DamageAssistsAsLightAssault { get; set; }
        public int DamageAssistsAsMedic { get; set; }
        public int DamageAssistsAsEngineer { get; set; }
        public int DamageAssistsAsMax { get; set; }

        public PlanetsideClass PrimaryPlanetsideClass => GetOrderedPlanetsideClassEventCountsList().Select(c => c.PlanetsideClass).FirstOrDefault();

        public List<PlanetsideClassEventCount> GetOrderedPlanetsideClassEventCountsList()
        {
            var classCountsList = new List<PlanetsideClassEventCount>()
            {
                new PlanetsideClassEventCount(PlanetsideClass.HeavyAssault, EventsAsHeavyAssault),
                new PlanetsideClassEventCount(PlanetsideClass.LightAssault, EventsAsLightAssault),
                new PlanetsideClassEventCount(PlanetsideClass.Infiltrator, EventsAsInfiltrator),
                new PlanetsideClassEventCount(PlanetsideClass.Medic, EventsAsMedic),
                new PlanetsideClassEventCount(PlanetsideClass.Engineer, EventsAsEngineer),
                new PlanetsideClassEventCount(PlanetsideClass.MAX, EventsAsMax)
            };

            return classCountsList.OrderByDescending(c => c.EventCount).ToList();
        }
    }
}
