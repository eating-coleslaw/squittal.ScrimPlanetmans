using System;

namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryWeaponStats
    {
        public int Points { get; set; }
        public int NetScore { get; set; }
        public int Kills { get; set; }
        public int HeadshotKills { get; set; }
        public int Deaths { get; set; }
        public int HeadshotDeaths { get; set; }

        public int ScoredDeaths { get; set; }
        public int ZeroPointDeaths { get; set; }

        public int DamageAssistedKills { get; set; }
        //public int DamageAssistedOnlyKills { get; set; }
        public int DamageAssistedDeaths { get; set; }
        //public int DamageAssistedOnlyDeaths { get; set; }

        public int GrenadeAssistedKills { get; set; }
        //public int GrenadeAssistedOnlyKills { get; set; }
        public int GrenadeAssistedDeaths { get; set; }
        //public int GrenadeAssistedOnlyDeaths { get; set; }

        //public int SpotAssistedKills { get; set; }
        //public int SpotAssistedOnlyKills { get; set; }
        //public int SpotAssistedDeaths { get; set; }
        //public int SpotAssistedOnlyDeaths { get; set; }

        public int AssistedKills { get; set; }
        public int UnassistedKills { get; set; }
        public int AssistedDeaths { get; set; }
        public int UnassistedDeaths { get; set; }

        public int UnassistedHeadshotKills { get; set; }
        public int UnassistedHeadshotDeaths { get; set; }


        public int OneVsOneCount => UnassistedDeaths + UnassistedKills;

        public int KillDeathEngagementCount => Deaths + Kills;

        public double OneVsOneEngagementPercent
        {
            get
            {
                if (KillDeathEngagementCount > 0)
                {
                    return Math.Round((double)(UnassistedKills + UnassistedDeaths) / KillDeathEngagementCount * 100.0, 0);
                }
                else
                {
                    return Math.Round((UnassistedKills + UnassistedDeaths) / 1.0 * 100.0, 0);
                }
            }
        }

        public double OneVsOneKillDeathRatio
        {
            get
            {
                if (UnassistedDeaths > 0)
                {
                    return Math.Round((double)(UnassistedKills / (double)UnassistedDeaths), 1);
                }
                else
                {
                    return Math.Round(UnassistedKills / 1.0, 1);
                }
            }
        }

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

        public double OneVsOneHeadshotKillRatio
        {
            get
            {
                if (UnassistedDeaths > 0)
                {
                    return Math.Round((double)(UnassistedHeadshotKills / (double)UnassistedDeaths), 1);
                }
                else
                {
                    return Math.Round(UnassistedHeadshotKills / 1.0, 1);
                }
            }
        }

        public double OneVsOneHeadshotDeathRatio
        {
            get
            {
                if (UnassistedDeaths > 0)
                {
                    return Math.Round((double)(UnassistedHeadshotKills / (double)UnassistedDeaths), 1);
                }
                else
                {
                    return Math.Round(UnassistedHeadshotKills / 1.0, 1);
                }
            }
        }
    }
}
