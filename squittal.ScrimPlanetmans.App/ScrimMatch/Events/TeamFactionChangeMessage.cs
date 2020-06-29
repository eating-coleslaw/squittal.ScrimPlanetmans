namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamFactionChangeMessage
    {
        public int TeamOrdinal { get; set; }
        public int? NewFactionId { get; set; }
        public string NewFactionAbbreviation { get; set; }

        public int? OldFactionId { get; set; }
        public string OldFactionAbbreviation { get; set; }

        public string Info { get; set; }

        public TeamFactionChangeMessage(int teamOrdinal, int? newFactionId, string newFactionAbbreviation, int? oldFactionId, string oldFactionAbbreviation)
        {
            TeamOrdinal = teamOrdinal;
            NewFactionId = newFactionId;
            NewFactionAbbreviation = newFactionAbbreviation;
            OldFactionId = oldFactionId;
            OldFactionAbbreviation = oldFactionAbbreviation;

            var newIdDisplay = NewFactionId == null ? string.Empty : $"{NewFactionId.ToString()}-";
            var oldIdDisplay = OldFactionId == null ? string.Empty : $"{OldFactionId.ToString()}-";

            Info = $"Alias for Team {TeamOrdinal} changed from {oldIdDisplay}{OldFactionAbbreviation} to {newIdDisplay}{NewFactionAbbreviation}";
        }
    }
}
