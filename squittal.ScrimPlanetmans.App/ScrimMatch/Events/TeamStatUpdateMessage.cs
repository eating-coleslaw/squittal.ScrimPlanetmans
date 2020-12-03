using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamStatUpdateMessage
    {
        public Team Team { get; set; }

        public OverlayMessageData OverlayMessageData { get; set;}

        public string Info { get; set; } = string.Empty;

        public TeamStatUpdateMessage(Team team)
        {
            Team = team;

            OverlayMessageData = new OverlayMessageData();

            Info = GetInfo();
        }

        public TeamStatUpdateMessage(Team team, OverlayMessageData overlayMessageData)
        {
            Team = team;

            OverlayMessageData = overlayMessageData;

            Info = GetInfo();
        }

        private string GetInfo()
        {
            return $"Team {Team.TeamOrdinal} [{Team.Alias}] Stat Update";
        }
    }
}
