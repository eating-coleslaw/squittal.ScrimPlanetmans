using System;
using System.Collections.Generic;
using System.Linq;

namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryTeamStats
    {
        public string ScrimMatchId { get; set; }
        public int TeamOrdinal { get; set; }
        public int Points { get; set; }
        public int NetScore { get; set; }
        public int Kills { get; set; }
        public int HeadshotKills { get; set; }
        public int Deaths { get; set; }
        public int HeadshotEnemyDeaths { get; set; }
        public int TeamKills { get; set; }
        public int Suicides { get; set; }
        public int ScoredDeaths { get; set; }
        public int ZeroPointDeaths { get; set; }
        public int TeamKillDeaths { get; set; }
        public int DamageAssists { get; set; }
        public int DamageTeamAssists { get; set; }
        public int DamageAssistedKills { get; set; }
        public int DamageAssistedDeaths { get; set; }
        public int DamageAssistedEnemyDeaths { get; set; }
        public int UnassistedEnemyDeaths { get; set; }
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

        public int EventsAsHeavyAssault => DeathsAsHeavyAssault + KillsAsHeavyAssault + DamageAssistsAsHeavyAssault;
        public int EventsAsLightAssault => DeathsAsLightAssault + KillsAsLightAssault + DamageAssistsAsLightAssault;
        public int EventsAsInfiltrator => DeathsAsInfiltrator + KillsAsInfiltrator + DamageAssistsAsInfiltrator;
        public int EventsAsMedic => DeathsAsMedic + KillsAsMedic + DamageAssistsAsMedic;
        public int EventsAsEngineer => DeathsAsEngineer + KillsAsEngineer + DamageAssistsAsEngineer;
        public int EventsAsMax => DeathsAsMax + KillsAsMax + DamageAssistsAsMax;

        public int EnemyDeaths => Deaths - TeamKillDeaths - Suicides;

        public int OneVsOneCount => UnassistedEnemyDeaths + UnassistedKills;

        public int UnassistedKills => (Kills - DamageAssistedKills);

        public double OneVsOneRatio
        {
            get
            {
                if (OneVsOneCount > 0)
                {
                    return Math.Round((double)(UnassistedKills / (double)UnassistedEnemyDeaths), 2);
                }
                else
                {
                    return UnassistedKills / 1.0;
                }
            }
        }

        public double HeadshotPercent
        {
            get
            {
                if (Kills > 0)
                {
                    return Math.Round((double)(HeadshotKills / (double)Kills) * 100, 1);
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public double HeadshotEnemyDeathPercent
        {
            get
            {
                if (Deaths > 0)
                {
                    return Math.Round((double)(HeadshotEnemyDeaths / (double)EnemyDeaths) * 100, 1);
                }
                else
                {
                    return 0.0;
                }
            }
        }

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
