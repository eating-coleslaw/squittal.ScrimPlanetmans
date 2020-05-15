namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamAliasChangeMessage
    {
        public int TeamOrdinal { get; set; }
        public string NewAlias { get; set; }
        public string OldAlias { get; set; }

        public string Info { get; set; }

        public TeamAliasChangeMessage(int teamOrdinal, string newAlias, string oldAlias)
        {
            TeamOrdinal = teamOrdinal;
            NewAlias = newAlias;
            OldAlias = oldAlias;

            Info = $"Alias for Team {TeamOrdinal} changed from {OldAlias} to {NewAlias}";
        }
    }
}
