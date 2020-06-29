using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimKillfeedEventMessage
    {
        public ScrimKillfeedEvent KillfeedEvent { get; set; }

        public ScrimKillfeedEventMessage(ScrimKillfeedEvent killfeedEvent)
        {
            KillfeedEvent = killfeedEvent;
        }
    }
}
