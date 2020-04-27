using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ScrimEventAggregate
    {
        public int Points { get; set; } = 0;
        public int NetScore { get; set; } = 0;

        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Headshots { get; set; } = 0;
        public int HeadshotDeaths { get; set; } = 0;
        public int Suicides { get; set; } = 0;
        public int Teamkills { get; set; } = 0;
        public int TeamkillDeaths { get; set; } = 0;

        public int RevivesGiven { get; set; } = 0;
        public int RevivesTaken { get; set; } = 0;

        public int DamageAssists { get; set; } = 0;
        public int UtilityAssists { get; set; } = 0;
        public int Assists
        {
            get
            {
                return DamageAssists + UtilityAssists;
            }
        }

        public int DamageAssistedDeaths { get; set; } = 0;
        public int UtilityAssistedDeaths { get; set; } = 0;

        public int ObjectiveCaptureTicks { get; set; } = 0;
        public int ObjectiveDefenseTicks { get; set; } = 0;
        public int ObjectiveTicks
        {
            get
            {
                return ObjectiveCaptureTicks + ObjectiveDefenseTicks;
            }
        }
        public int BaseDefenses { get; set; } = 0;
        public int BaseCaptures { get; set; } = 0;
        public int BaseControlVictories
        {
            get
            {
                return BaseDefenses + BaseCaptures;
            }
        }

        public FacilityControlType PreviousScoredBaseControlType { get; set; } = FacilityControlType.Unknown;

        
        public int VehiclesDestroyed { get; set; } = 0;
        public int VehiclesLost { get; set; } = 0;

        #region Air Vehicle Stats
        public int InterceptorsDestroyed { get; set; } = 0;
        public int InterceptorsLost { get; set; } = 0;
        public int EsfsDestroyed { get; set; } = 0;
        public int EsfsLost { get; set; } = 0;
        public int ValkyriesDestroyed { get; set; } = 0;
        public int ValkyriesLost { get; set; } = 0;
        public int LiberatorsDestroyed { get; set; } = 0;
        public int LiberatorsLost { get; set; } = 0;
        public int GalaxiesDestroyed { get; set; } = 0;
        public int GalaxiesLost { get; set; } = 0;
        public int BastionsDestroyed { get; set; } = 0;
        public int BastionsLost { get; set; } = 0;
        #endregion Air Vehicle Stats

        public int Events
        {
            get
            {
                return (Kills + Deaths + RevivesGiven + RevivesTaken + Assists + ObjectiveTicks);
            }
        }

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
            Points += addend.Points;
            NetScore += addend.NetScore;
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
            DamageAssistedDeaths += addend.DamageAssistedDeaths;
            UtilityAssistedDeaths += addend.UtilityAssistedDeaths;

            VehiclesDestroyed += addend.VehiclesDestroyed;
            VehiclesLost += addend.VehiclesLost;

            InterceptorsDestroyed += addend.InterceptorsDestroyed;
            InterceptorsLost += addend.InterceptorsLost;
            EsfsDestroyed += addend.EsfsDestroyed;
            EsfsLost += addend.EsfsLost;
            ValkyriesDestroyed += addend.ValkyriesDestroyed;
            ValkyriesLost += addend.ValkyriesLost;
            LiberatorsDestroyed += addend.LiberatorsDestroyed;
            LiberatorsLost += addend.LiberatorsLost;
            GalaxiesDestroyed += addend.GalaxiesDestroyed;
            GalaxiesLost += addend.GalaxiesLost;
            BastionsDestroyed += addend.BastionsDestroyed;
            BastionsLost += addend.BastionsLost;

            ObjectiveCaptureTicks += addend.ObjectiveCaptureTicks;
            ObjectiveDefenseTicks += addend.ObjectiveDefenseTicks;
            BaseCaptures += addend.BaseCaptures;
            BaseDefenses += addend.BaseDefenses;

            if (addend.BaseCaptures > 0 && addend.BaseDefenses == 0)
            {
                PreviousScoredBaseControlType = FacilityControlType.Capture;
            }
            else if (addend.BaseDefenses > 0 && addend.BaseCaptures == 0)
            {
                PreviousScoredBaseControlType = FacilityControlType.Defense;
            }

            return this;
        }

        public ScrimEventAggregate Subtract(ScrimEventAggregate subtrahend)
        {
            Points -= subtrahend.Points;
            NetScore -= subtrahend.NetScore;
            Kills -= subtrahend.Kills;
            Deaths -= subtrahend.Deaths;
            Headshots -= subtrahend.Headshots;
            HeadshotDeaths -= subtrahend.HeadshotDeaths;
            Suicides -= subtrahend.Suicides;
            Teamkills -= subtrahend.Teamkills;
            TeamkillDeaths -= subtrahend.TeamkillDeaths;
            RevivesGiven -= subtrahend.RevivesGiven;
            RevivesTaken -= subtrahend.RevivesTaken;
            DamageAssists -= subtrahend.DamageAssists;
            UtilityAssists -= subtrahend.UtilityAssists;
            DamageAssistedDeaths -= subtrahend.DamageAssistedDeaths;
            UtilityAssistedDeaths -= subtrahend.UtilityAssistedDeaths;

            VehiclesDestroyed -= subtrahend.VehiclesDestroyed;
            VehiclesLost -= subtrahend.VehiclesLost;

            InterceptorsDestroyed -= subtrahend.InterceptorsDestroyed;
            InterceptorsLost -= subtrahend.InterceptorsLost;
            EsfsDestroyed -= subtrahend.EsfsDestroyed;
            EsfsLost -= subtrahend.EsfsLost;
            ValkyriesDestroyed -= subtrahend.ValkyriesDestroyed;
            ValkyriesLost -= subtrahend.ValkyriesLost;
            LiberatorsDestroyed -= subtrahend.LiberatorsDestroyed;
            LiberatorsLost -= subtrahend.LiberatorsLost;
            GalaxiesDestroyed -= subtrahend.GalaxiesDestroyed;
            GalaxiesLost -= subtrahend.GalaxiesLost;
            BastionsDestroyed -= subtrahend.BastionsDestroyed;
            BastionsLost -= subtrahend.BastionsLost;

            ObjectiveCaptureTicks -= subtrahend.ObjectiveCaptureTicks;
            ObjectiveDefenseTicks -= subtrahend.ObjectiveDefenseTicks;
            BaseCaptures -= subtrahend.BaseCaptures;
            BaseDefenses -= subtrahend.BaseDefenses;

            if (BaseControlVictories == 0)
            {
                PreviousScoredBaseControlType = FacilityControlType.Unknown;
            }
            else if ( subtrahend.BaseCaptures != 0 || subtrahend.BaseDefenses != 0)
            {
                if (PreviousScoredBaseControlType == FacilityControlType.Capture || BaseDefenses > BaseCaptures)
                {
                    PreviousScoredBaseControlType = FacilityControlType.Defense;
                }
                else if (PreviousScoredBaseControlType == FacilityControlType.Defense || BaseCaptures > BaseDefenses)
                {
                    PreviousScoredBaseControlType = FacilityControlType.Capture;
                }
            }

            return this;
        }
    }
}
