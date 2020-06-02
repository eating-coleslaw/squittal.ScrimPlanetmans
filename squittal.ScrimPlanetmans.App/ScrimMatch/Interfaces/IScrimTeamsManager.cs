using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public interface IScrimTeamsManager
    {
        MaxPlayerPointsTracker MaxPlayerPointsTracker { get; }

        Team GetTeam(int teamOrdinal);
        string GetTeamAliasDisplay(int teamOrdinal);

        Team GetTeamOne();
        Team GetTeamTwo();

        Player GetPlayerFromId(string characterId);

        IEnumerable<string> GetAllPlayerIds();
        IEnumerable<Player> GetParticipatingPlayers();

        bool UpdateTeamAlias(int teamOrdinal, string alias, bool isCustom = false);

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
        
        Team GetTeamFromOutfitAlias(string aliasLower);
        
        bool DoPlayersShareTeam(Player firstPlayer, Player secondPlayer);
        bool IsOutfitAvailable(string alias);
        Task<bool> TryAddCharacterToTeam(int teamOrdinal, string inputString);

        void UpdatePlayerStats(string characterId, ScrimEventAggregate updates);
        void SetPlayerOnlineStatus(string characterId, bool isOnline);
        void SetPlayerLoadoutId(string characterId, int? loadoutId);
        void SetPlayerParticipatingStatus(string characterId, bool isParticipating);
        void SetPlayerBenchedStatus(string characterId, bool isBenched);

        Task SaveRoundEndScores(int round);
        Task RollBackAllTeamStats(int currentRound);
        
        int? GetNextWorldId(int previousWorldId);
        int? GetFirstTeamWithFactionId(int factionId);
        void UpdateTeamStats(int teamOrdinal, ScrimEventAggregate updates);

        Task AdjustTeamPoints(int teamOrdinal, PointAdjustment adjustment);
        Task RemoveTeamPointAdjustment(int teamOrdinal, PointAdjustment adjustment);

        Task<bool> RemoveOutfitFromTeamAndDb(string aliasLower);
        Task<bool> RemoveCharacterFromTeamAndDb(string characterId);
        int? GetTeamScoreDisplay(int teamOrdinal);
        
        bool IsConstructedTeamAvailable(int constructedTeamId, out Team owningTeam);
        Team GetTeamFromConstructedTeamId(int constructedTeamId);

        bool UdatePlayerTemporaryAlias(string playerId, string newAlias);
        void ClearPlayerDisplayName(string playerId);
    }
}
