using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportStats
    {
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
        public int TrickleDeaths { get; set; }

        public int DamageAssists { get; set; }

        public int DamageAssistedKills { get; set; }
        public int DamageAssistedOnlyKills { get; set; }
        public int GrenadeAssistedKills { get; set; }
        public int GrenadeAssistedOnlyKills { get; set; }
        public int SpotAssistedKills { get; set; }
        public int SpotAssistedOnlyKills { get; set; }
        public int AssistedKills { get; set; } // Does not include Spot-Assisted Kills

        public int DamageTeamAssists { get; set; }

        public int DamageAssistedDeaths { get; set; }
        public int DamageAssistedOnlyDeaths { get; set; }
        public int GrenadeAssistedDeaths { get; set; }
        public int GrenadeAssistedOnlyDeaths { get; set; }
        public int SpotAssistedDeaths { get; set; }
        public int SpotAssistedOnlyDeaths { get; set; }

        public int AssistedDeaths => DamageAssistedDeaths + GrenadeAssistedDeaths;
        public double WeightedAssistedDeaths => DamageAssistedDeaths + GrenadeAssistedDeaths + (SpotAssistedDeaths * 0.25);

        public int DamageAssistedEnemyDeaths { get; set; }
        public int DamageAssistedOnlyEnemyDeaths { get; set; }
        public int GrenadeAssistedEnemyDeaths { get; set; }
        public int GrenadeAssistedOnlyEnemyDeaths { get; set; }
        public int SpotAssistedEnemyDeaths { get; set; }
        public int SpotAssistedOnlyEnemyDeaths { get; set; }
        public int UnassistedEnemyDeaths { get; set; }

        public int KillDamageDealt { get; set; }
        public int AssistDamageDealt { get; set; }
        public int TotalDamageDealt { get; set; }

        public int PostReviveKills { get; set; }
        public int ReviveInstantDeaths { get; set; }
        public int ReviveLivesMoreThan15s { get; set; }
        public int ShortestRevivedLifeSeconds { get; set; }
        public int LongestRevivedLifeSeconds { get; set; }
        public int AvgRevivedLifeSeconds { get; set; }

        #region Class-Specific Stats
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
        #endregion Class-Specific Stats

        public int EnemyDeaths => Deaths - TeamKillDeaths - Suicides;


        public int OneVsOneCount => UnassistedEnemyDeaths + UnassistedKills;

        //public int UnassistedKills => (Kills - DamageAssistedKills);
        public int UnassistedKills => (Kills - AssistedKills);

        public int EnemyKillDeathEngagementCount => EnemyDeaths + Kills;

        public int EnemyEngagementCount => EnemyKillDeathEngagementCount + DamageAssists;

        public double WeightedEnemyEngagementCount => EnemyKillDeathEngagementCount + (DamageAssists / 2.0);

        public double UnfavorableEngagementPercent
        {
            get
            {
                if (EnemyEngagementCount > 0)
                {
                    return Math.Round((1 - ((UnassistedKills + DamageAssists) / (double)EnemyEngagementCount)) * 100, 0);
                }
                else
                {
                    return Math.Round((1 - ((UnassistedKills + DamageAssists) / (double)1.0)) * 100.0, 0);
                }
            }
        }

        public double FavorableEngagementCount =>
                        UnassistedKills
                        + (UnassistedEnemyDeaths - (ReviveInstantDeaths * 0.5))
                        + AssistedKills
                        + (SpotAssistedOnlyKills * 0.25)
                        + ((DamageAssists - WeightedAssistedDeaths) / 2);

        public double WeightedFavorableEngagementPercent3
        {
            get
            {
                if (EnemyEngagementCount > 0 && FavorableEngagementCount >= 0)
                {
                    return (FavorableEngagementCount >= WeightedEnemyEngagementCount) ? 100.0
                                : Math.Round(FavorableEngagementCount / WeightedEnemyEngagementCount * 100, 0);
                }
                else if (FavorableEngagementCount < 0)
                {
                    return Math.Round(0 * 100.0, 0);
                }
                else
                {
                    return Math.Round(FavorableEngagementCount * 100.0, 0);
                }
            }
        }

        public double FavorableEngagementPercent
        {
            get
            {
                if (EnemyEngagementCount > 0)
                {
                    return Math.Round((UnassistedKills + UnassistedEnemyDeaths + (DamageAssists - DamageAssistedEnemyDeaths)) / (double)EnemyEngagementCount * 100, 0);
                }
                else
                {
                    return Math.Round(((UnassistedKills + UnassistedEnemyDeaths + (DamageAssists - DamageAssistedEnemyDeaths)) / (double)1.0) * 100.0, 0);
                }
            }
        }

        public double WeightedFavorableEngagementPercent
        {
            get
            {
                if (EnemyEngagementCount > 0)
                {
                    return Math.Round((double)(UnassistedKills + UnassistedEnemyDeaths + ((DamageAssists - DamageAssistedEnemyDeaths)) / 2.0) / WeightedEnemyEngagementCount * 100, 0);
                }
                else
                {
                    return Math.Round(((UnassistedKills + UnassistedEnemyDeaths + ((DamageAssists - DamageAssistedEnemyDeaths)) / 2.0) / (double)1.0) * 100.0, 0);
                }
            }
        }

        public double WeightedFavorableEngagementPercent2
        {
            get
            {
                if (EnemyEngagementCount > 0)
                {
                    return Math.Round((double)(UnassistedKills + UnassistedEnemyDeaths + ((((DamageAssists + DamageAssistedKills) / 2.0) - DamageAssistedEnemyDeaths) / 2.0)) / WeightedEnemyEngagementCount * 100, 0);
                }
                else
                {
                    return Math.Round(((UnassistedKills + UnassistedEnemyDeaths + ((((DamageAssists + DamageAssistedKills) / 2.0) - DamageAssistedEnemyDeaths) / 2.0)) / (double)1.0) * 100.0, 0);
                }
            }
        }

        public double OneVsOneEngagementPercent
        {
            get
            {
                if (EnemyKillDeathEngagementCount > 0)
                {
                    return Math.Round((double)(UnassistedKills + UnassistedEnemyDeaths) / EnemyKillDeathEngagementCount * 100.0, 0);
                }
                else
                {
                    return Math.Round((UnassistedKills + UnassistedEnemyDeaths) / 1.0 * 100.0, 0);
                }
            }
        }

        public double OneVsOneKillDeathRatio
        {
            get
            {
                if (UnassistedEnemyDeaths > 0)
                {
                    return Math.Round((double)(UnassistedKills / (double)UnassistedEnemyDeaths), 1);
                }
                else
                {
                    return Math.Round(UnassistedKills / 1.0, 1);
                }
            }
        }

        public double HeadshotPercent
        {
            get
            {
                if (Kills > 0)
                {
                    return Math.Round((double)(HeadshotKills / (double)Kills) * 100, 0);
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
                    return Math.Round((double)(HeadshotEnemyDeaths / (double)EnemyDeaths) * 100, 0);
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
            //return classCountsList.OrderBy(c => c.EventCount).ToList();
        }
    }
}
