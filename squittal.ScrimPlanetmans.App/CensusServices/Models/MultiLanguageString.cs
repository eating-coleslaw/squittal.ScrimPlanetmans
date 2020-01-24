using Newtonsoft.Json;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class MultiLanguageString
    {
        [JsonProperty("en")]
        public string English { get; set; }
    }
}
