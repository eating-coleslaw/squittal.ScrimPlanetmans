namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamStatUpdateMessage
    {
        public Team Team { get; set; }

        public string Info { get; set; } = string.Empty;

        public TeamStatUpdateMessage(Team team)
        {
            Team = team;

            Info = $"Team {team.TeamOrdinal} [{team.Alias}] Stat Update";
        }
    }
}
