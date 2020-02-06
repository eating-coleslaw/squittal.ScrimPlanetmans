using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Hubs.Models
{
    public class TeamPlayerChangeMessage
    {
        public Player Player { get; set; }
        public int TeamOrdinal { get; set; }
        public TeamPlayerChangeType ChangeType { get; set; }
        public string Info { get; set; } = string.Empty;
    }

    public enum TeamPlayerChangeType
    {
        Add = 0,
        Remove = 1
    };
}
