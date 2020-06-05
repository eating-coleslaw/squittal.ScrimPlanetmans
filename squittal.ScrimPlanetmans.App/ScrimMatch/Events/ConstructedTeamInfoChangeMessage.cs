using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ConstructedTeamInfoChangeMessage
    {
        public ConstructedTeam ConstructedTeam { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }

        public string OldAlias { get; set; }
        public string NewAlias { get; set; }

        public string Info { get; set; }

        public ConstructedTeamInfoChangeMessage(ConstructedTeam constructedTeam, string oldName, string oldAlias)
        {
            ConstructedTeam = constructedTeam;

            NewName = ConstructedTeam.Name;
            NewAlias = ConstructedTeam.Alias;

            OldName = oldName;
            OldAlias = oldAlias;

            Info = $"Info changed for Constructed Team {ConstructedTeam.Id}: [{OldAlias}] {OldName} => [{NewAlias}] {NewName}";
        }
    }
}
