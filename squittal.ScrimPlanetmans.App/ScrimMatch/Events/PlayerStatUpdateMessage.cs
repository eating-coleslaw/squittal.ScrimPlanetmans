using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Events
{
    public class PlayerStatUpdateMessage
    {
        public Player Player { get; set; }

        public string Info { get; set; } = string.Empty;

        public PlayerStatUpdateMessage(Player player)
        {
            Player = player;

            Info = $"Player Stat Update: {player.NameDisplay} [{player.Id}]";
        }
    }
}
