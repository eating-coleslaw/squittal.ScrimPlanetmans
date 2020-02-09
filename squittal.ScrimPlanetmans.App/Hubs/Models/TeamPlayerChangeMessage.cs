using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Hubs.Models
{
    public class TeamPlayerChangeMessage
    {
        public Player Player { get; set; }
        public string PlayerId { get; set; }
        public string PlayerNameDisplay { get; set; }
        public bool IsOnline { get; set; }
        public int TeamOrdinal { get; set; }

        public string OutfitId { get; set; } = string.Empty;
        public string OutfitAlias { get; set; } = string.Empty;
        public string OutfitAliasLower { get; set; } = string.Empty;

        //public string ChangeType { get; set; }
        public TeamPlayerChangeType ChangeType { get; set; }

        public string Info { get; set; } = string.Empty;

        public TeamPlayerChangeMessage(Player player)
        {
            Player = player;

            PlayerId = player.Id;
            PlayerNameDisplay = player.NameDisplay;
            TeamOrdinal = player.TeamOrdinal;
            IsOnline = player.IsOnline;

            OutfitId = player.OutfitId;
            OutfitAlias = player.OutfitAlias;
            OutfitAliasLower = player.OutfitAliasLower;
        }
    }

    public enum TeamPlayerChangeType
    {
        Add = 0,
        Remove = 1,
        SubstitueIn = 2,
        SubstitueOut = 3,
        SetActive = 4
    };
}
