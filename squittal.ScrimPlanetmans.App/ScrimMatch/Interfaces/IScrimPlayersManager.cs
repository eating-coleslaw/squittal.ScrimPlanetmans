using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimPlayersManager
    {
        Task<Player> GetPlayerFromCharacterId(string characterId);
        Task<IEnumerable<Player>> GetPlayersFromOutfitAlias(string alias);
    }
}
