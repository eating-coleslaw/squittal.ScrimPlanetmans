using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ScrimTeamsManagerService
    {
        private readonly IScrimTeamsManager _teamsManager;

        public ScrimTeamsManagerService(IScrimTeamsManager teamsManager)
        {
            _teamsManager = teamsManager;
        }

        public Team GetTeamOne()
        {
            return _teamsManager.GetTeamOne();
        }
        public Team GetTeamTwo()
        {
            return _teamsManager.GetTeamTwo();
        }
        public Player GetPlayerFromId(string characterId)
        {
            return _teamsManager.GetPlayerFromId(characterId);
        }

        public void UpdateTeamAlias(int teamOrdinal, string alias)
        {
            _teamsManager.UpdateTeamAlias(teamOrdinal, alias);
        }

        public void SubmitPlayersList()
        {
            _teamsManager.SubmitPlayersList();
        }

        public async Task<bool> AddCharacterToTeam(int teamOrdinal, string characterId)
        {
            return await _teamsManager.AddCharacterToTeam(teamOrdinal, characterId);
        }
        public async Task<bool> AddOutfitAliasToTeam(int teamOrdinal, string alias)
        {
            return await _teamsManager.AddOutfitAliasToTeam(teamOrdinal, alias);
        }

        public async Task<bool> RemoveCharacterFromTeam(string characterId)
        {
            return await _teamsManager.RemoveCharacterFromTeam(characterId);
        }

        public bool IsCharacterAvailable(string characterId, out Team owningTeam)
        {
            return _teamsManager.IsCharacterAvailable(characterId, out owningTeam);

        }
        public bool IsOutfitAvailable(string alias, out Team owningTeam)
        {
            return _teamsManager.IsOutfitAvailable(alias, out owningTeam);
        }
    }
}
