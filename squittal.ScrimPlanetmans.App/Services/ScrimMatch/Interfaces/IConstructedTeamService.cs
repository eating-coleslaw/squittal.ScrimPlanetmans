using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IConstructedTeamService
    {
        Task<IEnumerable<ConstructedTeam>> GetConstructedTeams(bool ignoreCollections = false, bool includeHiddenTeams = false);

        Task<ConstructedTeam> GetConstructedTeam(int teamId, bool ignoreCollections = false);

        Task<ConstructedTeamFormInfo> GetConstructedTeamFormInfo(int teamId, bool ignoreCollections = false);

        Task<ConstructedTeam> CreateConstructedTeam(ConstructedTeam constructedTeam);
        Task SaveConstructedTeam(ConstructedTeamFormInfo constructedTeamFormInfo);

        //Task<Character> TryAddCharacterToConstructedTeam(int teamId, string characterInput);
        Task<Character> TryAddCharacterToConstructedTeam(int teamId, string characterInput, string customAlias);
        
        Task<bool> IsCharacterIdOnTeam(int teamId, string characterId);
        Task<bool> TryRemoveCharacterFromConstructedTeam(int teamId, string characterId);
        Task<bool> UpdateConstructedTeamInfo(ConstructedTeam teamUpdate);
        Task<int> GetConstructedTeamMemberCount(int teamId);
        Task<IEnumerable<string>> GetConstructedTeamFactionMemberIds(int teamId, int factionId);
        Task<IEnumerable<ConstructedTeamPlayerMembership>> GetConstructedTeamFactionMembers(int teamId, int factionId);
        Task<IEnumerable<Character>> GetConstructedTeamFactionCharacters(int teamId, int factionId);
        Task<IEnumerable<ConstructedTeamMemberDetails>> GetConstructedTeamFactionMemberDetails(int teamId, int factionId);
        Task<IEnumerable<Player>> GetConstructedTeamFactionPlayers(int teamId, int factionId);
        Task<bool> TryUpdateMemberAlias(int teamId, string characterId, string oldAlias, string newAlias);
        Task<bool> TryClearMemberAlias(int teamId, string characterId, string oldAlias);
    }
}
