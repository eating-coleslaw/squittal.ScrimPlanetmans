using squittal.ScrimPlanetmans.CensusStream.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public interface IEquitablePayload<T> : IEquitable<T> where T : PayloadBase
    {
    }
}
