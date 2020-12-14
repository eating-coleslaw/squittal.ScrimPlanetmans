namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamLockStatusChangeMessage
    {
        public int TeamOrdinal { get; set; }
        public bool IsLocked { get; set; }
        public string Info { get; set; }


        public TeamLockStatusChangeMessage(int teamOrdinal, bool newIsLocked)
        {
            TeamOrdinal = teamOrdinal;
            IsLocked = newIsLocked;

            Info = IsLocked ? $"Team {TeamOrdinal} locked" : $"Team {TeamOrdinal} unlocked";
        }
    }
}
