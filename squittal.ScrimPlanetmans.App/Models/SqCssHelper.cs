namespace squittal.ScrimPlanetmans.Models
{
    public static class SqCssHelper
    {
        public static string GetFactionClassFromId(int? factionId)
        {
            var cssClass = factionId switch
            {
                1 => "vs",
                2 => "nc",
                3 => "tr",
                4 => "ns",
                _ => "ns",
            };
            return cssClass;
        }

        public static string GetZoneDisplayEmojiFromName(string zoneName)
        {
            return zoneName switch
            {
                "Amerish" => "🗻",
                "Esamir" => "❄️",
                "Hossin" => "🌳",
                "Indar" => "☀️",
                _ => "❔",
            };
        }
    }
}
