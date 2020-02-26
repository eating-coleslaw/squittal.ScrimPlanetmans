using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class PlayerLogoutMessage
    {
        public Player Player { get; set; }
        public PlayerLogout Logout { get; set; }
        public string Info { get; set; } = string.Empty;

        public PlayerLogoutMessage(Player player, PlayerLogout logout)
        {
            Player = player;
            Logout = logout;

            Info = $"Team {Player.TeamOrdinal} player LOGOUT: [{Player.OutfitAlias}] {Player.NameDisplay} ({Player.Id})";
        }
    }
}
