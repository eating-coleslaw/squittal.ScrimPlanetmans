using squittal.ScrimPlanetmans.Data.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamConstructedTeamChangeMessage
    {
        public ConstructedTeam ConstructedTeam { get; set; }
        public int TeamOrdinal { get; set; }
        public int FactionId { get; set; }

        public int? PlayersFound { get; set; }

        public TeamChangeType ChangeType { get; set; }
        public string Info { get; set; }

        public TeamConstructedTeamChangeMessage(int teamOrdinal, ConstructedTeam constructedTeam, int factionId, TeamChangeType changeType, int? playersFound = null)
        {
            TeamOrdinal = teamOrdinal;
            ConstructedTeam = constructedTeam;
            FactionId = factionId;
            ChangeType = changeType;

            PlayersFound = playersFound;

            Info = GetInfoMessage();
        }

        private string GetInfoMessage()
        {
            if (ConstructedTeam == null)
            {
                return string.Empty;
            }

            var type = Enum.GetName(typeof(TeamChangeType), ChangeType).ToUpper();

            return $"Team {TeamOrdinal} Constructed Team {type}: [{ConstructedTeam.Alias}] {ConstructedTeam.Name} [{ConstructedTeam.Id}] - Faction {FactionId}";
        }
    }
}
