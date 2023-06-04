using System;
using System.Collections.Generic;
using System.Linq;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class ScrimEventAggregate
    {
        public int Points { get; set; } = 0;
        public int NetScore { get; set; } = 0;

        public List<PointAdjustment> PointAdjustments = new List<PointAdjustment>();

        public int PointsPenalized
        {
            get
            {
                return GetPointsPenalized();
            }
        }

        public int PointsGranted
        {
            get
            {
                return GetPointsGranted();
            }
        }

        public int FirstCaptures { get; set; } = 0;
        public int SubsequentCaptures { get; set; } = 0;

        public int WeightedCapturesCount
        {
            get
            {
                return FirstCaptures + (SubsequentCaptures * 2);
            }
        }

        public int FirstCapturePoints { get; set; } = 0;
        public int SubsequentCapturePoints { get; set; } = 0;
        public int PeriodicCapturePoints { get; set; }
        public int PeriodicCaptureTicks { get; set; }

        public int CapturePoints
        {
            get
            {
                return FirstCapturePoints + SubsequentCapturePoints + PeriodicCaptureTicks;
            }
        }


        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int Headshots { get; set; } = 0;
        public int HeadshotDeaths { get; set; } = 0;
        public int Suicides { get; set; } = 0;
        public int Teamkills { get; set; } = 0;
        public int TeamkillDeaths { get; set; } = 0;

        public int RevivesGiven { get; set; } = 0;
        public int RevivesTaken { get; set; } = 0;

        public int EnemyRevivesAllowed { get; set; } = 0;
        public int KillsUndoneByRevive { get; set; } = 0;

        public int SecuredKills => Kills - KillsUndoneByRevive;
        public int ConfirmedDeaths => Deaths - RevivesTaken;

        public int DamageAssists { get; set; } = 0;

        public int GrenadeAssists { get; set; } = 0;
        public int SpotAssists { get; set; } = 0;
        public int HealSupportAssists { get; set; } = 0;
        public int ProtectAlliesAssists { get; set; } = 0;

        //public int UtilityAssists { get; set; } = 0;
        public int UtilityAssists
        {
            get
            {
                return GrenadeAssists + SpotAssists; // + HealSupportAssists + ProtectAlliesAssists;
            }
        }
        public int Assists
        {
            get
            {
                return DamageAssists + UtilityAssists;
            }
        }

        public int DamageAssistedDeaths { get; set; } = 0;

        public int GrenadeAssistedDeaths { get; set; } = 0;
        public int SpotAssistedDeaths { get; set; } = 0;
        public int ProtectAlliesAssistedDeaths { get; set; } = 0;
        //public int UtilityAssistedDeaths { get; set; } = 0;
        public int UtilityAssistedDeaths
        {
            get
            {
                return GrenadeAssistedDeaths + SpotAssistedDeaths; // + ProtectAlliesAssistedDeaths;
            }
        }

        public int DamageTeamAssists { get; set; } = 0;
        public int DamageSelfAssists { get; set; } = 0;
        public int GrenadeTeamAssists { get; set; } = 0;
        public int GrenadeSelfAssists { get; set; } = 0;
        public int DamageTeamAssistedDeaths { get; set; } = 0;
        public int GrenadeTeamAssistedDeaths { get; set; } = 0;
        public int DamageSelfAssistedDeaths { get; set; } = 0;
        public int GrenadeSelfAssistedDeaths { get; set; } = 0;

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

        #region Vehicles
        public int VehiclesDestroyed { get; set; } = 0;
        public int VehiclesLost { get; set; } = 0;

        #region Air Vehicles
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
        #endregion Air Vehicles

        #region Ground Vehicles
        public int FlashesDestroyed { get; set; } = 0;
        public int FlashesLost { get; set; } = 0;
        public int HarassersDestroyed { get; set; } = 0;
        public int HarassersLost { get; set; } = 0;
        public int AntsDestroyed { get; set; } = 0;
        public int AntsLost { get; set; } = 0;
        public int SunderersDestroyed { get; set; } = 0;
        public int SunderersLost { get; set; } = 0;
        public int LightningsDestroyed { get; set; } = 0;
        public int LightningsLost { get; set; } = 0;
        public int MbtsDestroyed { get; set; } = 0;
        public int MbtsLost { get; set; } = 0;
        #endregion Ground Vehicles

        #endregion Vehicles

        public int Events
        {
            get
            {
                return (Kills
                        + Deaths
                        + Teamkills
                        + RevivesGiven 
                        + RevivesTaken
                        + Assists
                        + ObjectiveTicks
                        + VehiclesDestroyed
                        + VehiclesLost);
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
                return Events > 0;
                //return (Kills > 0 || Deaths > 0 || Teamkills > 0);
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

        private int GetPointsPenalized()
        {
            if (PointAdjustments.Any(pa => pa.AdjustmentType == PointAdjustmentType.Penalty))
            {
                return PointAdjustments.Where(pa => pa.AdjustmentType == PointAdjustmentType.Penalty)
                                       .Select(pa => pa.Points)
                                       .Sum();

            }
            else
            {
                return 0;
            }
        }

        private int GetPointsGranted()
        {
            if (PointAdjustments.Any(pa => pa.AdjustmentType == PointAdjustmentType.Bonus))
            {
                return PointAdjustments.Where(pa => pa.AdjustmentType == PointAdjustmentType.Bonus)
                                       .Select(pa => pa.Points)
                                       .Sum();

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

            FirstCaptures += addend.FirstCaptures;
            SubsequentCaptures += addend.SubsequentCaptures;
            PeriodicCaptureTicks += addend.PeriodicCaptureTicks;

            FirstCapturePoints += addend.FirstCapturePoints;
            SubsequentCapturePoints += addend.SubsequentCapturePoints;
            PeriodicCapturePoints += addend.PeriodicCapturePoints;

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
            //UtilityAssists += addend.UtilityAssists;
            GrenadeAssists += addend.GrenadeAssists;
            SpotAssists += addend.SpotAssists;
            HealSupportAssists += addend.HealSupportAssists;
            ProtectAlliesAssists += addend.ProtectAlliesAssists;

            EnemyRevivesAllowed += addend.EnemyRevivesAllowed;
            KillsUndoneByRevive += addend.KillsUndoneByRevive;

            DamageAssistedDeaths += addend.DamageAssistedDeaths;
            //UtilityAssistedDeaths += addend.UtilityAssistedDeaths;
            GrenadeAssistedDeaths += addend.GrenadeAssistedDeaths;
            SpotAssistedDeaths += addend.SpotAssistedDeaths;
            ProtectAlliesAssistedDeaths += addend.ProtectAlliesAssistedDeaths;

            DamageTeamAssists += addend.DamageTeamAssists;
            DamageSelfAssists += addend.DamageSelfAssists;
            GrenadeTeamAssists += addend.GrenadeTeamAssists;
            GrenadeSelfAssists += addend.GrenadeSelfAssists;
            DamageTeamAssistedDeaths += addend.DamageTeamAssistedDeaths;
            DamageSelfAssistedDeaths += addend.DamageSelfAssistedDeaths;
            GrenadeTeamAssistedDeaths += addend.GrenadeTeamAssistedDeaths;
            GrenadeSelfAssistedDeaths += addend.GrenadeSelfAssistedDeaths;

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

            FlashesDestroyed += addend.FlashesDestroyed;
            FlashesLost += addend.FlashesLost;
            HarassersDestroyed += addend.HarassersDestroyed;
            HarassersLost += addend.HarassersLost;
            AntsDestroyed += addend.AntsDestroyed;
            AntsLost += addend.AntsLost;
            SunderersDestroyed += addend.SunderersDestroyed;
            SunderersLost += addend.SunderersLost;
            LightningsDestroyed += addend.LightningsDestroyed;
            LightningsLost += addend.LightningsLost;
            MbtsDestroyed += addend.MbtsDestroyed;
            MbtsLost += addend.MbtsLost;

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

            if (addend.PointAdjustments.Count > 0)
            {
                PointAdjustments.AddRange(addend.PointAdjustments.Where(pa => !PointAdjustments.Contains(pa)).ToList());
            }

            return this;
        }

        public ScrimEventAggregate Subtract(ScrimEventAggregate subtrahend)
        {
            Points -= subtrahend.Points;
            NetScore -= subtrahend.NetScore;

            FirstCaptures -= subtrahend.FirstCaptures;
            SubsequentCaptures -= subtrahend.SubsequentCaptures;
            PeriodicCaptureTicks -= subtrahend.PeriodicCaptureTicks;

            FirstCapturePoints -= subtrahend.FirstCapturePoints;
            SubsequentCapturePoints -= subtrahend.SubsequentCapturePoints;
            PeriodicCapturePoints -= subtrahend.PeriodicCapturePoints;

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

            //UtilityAssists -= subtrahend.UtilityAssists;
            GrenadeAssists -= subtrahend.GrenadeAssists;
            SpotAssists -= subtrahend.SpotAssists;
            HealSupportAssists -= subtrahend.HealSupportAssists;
            ProtectAlliesAssists -= subtrahend.ProtectAlliesAssists;

            DamageAssistedDeaths -= subtrahend.DamageAssistedDeaths;
            //UtilityAssistedDeaths -= subtrahend.UtilityAssistedDeaths;
            GrenadeAssistedDeaths -= subtrahend.GrenadeAssistedDeaths;
            SpotAssistedDeaths -= subtrahend.SpotAssistedDeaths;
            ProtectAlliesAssistedDeaths -= subtrahend.ProtectAlliesAssistedDeaths;

            EnemyRevivesAllowed -= subtrahend.EnemyRevivesAllowed;
            KillsUndoneByRevive += subtrahend.KillsUndoneByRevive;

            DamageTeamAssists -= subtrahend.DamageTeamAssists;
            DamageSelfAssists -= subtrahend.DamageSelfAssists;
            GrenadeTeamAssists -= subtrahend.GrenadeTeamAssists;
            GrenadeSelfAssists -= subtrahend.GrenadeSelfAssists;
            DamageTeamAssistedDeaths -= subtrahend.DamageTeamAssistedDeaths;
            DamageSelfAssistedDeaths -= subtrahend.DamageSelfAssistedDeaths;
            GrenadeTeamAssistedDeaths -= subtrahend.GrenadeTeamAssistedDeaths;
            GrenadeSelfAssistedDeaths -= subtrahend.GrenadeSelfAssistedDeaths;

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

            FlashesDestroyed -= subtrahend.FlashesDestroyed;
            FlashesLost -= subtrahend.FlashesLost;
            HarassersDestroyed -= subtrahend.HarassersDestroyed;
            HarassersLost -= subtrahend.HarassersLost;
            AntsDestroyed -= subtrahend.AntsDestroyed;
            AntsLost -= subtrahend.AntsLost;
            SunderersDestroyed -= subtrahend.SunderersDestroyed;
            SunderersLost -= subtrahend.SunderersLost;
            LightningsDestroyed -= subtrahend.LightningsDestroyed;
            LightningsLost -= subtrahend.LightningsLost;
            MbtsDestroyed -= subtrahend.MbtsDestroyed;
            MbtsLost -= subtrahend.MbtsLost;

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
                // Base Defenses, if there are any, always occur before Base Captures within a round
                if (BaseDefenses > BaseCaptures)
                {
                    PreviousScoredBaseControlType = FacilityControlType.Defense;
                }
                else if (BaseCaptures >= BaseDefenses)
                {
                    PreviousScoredBaseControlType = FacilityControlType.Capture;
                }
            }

            if (subtrahend.PointAdjustments.Count > 0)
            {
                PointAdjustments.RemoveAll(pa => subtrahend.PointAdjustments.Contains(pa));
            }

            return this;
        }

        public void AddPointAdjustment(PointAdjustment adjustment)
        {
            Points += adjustment.Points;
            NetScore += adjustment.Points;

            PointAdjustments.Add(adjustment);
        }

        public void RemovePointAdjustment(PointAdjustment adjustment)
        {
            if (!PointAdjustments.Contains(adjustment))
            {
                return;
            }
            
            Points -= adjustment.Points;
            NetScore -= adjustment.Points;

            PointAdjustments.Remove(adjustment);
        }
    }
}
