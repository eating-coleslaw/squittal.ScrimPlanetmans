using squittal.ScrimPlanetmans.CensusStream.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public interface IEquitablePayload<T> : IEquitable<T> where T : PayloadBase
    {
        //public string EventName { get; set; }
        //public int WorldId { get; set; }
        //public int? ZoneId { get; set; }
        //public DateTime Timestamp { get; set; }
    }
}
