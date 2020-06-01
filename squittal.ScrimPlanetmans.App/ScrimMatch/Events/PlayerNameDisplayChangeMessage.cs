using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class PlayerNameDisplayChangeMessage
    {
        public Player Player { get; set; }
        public string OldNameDisplay { get; set; }
        public string NewNameDisplay { get; set; }

        public string Info { get; set; }

        public PlayerNameDisplayChangeMessage(Player player, string newNameDisplay, string oldNameDisplay)
        {
            Player = player;
            NewNameDisplay = !string.IsNullOrWhiteSpace(newNameDisplay) ? newNameDisplay : "null";
            OldNameDisplay = !string.IsNullOrWhiteSpace(oldNameDisplay) ? oldNameDisplay : "null";

            Info = $"Display Name for Team {Player.TeamOrdinal} player {Player.NameFull} [{Player.Id}] changed from {OldNameDisplay} to {NewNameDisplay}";
        }
    }
}
