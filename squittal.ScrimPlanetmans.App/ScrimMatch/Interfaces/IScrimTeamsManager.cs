using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimTeamsManager
    {
        Team GetTeamOne();
        Team GetTeamTwo();
        Player GetPlayerFromId(string characterId);

        void UpdateTeamAlias(int teamOrdinal, string alias);

        void SubmitPlayersList();

        Task<bool> AddCharacterToTeam(int teamOrdinal, string characterId);
        Task<bool> AddOutfitAliasToTeam(int teamOrdinal, string alias);
        
        Task<bool> RemoveCharacterFromTeam(string characterId);

        bool IsCharacterAvailable(string characterId, out Team owningTeam);
        bool IsOutfitAvailable(string alias, out Team owningTeam);

    }
}
