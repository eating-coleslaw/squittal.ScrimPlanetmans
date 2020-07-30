using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.Models.Planetside;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ConstructedTeamMemberChangeMessage
    {
        public Character Character { get; set; }
        public ConstructedTeamMemberDetails MemberDetails { get; set; }
        public string CharacterId { get; set; }
        public int TeamId { get; set; }
        public ConstructedTeamMemberChangeType ChangeType { get; set; }

        public string MemberAlias { get; set; }

        public string Info { get; set; } // => GetInfoMessage(); }

        // ADD Message
        public ConstructedTeamMemberChangeMessage(int teamId, Character character, ConstructedTeamMemberDetails memberDetails, ConstructedTeamMemberChangeType changeType)
        {
            Character = character;
            MemberDetails = memberDetails;
            CharacterId = character.Id;
            TeamId = teamId;
            ChangeType = changeType;

            MemberAlias = memberDetails.NameAlias;

            Info = GetInfoMessage();
        }

        // REMOVE Message
        public ConstructedTeamMemberChangeMessage(int teamId, string characterId, ConstructedTeamMemberChangeType changeType)
        {
            CharacterId = characterId;
            TeamId = teamId;
            ChangeType = changeType;

            Info = GetInfoMessage();
        }

        // UPDATE ALIAS Message
        public ConstructedTeamMemberChangeMessage(int teamId, string characterId, ConstructedTeamMemberChangeType changeType, string oldAlias, string newAlias)
        {
            CharacterId = characterId;
            TeamId = teamId;
            ChangeType = changeType;

            MemberAlias = newAlias;

            var type = Enum.GetName(typeof(ConstructedTeamMemberChangeType), ChangeType).ToUpper();

            var oldAliasDisplay = string.IsNullOrWhiteSpace(oldAlias) ? "null" : oldAlias;
            var newAliasDisplay = string.IsNullOrWhiteSpace(newAlias) ? "null" : newAlias;

            Info = $"Constructed Team {TeamId} Character {type}: {oldAliasDisplay} => {newAliasDisplay} [{CharacterId}]";
        }

        private string GetInfoMessage()
        {
            string characterName = string.Empty;
            
            if (Character != null)
            {
                characterName = Character.Name;
            }

            var type = Enum.GetName(typeof(ConstructedTeamMemberChangeType), ChangeType).ToUpper();

            return $"Constructed Team {TeamId} Character {type}: {characterName} [{CharacterId}]";
        }
    }
}
