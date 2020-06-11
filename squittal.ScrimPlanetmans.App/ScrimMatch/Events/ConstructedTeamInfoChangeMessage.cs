using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ConstructedTeamInfoChangeMessage
    {
        public ConstructedTeam ConstructedTeam { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
        public bool NewIsHidden { get; set; }

        public string OldAlias { get; set; }
        public string NewAlias { get; set; }
        public bool OldIsHidden { get; set; }

        public string Info { get; set; }

        public ConstructedTeamInfoChangeMessage(ConstructedTeam constructedTeam, string oldName, string oldAlias, bool oldIsHidden)
        {
            ConstructedTeam = constructedTeam;

            NewName = ConstructedTeam.Name;
            NewAlias = ConstructedTeam.Alias;
            NewIsHidden = ConstructedTeam.IsHiddenFromSelection;

            OldName = oldName;
            OldAlias = oldAlias;
            OldIsHidden = oldIsHidden;


            Info = $"Info changed for Constructed Team {ConstructedTeam.Id}: [{OldAlias}] {OldName} {OldIsHidden.ToString().ToUpper()} => [{NewAlias}] {NewName} {NewIsHidden.ToString().ToUpper()}";
        }
    }
}
