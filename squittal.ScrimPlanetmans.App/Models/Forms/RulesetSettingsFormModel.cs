using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;

namespace squittal.ScrimPlanetmans.Models.Forms
{
    public class RulesetSettingsFormModel
    {
        public int RulesetId { get; set; }

        public string RulesetName { get; set; }
        public string DefaultMatchTitle { get; set; }

        private bool _enableRoundTimeLimit = true;
        private TimerDirection _roundTimerDirection = TimerDirection.Down;

        //public bool EnableRoundTimeLimit { get; set; }
        public bool EnableRoundTimeLimit
        {
            get => _enableRoundTimeLimit;
            set
            {
                _enableRoundTimeLimit = value;
                if (!value)
                {
                    _roundTimerDirection = TimerDirection.Up;
                }
                else
                {
                    _roundTimerDirection = TimerDirection.Down;
                }
            }
        }

        public int DefaultRoundLength { get; set; }
        public TimerDirection RoundTimerDirection => _roundTimerDirection; // { get; set; } = TimerDirection.Down;

        public bool DefaultEndRoundOnFacilityCapture { get; set; }

        public bool EndRoundOnPointValueReached { get; set; } = false;
        public int TargetPointValue { get; set; }
        public int InitialPoints { get; set; } = 0;

        public MatchWinCondition MatchWinCondition { get; set; } = MatchWinCondition.MostPoints;
        public RoundWinCondition RoundWinCondition { get; set; } = RoundWinCondition.NotApplicable;

        public bool EnablePeriodicFacilityControlRewards { get; set; } = false;
        public int PeriodicFacilityControlPoints { get; set; }
        public int PeriodicFacilityControlInterval { get; set; }
        public PointAttributionType PeriodFacilityControlPointAttributionType { get; set; } = PointAttributionType.Standard; // PointAttributionType.Tickets;


        #region Overlay Settings
        public bool UseCompactOverlayLayout { get; set; }
        public OverlayStatsDisplayType OverlayStatsDisplayType { get; set; }

        public ShowStatusPanelScoresSelectOptions ShowOverlayStatusPanelScoresSelection { get; set; }
        public bool? ShowOverlayStatusPanelScores => ConvertToNullableBool(ShowOverlayStatusPanelScoresSelection);

        private readonly bool _useCompactOverlayLayoutDefault = false;
        private readonly OverlayStatsDisplayType _overlayStatsDisplayTypeDefault = OverlayStatsDisplayType.InfantryScores;
        private readonly ShowStatusPanelScoresSelectOptions _showOverlayStatusPanelScoresSelectionDefault = ShowStatusPanelScoresSelectOptions.UseStatsDisplayDefault;
        #endregion Overlay Settings


        public RulesetSettingsFormModel(Ruleset ruleset)
        {
            RulesetId = ruleset.Id;
            RulesetName = ruleset.Name;

            //DefaultRoundLength = ruleset.DefaultRoundLength;
            DefaultMatchTitle = ruleset.DefaultMatchTitle;
            //DefaultEndRoundOnFacilityCapture = ruleset.DefaultEndRoundOnFacilityCapture;

            EnableRoundTimeLimit = ruleset.EnableRoundTimeLimit;
            DefaultRoundLength = ruleset.DefaultRoundLength;
            //RoundTimerDirection = ruleset.RoundTimerDirection ?? TimerDirection.Down;
            _roundTimerDirection = ruleset.RoundTimerDirection ?? TimerDirection.Down;

            DefaultEndRoundOnFacilityCapture = ruleset.DefaultEndRoundOnFacilityCapture;

            EndRoundOnPointValueReached = ruleset.EndRoundOnPointValueReached;
            TargetPointValue = ruleset.TargetPointValue ?? 100;
            InitialPoints = 0; // ruleset.InitialPoints ?? 0;

            MatchWinCondition = ruleset.MatchWinCondition;
            RoundWinCondition = ruleset.RoundWinCondition;

            EnablePeriodicFacilityControlRewards = ruleset.EnablePeriodicFacilityControlRewards;
            PeriodicFacilityControlPoints = ruleset.PeriodicFacilityControlPoints ?? 1;
            PeriodicFacilityControlInterval = ruleset.PeriodicFacilityControlInterval ?? 10;
            PeriodFacilityControlPointAttributionType = ruleset.PeriodFacilityControlPointAttributionType ?? PointAttributionType.Standard;

            if (ruleset.RulesetOverlayConfiguration != null)
            {
                UseCompactOverlayLayout = ruleset.RulesetOverlayConfiguration.UseCompactLayout;
                OverlayStatsDisplayType = ruleset.RulesetOverlayConfiguration.StatsDisplayType;
                ShowOverlayStatusPanelScoresSelection = ConvertToSelectOption(ruleset.RulesetOverlayConfiguration.ShowStatusPanelScores);
            }
            else
            {
                UseCompactOverlayLayout = _useCompactOverlayLayoutDefault;
                OverlayStatsDisplayType = _overlayStatsDisplayTypeDefault;
                ShowOverlayStatusPanelScoresSelection = _showOverlayStatusPanelScoresSelectionDefault;
            }
        }

        public void SetProperties(Ruleset ruleset)
        {
            RulesetId = ruleset.Id;
            RulesetName = ruleset.Name;

            //DefaultRoundLength = ruleset.DefaultRoundLength;
            DefaultMatchTitle = ruleset.DefaultMatchTitle;
            //DefaultEndRoundOnFacilityCapture = ruleset.DefaultEndRoundOnFacilityCapture;

            EnableRoundTimeLimit = ruleset.EnableRoundTimeLimit;
            DefaultRoundLength = ruleset.DefaultRoundLength;
            //RoundTimerDirection = ruleset.RoundTimerDirection ?? TimerDirection.Down;
            _roundTimerDirection = ruleset.RoundTimerDirection ?? TimerDirection.Down;

            DefaultEndRoundOnFacilityCapture = ruleset.DefaultEndRoundOnFacilityCapture;
            
            EndRoundOnPointValueReached = ruleset.EndRoundOnPointValueReached;
            TargetPointValue = ruleset.TargetPointValue ?? 100;
            InitialPoints = 0; // ruleset.InitialPoints ?? 0;

            MatchWinCondition = ruleset.MatchWinCondition;
            RoundWinCondition = ruleset.RoundWinCondition;
            
            EnablePeriodicFacilityControlRewards = ruleset.EnablePeriodicFacilityControlRewards;
            PeriodicFacilityControlPoints = ruleset.PeriodicFacilityControlPoints ?? 1;
            PeriodicFacilityControlInterval = ruleset.PeriodicFacilityControlInterval ?? 10;
            PeriodFacilityControlPointAttributionType = ruleset.PeriodFacilityControlPointAttributionType ?? PointAttributionType.Standard;

            if (ruleset.RulesetOverlayConfiguration != null)
            {
                UseCompactOverlayLayout = ruleset.RulesetOverlayConfiguration.UseCompactLayout;
                OverlayStatsDisplayType = ruleset.RulesetOverlayConfiguration.StatsDisplayType;
                ShowOverlayStatusPanelScoresSelection = ConvertToSelectOption(ruleset.RulesetOverlayConfiguration.ShowStatusPanelScores);
            }
            else
            {
                UseCompactOverlayLayout = _useCompactOverlayLayoutDefault;
                OverlayStatsDisplayType = _overlayStatsDisplayTypeDefault;
                ShowOverlayStatusPanelScoresSelection = _showOverlayStatusPanelScoresSelectionDefault;
            }
        }

        public Ruleset GetRuleset()
        {
            return new Ruleset
            {
                Id = RulesetId,
                Name = RulesetName,
                //DefaultRoundLength = DefaultRoundLength,
                DefaultMatchTitle = DefaultMatchTitle,
                //DefaultEndRoundOnFacilityCapture = DefaultEndRoundOnFacilityCapture,

                EnableRoundTimeLimit = EnableRoundTimeLimit,
                DefaultRoundLength = DefaultRoundLength,
                RoundTimerDirection = RoundTimerDirection,

                DefaultEndRoundOnFacilityCapture = DefaultEndRoundOnFacilityCapture,
                
                EndRoundOnPointValueReached = EndRoundOnPointValueReached,
                TargetPointValue = TargetPointValue,
                InitialPoints = InitialPoints,
                
                MatchWinCondition = MatchWinCondition,
                RoundWinCondition = RoundWinCondition,
                
                EnablePeriodicFacilityControlRewards = EnablePeriodicFacilityControlRewards,
                PeriodicFacilityControlPoints = PeriodicFacilityControlPoints,
                PeriodicFacilityControlInterval = PeriodicFacilityControlInterval,
                PeriodFacilityControlPointAttributionType = PeriodFacilityControlPointAttributionType,
        };
        }

        public RulesetOverlayConfiguration GetOverlayConfiguration()
        {
            return new RulesetOverlayConfiguration
            {
                RulesetId = RulesetId,
                UseCompactLayout = UseCompactOverlayLayout,
                StatsDisplayType = OverlayStatsDisplayType,
                ShowStatusPanelScores = ShowOverlayStatusPanelScores
            };
        }

        private ShowStatusPanelScoresSelectOptions ConvertToSelectOption(bool? showStatusPanelScore)
        {
            switch (showStatusPanelScore)
            {
                case true:
                    return ShowStatusPanelScoresSelectOptions.Yes;

                case false:
                    return ShowStatusPanelScoresSelectOptions.No;

                case null:
                    return ShowStatusPanelScoresSelectOptions.UseStatsDisplayDefault;
            }
        }

        private bool? ConvertToNullableBool(ShowStatusPanelScoresSelectOptions showOverlayStatusPanelScoresSelection)
        {
            switch (showOverlayStatusPanelScoresSelection)
            {
                case ShowStatusPanelScoresSelectOptions.Yes:
                    return true;

                case ShowStatusPanelScoresSelectOptions.No:
                    return false;

                case ShowStatusPanelScoresSelectOptions.UseStatsDisplayDefault:
                    return null;

                default:
                    return null;
            }
        }
    }


    public enum ShowStatusPanelScoresSelectOptions
    {
        Yes,
        No,
        UseStatsDisplayDefault
    }
}
