using System;

namespace squittal.ScrimPlanetmans.Shared.Models
{
    public class ScrimEventAggregate
    {
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Headshots { get; set; } = 0;
        public int HeadshotDeaths { get; set; } = 0;
        public int Suicides { get; set; } = 0;
        public int Teamkills { get; set; } = 0;
        public int TeamkillDeaths { get; set; } = 0;

        public int RevivesGiven { get; set; } = 0;
        public int RevivesTaken { get; set; } = 0;

        public int DamageAssists { get; set; }
        public int UtilityAssists { get; set; }

        public int ObjectiveTicks { get; set; }
        public int BaseCaps { get; set; }


        public double KillDeathRatio
        {
            get
            {
                if (Deaths == 0)
                {
                    return (Kills / 1.0);
                }
                else
                {
                    return Math.Round((double)(Kills / (double)Deaths), 2);
                }
            }
        }

        public double EfficiencyRatio
        {
            get
            {
                if (Deaths == 0)
                {
                    return ((Kills + DamageAssists) / 1.0);
                }
                else
                {
                    return Math.Round((double)((Kills + DamageAssists) / Deaths), 2);
                }
            }
        }

        public double KdaRatio
        {
            get
            {
                if (Deaths == 0)
                {
                    return ((Kills + (DamageAssists / 2)) / 1.0);
                }
                else
                {
                    return Math.Round((double)((Kills + (DamageAssists / 2)) / Deaths), 2);
                }
            }
        }

        public double InGameKillDeathRatio
        {
            get
            {
                if (Deaths == 0)
                {
                    return (Kills / 1.0);
                }
                else
                {
                    return Math.Round((double)(Kills / (Deaths - RevivesTaken)), 2);
                }
            }
        }

        public double HeadshotRatio
        {
            get
            {
                return GetHeadshotRatio(Kills, Headshots);
            }
        }

        public double HeadshotDeathRatio
        {
            get
            {
                return GetHeadshotRatio(Deaths, HeadshotDeaths);
            }
        }

        public bool HasEvents
        {
            get
            {
                return (Kills > 0 || Deaths > 0 || Teamkills > 0);
            }
        }

        private double GetHeadshotRatio(int kills, int headshots)
        {
            if (kills > 0)
            {
                return Math.Round((double)headshots / (double)kills * 100.0, 1);
            }
            else
            {
                return 0;
            }
        }

        public ScrimEventAggregate Add(ScrimEventAggregate addend)
        {
            Kills += addend.Kills;
            Deaths += addend.Deaths;
            Headshots += addend.Headshots;
            HeadshotDeaths += addend.HeadshotDeaths;
            Suicides += addend.Suicides;
            Teamkills += addend.Teamkills;
            TeamkillDeaths += addend.TeamkillDeaths;
            RevivesGiven += addend.RevivesGiven;
            RevivesTaken += addend.RevivesTaken;
            DamageAssists += addend.DamageAssists;
            UtilityAssists += addend.UtilityAssists;
            ObjectiveTicks += addend.ObjectiveTicks;
            BaseCaps += addend.BaseCaps;

            return this;
        }
    }
}
