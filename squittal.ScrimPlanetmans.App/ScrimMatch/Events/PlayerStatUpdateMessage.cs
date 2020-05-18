using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class PlayerStatUpdateMessage
    {
        public Player Player { get; set; }

        public OverlayMessageData OverlayMessageData { get; set; }

        public string Info { get; set; } = string.Empty;

        public PlayerStatUpdateMessage(Player player)
        {
            Player = player;

            OverlayMessageData = new OverlayMessageData();

            //Info = $"Player Stat Update: {player.NameDisplay} [{player.Id}]";
            Info = Info = $"Player Stat Update: {player.NameDisplay} [{player.Id}]";
        }

        public PlayerStatUpdateMessage(Player player, OverlayMessageData overlayMessageData)
        {
            Player = player;

            OverlayMessageData = overlayMessageData;

            Info = GetInfo();
        }

        private string GetInfo()
        {
            return $"Player Stat Update: {Player.NameDisplay} [{Player.Id}]";
        }
    }
}
