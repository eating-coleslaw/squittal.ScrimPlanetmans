using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace squittal.ScrimPlanetmans.Models.Forms
{
    public class OutfitAlias
    {
        [StringLength(4, MinimumLength = 1)]
        [CustomValidation(typeof(OutfitAliasValidation), nameof(OutfitAliasValidation.OutfitAliasValidate))]
        public string Alias { get; set; }

        public OutfitAlias()
        {
            Alias = string.Empty;
        }

        public OutfitAlias(string alias)
        {
            Alias = alias;
        }
    }

    public class OutfitAliasValidation
    {
        private static Regex Regex = new Regex("[A-Za-z0-9]{1,4}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static ValidationResult OutfitAliasValidate(string alias)
        {
            Match match = Regex.Match(alias);
            if (!match.Success)
            {
                return new ValidationResult("Invalid outfit alias");
            }

            return ValidationResult.Success;
        }
    }
}
