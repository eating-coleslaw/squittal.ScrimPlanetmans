using squittal.ScrimPlanetmans.Data.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamConstructedTeamChangeMessage
    {
        public ConstructedTeam ConstructedTeam { get; set; }
        public int TeamOrdinal { get; set; }
        public TeamChangeType ChangeType { get; set; }
        public string Info { get; set; }

        public TeamConstructedTeamChangeMessage(int teamOrdinal, ConstructedTeam constructedTeam, TeamChangeType changeType)
        {
            TeamOrdinal = teamOrdinal;
            ConstructedTeam = constructedTeam;
            ChangeType = changeType;

            Info = GetInfoMessage();
        }

        private string GetInfoMessage()
        {
            if (ConstructedTeam == null)
            {
                return string.Empty;
            }

            var type = Enum.GetName(typeof(TeamChangeType), ChangeType).ToUpper();

            return $"Team {TeamOrdinal} Constructed Team {type}: [{ConstructedTeam.Alias}] {ConstructedTeam.Name} [{ConstructedTeam.Id}]";
        }
    }
}
