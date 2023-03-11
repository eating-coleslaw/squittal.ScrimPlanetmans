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
        //bool RemoveCharacterFromTeam(string characterId);

        void ClearAllTeams();
        void ClearTeam(int teamOrdinal);

        bool IsCharacterAvailable(string characterId, out Team owningTeam);
        bool IsCharacterAvailable(string characterId);

        bool IsOutfitAvailable(string alias, out Team owningTeam);

        int? GetTeamOrdinalFromPlayerId(string characterId);
        bool DoPlayersShareTeam(string firstId, string secondId, out int? firstOrdinal, out int? secondOrdinal);
        
        Team GetTeamFromOutfitAlias(string aliasLower);
        
        bool DoPlayersShareTeam(Player firstPlayer, Player secondPlayer);
        bool IsOutfitAvailable(string alias);
        Task<bool> TryAddFreeTextInputCharacterToTeam(int teamOrdinal, string inputString);

        Task UpdatePlayerStats(string characterId, ScrimEventAggregate updates);
        void SetPlayerOnlineStatus(string characterId, bool isOnline);
        void SetPlayerLoadoutId(string characterId, int? loadoutId);
        Task SetPlayerParticipatingStatus(string characterId, bool isParticipating);
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
        
        Task<bool> UdatePlayerTemporaryAlias(string playerId, string newAlias);
        Task ClearPlayerDisplayName(string playerId);

        Task<bool> AddConstructedTeamFactionMembersToTeam(int teamOrdinal, int constructedTeamId, int factionId);
        IEnumerable<Player> GetTeamOutfitPlayers(int teamOrdinal, string outfitAliasLower);
        IEnumerable<Player> GetTeamNonOutfitPlayers(int teamOrdinal);
        IEnumerable<Player> GetTeamConstructedTeamFactionPlayers(int teamOrdinal, int constructedTeamId, int factionId);
        Task<bool> RemoveConstructedTeamFactionFromTeamAndDb(int constructedTeamId, int factionId);
        bool RemoveConstructedTeamFactionFromTeam(int constructedTeamId, int factionId);
        bool IsConstructedTeamFactionAvailable(int constructedTeamId, int factionId, out Team owningTeam);
        bool IsConstructedTeamFactionAvailable(int constructedTeamId, int factionId);
        Team GetTeamFromConstructedTeamFaction(int constructedTeamId, int factionId);
        bool IsConstructedTeamAnyFactionAvailable(int constructedTeamId);
        void ResetAllTeamsMatchData();
        Task LockTeamPlayers(int teamOrdinal);
        void UnlockTeamPlayers(int teamOrdinal);
        bool GetTeamLockStatus(int teamOrdinal);
        void UnlockAllTeamPlayers();
        int GetCurrentMatchRoundTeamBaseControlsCount(int teamOrdinal);
        int GetCurrentMatchRoundBaseControlsCount();
        int GetCurrentMatchRoundWeightedCapturesCount();
        int GetCurrentMatchRoundTeamWeightedCapturesCount(int teamOrdinal);
        void UpdateAllTeamsMatchSeriesResults(int seriesMatchNumber);
        void UpdateAllTeamsMatchSeriesResults(int teamOrdinal, int seriesMatchNumber, ScrimSeriesMatchResultType matchResultType);
        List<ScrimSeriesMatchResult> GetTeamsScrimSeriesMatchResults(int teamOrdinal);
        int GetEnemyTeamOrdinal(int teamOrdinal);
        int? GetTeamCurrentRoundScoreDisplay(int teamOrdinal);
        int? GetTeamRoundScoreDisplay(int teamOrdinal, int matchRound);
        void ClearPlayerLastKilledByMap();
        bool TrySetPlayerLastKilledBy(string victimId, string attackerId);
        Player GetLastKilledByPlayer(string victimId);
    }
}
