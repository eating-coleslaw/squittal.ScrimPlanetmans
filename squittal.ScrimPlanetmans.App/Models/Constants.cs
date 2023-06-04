using System;
using System.Text.RegularExpressions;

namespace squittal.ScrimPlanetmans.Models
{
    // TODO: move some of these to a Configuration Options file
    public static class Constants
    {
        public const int MaxRoundSecondsConst = 1800;
        public const int MinRoundSecondsConst = 5; // 300; 5 minutes
        public const int DefaultRoundSecondsConst = 600; // 10 minutes 900; // 15 minutes
        public static int MaxRoundSeconds => MaxRoundSecondsConst; //1800; // 30 minutes
        public static int MinRoundSeconds => MinRoundSecondsConst; //5; //300; // 5 minutes
        public static int DefaultRoundSeconds => DefaultRoundSecondsConst; // 15 minutes

        public static int[] AvailableRoundDurationSeconds => new int[] { 300, 600, 900, 1200, 1500, 1800 };
        public static int[] AvailableRoundDurationMinutes => new int[] { 5, 10, 15, 20, 25, 30 };

        public const int MaxPreRoundCountdownSecondsConst = 30;
        public const int MinPreRoundCountdownSecondsConst = 0; // TODO: change to 5
        public const int DefaultPreRoundCountdownSecondsConst = 5;

        public static int[] AvailablePreRoundCountdownSeconds => new int[] { 0, 5, 10, 15, 20, 25, 30 };

        public static int MaxPreRoundCountdownSeconds => MaxPreRoundCountdownSecondsConst;
        public static int MinPreRoundCountdownSeconds => MinPreRoundCountdownSecondsConst;
        public static int DefaultPreRoundCountdownSeconds => DefaultPreRoundCountdownSecondsConst;


        public static int DefaultMatchFacilityId => -1; // No Facility
        public static int DefaultMatchWorldId => 19; // Jaeger


        public static int[] AvailableWorldIds => new int[]
        {
            1, // Connery
            10, // Miller
            13, // Cobalt
            17, // Emerald
            19, // Jaeger
            40 //  Soltech
        };

        public static int[] AvailableZoneIds => new int[]
        {
            2, // Indar
            4, // Hossin
            6, // Amerish
            8, // Esamir
        };

        public static Regex CharacterIdRegex => new Regex("[0-9]{19}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex CharacterNameRegex => new Regex("^[A-Za-z0-9]{1,32}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex CharacterInputListRegex => new Regex("^((([A-Za-z0-9]{1,32},?[ ]?)+))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //public static Regex CharacterInputListRegex => new Regex("^((([A-z0-9]{1,32},[ ]?)+))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Format for Planetside Infantry League: Season 2 => Namex##
        public static Regex Pil2CharacterNameRegex => new Regex("^[A-Za-z0-9]{2,}(x[0-9]{2})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Format for Legacy Jaeger Characters => TAGxName(VS|NC|TR)
        public static Regex LegacyJaegerCharacterNameRegex => new Regex("^([A-Za-z0-9]{0,4}x).{2,}(?<!(x[0-9]{2}))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex FactionSuffixCharacterNameRegex => new Regex("^[A-Za-z0-9]+(VS|NC|TR)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Regex OutfitAliasRegex => new Regex("[A-Za-z0-9]{1,4}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex OutfitAliasWithBracketsRegex => new Regex("^\\[([A-Za-z0-9]{1,4})\\]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public const string MatchTitleRegexString = "^([A-Za-z0-9()\\[\\]\\-_'.#][ ]{0,1}){1,49}[A-Za-z0-9()\\[\\]\\-_'.#]$";
        public static Regex MatchTitleRegex => new Regex(MatchTitleRegexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //public static Regex MatchTitleRegex => new Regex("^([A-Za-z0-9()\\[\\]\\-_'.#][ ]{0,1}){1,49}[A-Za-z0-9()\\[\\]\\-_'.#]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public const string RulesetNameRegexString = "^([A-Za-z0-9()\\[\\]\\-_'.][ ]{0,1}){1,49}[A-Za-z0-9()\\[\\]\\-_'.]$";
        public static Regex RulesetNameRegex => new Regex(RulesetNameRegexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string StripNonLettersOrDigits(string input)
        {
            char[] characters = input.ToCharArray();

            return new string(Array.FindAll(characters, (c => char.IsLetterOrDigit(c))));
        }

    }
}
