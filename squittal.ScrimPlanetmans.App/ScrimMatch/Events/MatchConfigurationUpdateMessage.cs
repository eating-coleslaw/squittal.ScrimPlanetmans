using squittal.ScrimPlanetmans.Models.ScrimEngine;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class MatchConfigurationUpdateMessage
    {
        public MatchConfiguration MatchConfiguration { get; set; }
        public string Info { get; set; }

        public MatchConfigurationUpdateMessage(MatchConfiguration matchConfiguration)
        {
            MatchConfiguration = new MatchConfiguration();

            MatchConfiguration.CopyValues(matchConfiguration);

            Info = "Match Configuration updated";
        }
    }
}
