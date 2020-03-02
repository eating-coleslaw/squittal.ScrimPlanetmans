using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IScrimPlayersService
    {
        Task<Player> GetPlayerFromCharacterId(string characterId);
        Task<Player> GetPlayerFromCharacterName(string characterName);
        Task<IEnumerable<Player>> GetPlayersFromOutfitAlias(string alias);
    }
}
