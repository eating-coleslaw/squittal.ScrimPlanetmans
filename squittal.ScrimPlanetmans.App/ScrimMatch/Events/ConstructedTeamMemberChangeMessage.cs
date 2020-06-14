using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.Models.Planetside;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ConstructedTeamMemberChangeMessage
    {
        public Character Character { get; set; }
        public string CharacterId { get; set; }
        public int TeamId { get; set; }
        public ConstructedTeamMemberChangeType ChangeType { get; set; }

        public string Info { get => GetInfoMessage(); }

        public ConstructedTeamMemberChangeMessage(int teamId, Character character, ConstructedTeamMemberChangeType changeType)
        {
            Character = character;
            CharacterId = character.Id;
            TeamId = teamId;
            ChangeType = changeType;
        }

        public ConstructedTeamMemberChangeMessage(int teamId, string characterId, ConstructedTeamMemberChangeType changeType)
        {
            CharacterId = characterId;
            TeamId = teamId;
            ChangeType = changeType;
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
