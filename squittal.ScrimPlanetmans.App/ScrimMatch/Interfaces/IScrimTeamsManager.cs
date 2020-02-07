using squittal.ScrimPlanetmans.ScrimMatch.EventsTest;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimTeamsManager
    {
        event EventHandler<TeamPlayerChangeEventArgs> RaiseTeamPlayerChangeEvent;

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
