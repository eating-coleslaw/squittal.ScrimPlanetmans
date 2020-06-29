using squittal.ScrimPlanetmans.ScrimMatch.Messages; 

namespace squittal.ScrimPlanetmans.Models.Forms
{
    public class ConstructedTeamCharacterChange
    {
        public string CharacterInput { get; set; }

        public TeamPlayerChangeType ChangeType { get; set; }
    }
}
