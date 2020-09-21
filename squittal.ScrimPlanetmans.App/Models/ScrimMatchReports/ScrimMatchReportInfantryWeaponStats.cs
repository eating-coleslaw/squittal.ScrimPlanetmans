using System;

namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryWeaponStats
    {
        public int Points { get; set; }
        public int Kills { get; set; }
        public int HeadshotKills { get; set; }
        public int Deaths { get; set; }
        public int HeadshotDeaths { get; set; }

        public int Teamkills { get; set; }
        public int TeamkillDeaths { get; set; }
        public int Suicides { get; set; }

        public int ScoredKills { get; set; }
        public int ZeroPointKills { get; set; }

        public int ScoredDeaths { get; set; }
        public int ZeroPointDeaths { get; set; }

        public int DamageAssistedKills { get; set; }
        public int DamageAssistedDeaths { get; set; }

        public int GrenadeAssistedKills { get; set; }
        public int GrenadeAssistedDeaths { get; set; }

        public int AssistedKills { get; set; }
        public int UnassistedKills { get; set; }
        public int AssistedDeaths { get; set; }
        public int UnassistedDeaths { get; set; }

        public int UnassistedHeadshotKills { get; set; }
        public int UnassistedHeadshotDeaths { get; set; }

        public int HeadshotTeamkills { get; set; }
        public int EnemyDeaths { get; set; }
        public int HeadshotEnemyDeaths { get; set; }


        public int DamageAssistedEnemyDeaths { get; set; }

        public int GrenadeAssistedEnemyDeaths { get; set; }

        public int AssistedEnemyDeaths { get; set; }
        public int UnassistedEnemyDeaths { get; set; }

        public int UnassistedHeadshotEnemyDeaths { get; set; }


        public int KillDeathEngagementCount => Deaths + Kills;

        public double HeadshotKillPercent
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

        public double HeadshotDeathPercent
        {
            get
            {
                if (Deaths > 0)
                {
                    return Math.Round((double)(HeadshotDeaths / (double)Deaths) * 100, 0);
                }
                else
                {
                    return 0.0;
                }
            }
        }
    }
}
