using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using squittal.ScrimPlanetmans.Services.Rulesets;
using System;
using System.Threading;

namespace squittal.ScrimPlanetmans.Models.ScrimEngine
{
    public class MatchConfiguration
    {    
        public string Title { get; set; } = "PS2 Scrims";

        public bool IsManualTitle { get; private set; } = false;


        public int RoundSecondsTotal { get; set; } = 900;
        public bool IsManualRoundSecondsTotal { get; private set; } = false;

        // Target Base Configuration
        public bool IsManualWorldId { get; private set; } = false;
        public bool IsWorldIdSet { get; private set; } = false;
        public int WorldId { get => GetWorldIdFromString(); }
        public string WorldIdString { get; set; } = "19";
        public int FacilityId { get => GetFacilityIdFromString(); }
        public string FacilityIdString { get; set; } = "-1";

        public bool EndRoundOnFacilityCapture { get; set; } = false; // TODO: move this setting to the Ruleset model
        public bool IsManualEndRoundOnFacilityCapture { get; private set; } = false;

        public int? TargetPointValue { get; set; } //= 200;
        public int TargetPointValueNonNullable => TargetPointValue.GetValueOrDefault();
        public bool IsManualTargetPointValue { get; private set; }
        public int? InitialPoints { get; set; } //= 0;
        public bool IsManualInitialPoints { get; private set; }

        public int? PeriodicFacilityControlPoints { get; set; } //= 5;
        public int PeriodicFacilityControlPointsNonNullable => PeriodicFacilityControlPoints.GetValueOrDefault();
        public bool IsManualPeriodicFacilityControlPoints { get; private set; }
        public int? PeriodicFacilityControlInterval { get; set; } //= 15;
        public int PeriodicFacilityControlIntervalNonNullable => PeriodicFacilityControlInterval.GetValueOrDefault();
        public bool IsManualPeriodicFacilityControlInterval { get; private set; }

        #region Values from Ruleset
        public bool EnableRoundTimeLimit { get; set; }
        public TimerDirection? RoundTimerDirection { get; set; }
        public bool EndRoundOnPointValueReached { get; set; }
        public MatchWinCondition MatchWinCondition { get; set; }
        public RoundWinCondition RoundWinCondition { get; set; }
        public bool EnablePeriodicFacilityControlRewards { get; set; } = false;
        public PointAttributionType? PeriodFacilityControlPointAttributionType { get; set; }
        #endregion Values from Ruleset

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);
        private readonly AutoResetEvent _autoEventRoundSeconds = new AutoResetEvent(true);
        private readonly AutoResetEvent _autoEventMatchTitle = new AutoResetEvent(true);
        private readonly AutoResetEvent _autoEndRoundOnFacilityCapture = new AutoResetEvent(true);

        private readonly AutoResetEvent _autoTargetPointValue = new AutoResetEvent(true);
        private readonly AutoResetEvent _autoInitialPoints = new AutoResetEvent(true);
        private readonly AutoResetEvent _autoPeriodicFacilityControlPoints = new AutoResetEvent(true);
        private readonly AutoResetEvent _autoPeriodicFacilityControlInterval = new AutoResetEvent(true);

        public bool SaveLogFiles { get; set; } = true;
        public bool SaveEventsToDatabase { get; set; } = true;

        public MatchConfiguration()
        {
        }

        public MatchConfiguration(Ruleset ruleset)
        {
            Title = ruleset.DefaultMatchTitle;
            EnableRoundTimeLimit = ruleset.EnableRoundTimeLimit;
            RoundSecondsTotal = ruleset.DefaultRoundLength;
            RoundTimerDirection = ruleset.RoundTimerDirection;
            EndRoundOnPointValueReached = ruleset.EndRoundOnPointValueReached;
            MatchWinCondition = ruleset.MatchWinCondition;
            RoundWinCondition = ruleset.RoundWinCondition;
            TargetPointValue = ruleset.TargetPointValue;
            InitialPoints = ruleset.InitialPoints;
            EnablePeriodicFacilityControlRewards = ruleset.EnablePeriodicFacilityControlRewards;
            PeriodFacilityControlPointAttributionType = ruleset.PeriodFacilityControlPointAttributionType;
            PeriodicFacilityControlPoints = ruleset.PeriodicFacilityControlPoints;
            PeriodicFacilityControlInterval = ruleset.PeriodicFacilityControlInterval;
        }

        public void CopyValues(MatchConfiguration sourceConfig)
        {
            Title = sourceConfig.Title;
            IsManualTitle = sourceConfig.IsManualTitle;
            RoundSecondsTotal = sourceConfig.RoundSecondsTotal;
            IsManualRoundSecondsTotal = sourceConfig.IsManualRoundSecondsTotal;
            IsManualWorldId = sourceConfig.IsManualWorldId;
            IsWorldIdSet = sourceConfig.IsWorldIdSet;
            WorldIdString = sourceConfig.WorldIdString;
            FacilityIdString = sourceConfig.FacilityIdString;
            EndRoundOnFacilityCapture = sourceConfig.EndRoundOnFacilityCapture;
            IsManualEndRoundOnFacilityCapture = sourceConfig.IsManualEndRoundOnFacilityCapture;
            TargetPointValue = sourceConfig.TargetPointValue;
            IsManualTargetPointValue = sourceConfig.IsManualTargetPointValue;
            InitialPoints = sourceConfig.InitialPoints;
            IsManualInitialPoints = sourceConfig.IsManualInitialPoints;
            PeriodicFacilityControlPoints = sourceConfig.PeriodicFacilityControlPoints;
            IsManualPeriodicFacilityControlPoints = sourceConfig.IsManualPeriodicFacilityControlPoints;
            PeriodicFacilityControlInterval = sourceConfig.PeriodicFacilityControlInterval;
            IsManualPeriodicFacilityControlInterval = sourceConfig.IsManualPeriodicFacilityControlInterval;
            EnableRoundTimeLimit = sourceConfig.EnableRoundTimeLimit;
            RoundTimerDirection = sourceConfig.RoundTimerDirection;
            EndRoundOnPointValueReached = sourceConfig.EndRoundOnPointValueReached;
            MatchWinCondition = sourceConfig.MatchWinCondition;
            RoundWinCondition = sourceConfig.RoundWinCondition;
            EnablePeriodicFacilityControlRewards = sourceConfig.EnablePeriodicFacilityControlRewards;
            PeriodFacilityControlPointAttributionType = sourceConfig.PeriodFacilityControlPointAttributionType;
        }

        public bool TrySetTitle(string title, bool isManualValue)
        {
            if (!RulesetDataService.IsValidRulesetDefaultMatchTitle(title))
            {
                return false;
            }

            _autoEventMatchTitle.WaitOne();

            if (isManualValue)
            {
                Title = title;
                IsManualTitle = true;

                _autoEventMatchTitle.Set();

                return true;
            }
            else if (!IsManualTitle)
            {
                Title = title;

                _autoEventMatchTitle.Set();

                return true;
            }
            else
            {
                _autoEventMatchTitle.Set();

                return false;
            }
        }

        public bool TrySetRoundLength(int seconds, bool isManualValue)
        {
            //Console.WriteLine($"TrySetRoundLength({seconds},{isManualValue})");
            
            if (seconds <= 0)
            {
                return false;
            }

            _autoEventRoundSeconds.WaitOne();

            if (isManualValue)
            {
                RoundSecondsTotal = seconds;
                IsManualRoundSecondsTotal = true;

                _autoEventRoundSeconds.Set();

                return true;
            }
            else if (!IsManualRoundSecondsTotal)
            {
                RoundSecondsTotal = seconds;

                _autoEventRoundSeconds.Set();

                return true;
            }
            else
            {
                _autoEventRoundSeconds.Set();

                return false;
            }
        }

        public bool TrySetTargetPointValue(int? points, bool isManualValue)
        {
            //Console.WriteLine($"TrySetTargetPointValue({points},{isManualValue})");
            _autoTargetPointValue.WaitOne();

            if (isManualValue)
            {
                TargetPointValue = points;
                IsManualTargetPointValue = true;

                _autoTargetPointValue.Set();
                return true;
            }
            else if (!IsManualTargetPointValue)
            {
                TargetPointValue = points;
                
                _autoTargetPointValue.Set();
                return true;
            }
            else
            {
                _autoTargetPointValue.Set();
                return false;
            }
        }

        public void ResetTargetPointValue()
        {
            TargetPointValue = 200;
            IsManualTargetPointValue = false;
        }

        public bool TrySetInitialPoints(int? points, bool isManualValue)
        {
            //Console.WriteLine($"TrySetInitialPoints({points},{isManualValue})");
            _autoInitialPoints.WaitOne();

            if (isManualValue)
            {
                InitialPoints = points;
                IsManualInitialPoints = true;

                _autoTargetPointValue.Set();
                return true;
            }
            else if (!IsManualInitialPoints)
            {
                InitialPoints = points;

                _autoInitialPoints.Set();
                return true;
            }
            else
            {
                _autoInitialPoints.Set();
                return false;
            }
        }

        public void ResetInitialPoints()
        {
            InitialPoints = 0;
            IsManualInitialPoints = false;
        }

        public bool TrySetPeriodicFacilityControlPoints(int? points, bool isManualValue)
        {
            //Console.WriteLine($"TrySetPeriodicFacilityControlPoints({points},{isManualValue})");
            _autoPeriodicFacilityControlPoints.WaitOne();

            if (isManualValue)
            {
                PeriodicFacilityControlPoints = points;
                IsManualPeriodicFacilityControlPoints = true;

                _autoPeriodicFacilityControlPoints.Set();
                return true;
            }
            else if (!IsManualPeriodicFacilityControlPoints)
            {
                PeriodicFacilityControlPoints = points;
                IsManualPeriodicFacilityControlPoints = isManualValue;

                _autoPeriodicFacilityControlPoints.Set();
                return true;
            }
            else
            {
                _autoPeriodicFacilityControlPoints.Set();
                return false;
            }
        }

        public void ResetPeriodicFacilityControlPoints()
        {
            PeriodicFacilityControlPoints = 5;
            IsManualPeriodicFacilityControlPoints = false;
        }

        public bool TrySetPeriodicFacilityControlInterval(int? interval, bool isManualValue)
        {
            //Console.WriteLine($"TrySetPeriodicFacilityControlInterval({interval},{isManualValue})");
            _autoPeriodicFacilityControlInterval.WaitOne();

            if (isManualValue)
            {
                PeriodicFacilityControlInterval = interval;
                IsManualPeriodicFacilityControlInterval = true;

                _autoPeriodicFacilityControlInterval.Set();
                return true;
            }
            else if (!IsManualPeriodicFacilityControlInterval)
            {
                PeriodicFacilityControlInterval = interval;
                IsManualPeriodicFacilityControlInterval = isManualValue;

                _autoPeriodicFacilityControlInterval.Set();
                return true;
            }
            else
            {
                _autoPeriodicFacilityControlInterval.Set();
                return false;
            }
        }

        public void ResetPeriodicFacilityControlInterval()
        {
            PeriodicFacilityControlInterval = 15;
            IsManualPeriodicFacilityControlInterval = false;
        }

        public bool TrySetEndRoundOnFacilityCapture(bool endOnCapture, bool isManualValue)
        {
            _autoEndRoundOnFacilityCapture.WaitOne();

            if (isManualValue)
            {
                EndRoundOnFacilityCapture = endOnCapture;
                IsManualEndRoundOnFacilityCapture = true;

                _autoEndRoundOnFacilityCapture.Set();

                return true;
            }
            else if (!IsManualEndRoundOnFacilityCapture)
            {
                EndRoundOnFacilityCapture = endOnCapture;
                IsManualEndRoundOnFacilityCapture = isManualValue;

                _autoEndRoundOnFacilityCapture.Set();

                return true;
            }
            else
            {
                _autoEndRoundOnFacilityCapture.Set();

                return false;
            }
        }

        public void ResetEndRoundOnFacilityCapture()
        {
            EndRoundOnFacilityCapture = false;
            IsManualEndRoundOnFacilityCapture = false;
        }

        public void ResetWorldId()
        {
            //Console.WriteLine($"Resetting World ID!");
            WorldIdString = "19";
            IsManualWorldId = false;
            IsWorldIdSet = false;
        }
        
        public bool TrySetWorldId(int worldId, bool isManualValue = false, bool isRollBack = false)
        {
            if (worldId <= 0)
            {
                return false;
            }
            return TrySetWorldId(worldId.ToString(), isManualValue, isRollBack);
        }

        public bool TrySetWorldId(string worldIdString, bool isManualValue = false, bool isRollBack = false)
        {
            //Console.WriteLine($"MatchConfiguration: trying to set WorldId);
            
            _autoEvent.WaitOne();

            if (isManualValue)
            {
                WorldIdString = worldIdString;
                IsManualWorldId = true;
                IsWorldIdSet = true;

                _autoEvent.Set();

                return true;
            }
            else if (!IsManualWorldId && (!IsWorldIdSet || isRollBack))
            {
                WorldIdString = worldIdString;

                IsWorldIdSet = true;

                _autoEvent.Set();

                return true;
            }
            else
            {
                _autoEvent.Set();

                return false;
            }
        }

        private int GetFacilityIdFromString()
        {
            if (int.TryParse(FacilityIdString, out int intId))
            {
                return intId;
            }
            else
            {
                return -1;
            }
        }

        private int GetWorldIdFromString()
        {
            if (int.TryParse(WorldIdString, out int intId))
            {
                return intId;
            }
            else
            {
                return 19; // Default to Jaeger
            }
        }
    }
}
