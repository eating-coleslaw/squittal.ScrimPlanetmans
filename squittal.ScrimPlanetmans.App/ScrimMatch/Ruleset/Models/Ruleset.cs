using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class Ruleset
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateLastModified { get; set; }

        public bool IsDefault { get; set; }
        public bool IsCustomDefault { get; set; }

        public string SourceFile { get; set; }

        public string DefaultMatchTitle { get; set; } = string.Empty;

        public bool EnableRoundTimeLimit { get; set; } = true;
        public int DefaultRoundLength { get; set; } = 900;
        public TimerDirection? RoundTimerDirection { get; set; } = TimerDirection.Down;

        public bool DefaultEndRoundOnFacilityCapture { get; set; } = false;

        public bool EndRoundOnPointValueReached { get; set; } = false;
        public int? TargetPointValue { get; set; }
        public int? InitialPoints { get; set; }

        public MatchWinCondition MatchWinCondition { get; set; } = MatchWinCondition.MostPoints;
        public RoundWinCondition RoundWinCondition { get; set; } = RoundWinCondition.NotApplicable;

        public bool EnablePeriodicFacilityControlRewards { get; set; } = false;
        public int? PeriodicFacilityControlPoints { get; set; }
        public int? PeriodicFacilityControlInterval { get; set; }
        public PointAttributionType? PeriodFacilityControlPointAttributionType { get; set; } //= PointAttributionType.Standard;


        public ICollection<RulesetActionRule> RulesetActionRules { get; set; }
        public ICollection<RulesetItemCategoryRule> RulesetItemCategoryRules { get; set; }
        public ICollection<RulesetItemRule> RulesetItemRules { get; set; }
        public ICollection<RulesetFacilityRule> RulesetFacilityRules { get; set; }

        public RulesetOverlayConfiguration RulesetOverlayConfiguration { get; set; }

    }
}
