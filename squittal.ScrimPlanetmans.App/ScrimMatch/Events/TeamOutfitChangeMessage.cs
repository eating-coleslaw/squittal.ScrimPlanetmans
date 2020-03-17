using squittal.ScrimPlanetmans.Models.Planetside;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class TeamOutfitChangeMessage
    {
        public Outfit Outfit { get; set; }
        public TeamChangeType ChangeType { get; set; }
        public string Info { get; set; }

        public TeamOutfitChangeMessage(Outfit outfit, TeamChangeType changeType)
        {
            Outfit = outfit;
            ChangeType = changeType;

            Info = GetInfoMessage();
        }

        private string GetInfoMessage()
        {
            if (Outfit == null)
            {
                return string.Empty;
            }

            var type = Enum.GetName(typeof(TeamChangeType), ChangeType).ToUpper();

            return $"Team {Outfit.TeamOrdinal} Outfit {type}: [{Outfit.Alias}] {Outfit.Name} [{Outfit.Id}]";
        }
    }
}
