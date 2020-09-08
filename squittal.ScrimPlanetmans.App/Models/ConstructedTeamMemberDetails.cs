namespace squittal.ScrimPlanetmans.Models
{
    public class ConstructedTeamMemberDetails
    {
        public string CharacterId { get; set; }
        public int ConstructedTeamId { get; set; }
        public int FactionId { get; set; }

        public string NameFull { get; set; }
        public string NameAlias { get; set; }

        public string NameDisplay
        {
            get
            {
                return string.IsNullOrWhiteSpace(NameAlias) ? NameFull : NameAlias;
            }
        }

        public int? WorldId { get; set; }
        public int? PrestigeLevel { get; set; }
        public bool IsMatchParticipant { get; set; } = false;
        public bool IsDeleteAllowed { get => !IsMatchParticipant; }
    }
}
