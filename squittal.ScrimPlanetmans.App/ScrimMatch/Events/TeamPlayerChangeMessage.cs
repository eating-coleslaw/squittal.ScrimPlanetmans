using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
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

        public bool IsOutfitless { get; set; } = false; // assume most players will be added via outfits
        public bool IsLastOfOutfit { get; set; } = false;

        public TeamPlayerChangeType ChangeType { get; set; }

        public string Info { get => GetInfoMessage(); }

        public TeamPlayerChangeMessage(Player player, TeamPlayerChangeType type, bool isLastOfOutfit = false)
        {
            Player = player;
            ChangeType = type;

            PlayerId = player.Id;
            PlayerNameDisplay = player.NameDisplay;
            TeamOrdinal = player.TeamOrdinal;
            IsOnline = player.IsOnline;

            OutfitId = player.OutfitId;
            OutfitAlias = player.OutfitAlias;
            OutfitAliasLower = player.OutfitAliasLower;

            IsOutfitless = (!string.IsNullOrWhiteSpace(OutfitAliasLower));
            IsLastOfOutfit = isLastOfOutfit;
        }

        private string GetInfoMessage()
        {
            if (Player == null)
            {
                return string.Empty;
            }

            var type = ChangeType != TeamPlayerChangeType.Default
                            ? Enum.GetName(typeof(TeamPlayerChangeType), ChangeType).ToUpper()
                            : string.Empty;

            var online = Player.IsOnline == true
                            ? " ONLINE"
                            : string.Empty;

            return $"Team {Player.TeamOrdinal} {type}: {Player.NameDisplay} [{Player.Id}]{online}";
        }
    }

    // TODO: replace all instances of this with TeamChangeType
    public enum TeamPlayerChangeType
    {
        Default = 0,
        Add = 1,
        Remove = 2,
        SubstitueIn = 3,
        SubstitueOut = 4,
        SetActive = 5
    };

    public enum TeamChangeType
    {
        Default = 0,
        Add = 1,
        Remove = 2
    }
}
