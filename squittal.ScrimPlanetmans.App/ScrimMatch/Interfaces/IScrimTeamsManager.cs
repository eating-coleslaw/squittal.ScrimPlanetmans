using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimTeamsManager
    {
        Team GetTeam(int teamOrdinal);
        string GetTeamAliasDisplay(int teamOrdinal);

        Team GetTeamOne();
        Team GetTeamTwo();

        Player GetPlayerFromId(string characterId);

        IEnumerable<string> GetAllPlayerIds();
        IEnumerable<Player> GetParticipatingPlayers();

        //void UpdateTeamAlias(int teamOrdinal, string alias);
        bool UpdateTeamAlias(int teamOrdinal, string alias, bool isCustom = false);

        //void SubmitPlayersList();

        Task<bool> AddCharacterToTeam(int teamOrdinal, string characterId);
        Task<bool> AddOutfitAliasToTeam(int teamOrdinal, string aliasLower, string alias);

        Task<bool> RefreshOutfitPlayers(string aliasLower);

        bool RemoveOutfitFromTeam(string aliasLower);
        bool RemoveCharacterFromTeam(string characterId);

        void ClearAllTeams();
        void ClearTeam(int teamOrdinal);

        bool IsCharacterAvailable(string characterId, out Team owningTeam);
        bool IsCharacterAvailable(string characterId);

        bool IsOutfitAvailable(string alias, out Team owningTeam);

        int? GetTeamOrdinalFromPlayerId(string characterId);
        bool DoPlayersShareTeam(string firstId, string secondId, out int? firstOrdinal, out int? secondOrdinal);
        bool IsPlayerTracked(string characterId);
        void UpdatePlayerStats(string characterId, ScrimEventAggregate updates);
        Team GetTeamFromOutfitAlias(string aliasLower);
        void SetPlayerOnlineStatus(string characterId, bool isOnline);
        void SetPlayerLoadoutId(string characterId, int? loadoutId);
        bool DoPlayersShareTeam(Player firstPlayer, Player secondPlayer);
        bool IsOutfitAvailable(string alias);
        Task<bool> TryAddCharacterToTeam(int teamOrdinal, string inputString);
        
    }
}
