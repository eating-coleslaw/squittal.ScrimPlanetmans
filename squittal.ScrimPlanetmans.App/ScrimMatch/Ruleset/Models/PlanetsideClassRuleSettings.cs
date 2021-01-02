using squittal.ScrimPlanetmans.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class PlanetsideClassRuleSettings : IEquitable<PlanetsideClassRuleSettings>
    {
        public bool InfiltratorIsBanned { get; set; } = false;
        public int InfiltratorPoints { get; set; } = 0;

        public bool LightAssaultIsBanned { get; set; } = false;
        public int LightAssaultPoints { get; set; } = 0;

        public bool MedicIsBanned { get; set; } = false;
        public int MedicPoints { get; set; } = 0;

        public bool EngineerIsBanned { get; set; } = false;
        public int EngineerPoints { get; set; } = 0;

        public bool HeavyAssaultIsBanned { get; set; } = false;
        public int HeavyAssaultPoints { get; set; } = 0;

        public bool MaxIsBanned { get; set; } = false;
        public int MaxPoints { get; set; } = 0;

        #region Constructors
        public PlanetsideClassRuleSettings()
        {
        }

        public PlanetsideClassRuleSettings(RulesetItemCategoryRule rule)
        {
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }

        public PlanetsideClassRuleSettings(RulesetItemRule rule)
        {
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }

        public PlanetsideClassRuleSettings(JsonRulesetItemCategoryRule rule)
        {
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }

        public PlanetsideClassRuleSettings(JsonRulesetItemRule rule)
        {
            InfiltratorIsBanned = rule.InfiltratorIsBanned;
            InfiltratorPoints = rule.InfiltratorPoints;
            LightAssaultIsBanned = rule.LightAssaultIsBanned;
            LightAssaultPoints = rule.LightAssaultPoints;
            MedicIsBanned = rule.MedicIsBanned;
            MedicPoints = rule.MedicPoints;
            EngineerIsBanned = rule.EngineerIsBanned;
            EngineerPoints = rule.EngineerPoints;
            HeavyAssaultIsBanned = rule.HeavyAssaultIsBanned;
            HeavyAssaultPoints = rule.HeavyAssaultPoints;
            MaxIsBanned = rule.MaxIsBanned;
            MaxPoints = rule.MaxPoints;
        }
        #endregion Constructors

        #region Getters
        public bool GetClassIsBanned(PlanetsideClass planetsideClass)
        {
            return planetsideClass switch
            {
                PlanetsideClass.Infiltrator => InfiltratorIsBanned,
                PlanetsideClass.LightAssault => LightAssaultIsBanned,
                PlanetsideClass.Medic => MedicIsBanned,
                PlanetsideClass.Engineer => EngineerIsBanned,
                PlanetsideClass.HeavyAssault => HeavyAssaultIsBanned,
                PlanetsideClass.MAX => MaxIsBanned,
                _ => false,
            };
        }

        public int GetClassPoints(PlanetsideClass planetsideClass)
        {
            return planetsideClass switch
            {
                PlanetsideClass.Infiltrator => InfiltratorPoints,
                PlanetsideClass.LightAssault => LightAssaultPoints,
                PlanetsideClass.Medic => MedicPoints,
                PlanetsideClass.Engineer => EngineerPoints,
                PlanetsideClass.HeavyAssault => HeavyAssaultPoints,
                PlanetsideClass.MAX => MaxPoints,
                _ => 0,
            };
        }
        #endregion Getters

        #region Setters
        public void SetClassIsBanned(PlanetsideClass planetsideClass, bool newIsBanned)
        {
            switch (planetsideClass)
            {
                case PlanetsideClass.Infiltrator:
                    InfiltratorIsBanned = newIsBanned;
                    return;

                case PlanetsideClass.LightAssault:
                    LightAssaultIsBanned = newIsBanned;
                    return;

                case PlanetsideClass.Medic:
                    MedicIsBanned = newIsBanned;
                    return;

                case PlanetsideClass.Engineer:
                    EngineerIsBanned = newIsBanned;
                    return;

                case PlanetsideClass.HeavyAssault:
                    HeavyAssaultIsBanned = newIsBanned;
                    return;

                case PlanetsideClass.MAX:
                    MaxIsBanned = newIsBanned;
                    return;

                default:
                    return;
            }
        }

        public void SetClassPoints(PlanetsideClass planetsideClass, int newPoints)
        {
            switch (planetsideClass)
            {
                case PlanetsideClass.Infiltrator:
                    InfiltratorPoints = newPoints;
                    return;

                case PlanetsideClass.LightAssault:
                    LightAssaultPoints = newPoints;
                    return;

                case PlanetsideClass.Medic:
                    MedicPoints = newPoints;
                    return;

                case PlanetsideClass.Engineer:
                    EngineerPoints = newPoints;
                    return;

                case PlanetsideClass.HeavyAssault:
                    HeavyAssaultPoints = newPoints;
                    return;

                case PlanetsideClass.MAX:
                    MaxPoints = newPoints;
                    return;

                default:
                    return;
            }
        }
        #endregion Setters

        #region Equality
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PlanetsideClassRuleSettings);
        }
        
        public bool Equals(PlanetsideClassRuleSettings s)
        {
            if (s is null)
            {
                return false;
            }

            if (ReferenceEquals(this, s))
            {
                return true;
            }

            if (this.GetType() != s.GetType())
            {
                return false;
            }

            return this.InfiltratorIsBanned == s.InfiltratorIsBanned
                && this.InfiltratorPoints == s.InfiltratorPoints
                && this.LightAssaultIsBanned == s.LightAssaultIsBanned
                && this.LightAssaultPoints == s.LightAssaultPoints
                && this.MedicIsBanned == s.MedicIsBanned
                && this.MedicPoints == s.MedicPoints
                && this.EngineerIsBanned == s.EngineerIsBanned
                && this.EngineerPoints == s.EngineerPoints
                && this.HeavyAssaultIsBanned == s.HeavyAssaultIsBanned
                && this.HeavyAssaultPoints == s.HeavyAssaultPoints
                && this.MaxIsBanned == s.MaxIsBanned
                && this.MaxPoints == s.MaxPoints;
        }

        public static bool operator ==(PlanetsideClassRuleSettings lhs, PlanetsideClassRuleSettings rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(PlanetsideClassRuleSettings lhs, PlanetsideClassRuleSettings rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return GetHashCode();
        }
        #endregion Equality
    }
}
