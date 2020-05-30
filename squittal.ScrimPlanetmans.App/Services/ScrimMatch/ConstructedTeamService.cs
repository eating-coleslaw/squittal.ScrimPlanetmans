using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ConstructedTeamService : IConstructedTeamService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IScrimTeamsManager _teamsManager;
        private readonly ICharacterService _characterService;
        private readonly ILogger<ConstructedTeamService> _logger;

        public string CurrentMatchId { get; set; }
        public int CurrentMatchRound { get; set; } = 0;

        public ConstructedTeamService(IDbContextHelper dbContextHelper, IScrimTeamsManager teamsManager, ICharacterService characterService, ILogger<ConstructedTeamService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _teamsManager = teamsManager;
            _characterService = characterService;
            _logger = logger;
        }

        public async Task<ConstructedTeam> GetConstructedTeam(int teamId, bool ignoreCollections = false)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var team = await dbContext.ConstructedTeams.FirstOrDefaultAsync(t => t.Id == teamId);

                if (ignoreCollections || team == null)
                {
                    return team;
                }

                team.FactionPreferences = await dbContext.ConstructedTeamFactionPreferences.Where(pref => pref.ConstructedTeamId == teamId).ToListAsync();
                team.PlayerMemberships = await dbContext.ConstructedTeamPlayerMemberships.Where(m => m.ConstructedTeamId == teamId).ToListAsync();

                return team;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return null;
            }
        }

        public async Task<ConstructedTeamMatchInfo> GetConstructedTeamMatchInfo(int teamId)
        {
                var constructedTeam = await GetConstructedTeam(teamId, false);

                if (constructedTeam == null)
                {
                    return null;
                }

                var teamInfo = ConvertToTeamMatchInfo(constructedTeam);

                var matchTeam = _teamsManager.GetTeamFromConstructedTeamId(teamId);

                if (matchTeam == null)
                {
                    return teamInfo;
                }

                teamInfo.TeamOrdinal = matchTeam.TeamOrdinal;

                if (!constructedTeam.PlayerMemberships.Any())
                {
                    return teamInfo;
                }

                var teamPlayers = new List<Player>();

                foreach (var member in constructedTeam.PlayerMemberships)
                {
                    var player = _teamsManager.GetPlayerFromId(member.CharacterId);
                    if (player != null)
                    {
                        teamPlayers.Add(player);
                        teamInfo.OnlineMembersCount += (player.IsOnline ? 1 : 0);

                        if (teamInfo.ActiveFactionId == null)
                        {
                            teamInfo.ActiveFactionId = player.FactionId;
                        }
                    }
                }

                teamInfo.Players = teamPlayers;

                return teamInfo;
        }

        private ConstructedTeamMatchInfo ConvertToTeamMatchInfo(ConstructedTeam constructedTeam)
        {
                return new ConstructedTeamMatchInfo
                {
                    Id = constructedTeam.Id,
                    Name = constructedTeam.Name,
                    Alias = constructedTeam.Alias,
                    FactionPreferences = constructedTeam.FactionPreferences,
                    TotalMembersCount = constructedTeam.PlayerMemberships.Count()
                };
        }

        public async Task<ConstructedTeamFormInfo> GetConstructedTeamFormInfo(int teamId)
        {
            var constructedTeam = await GetConstructedTeam(teamId, false);

            if (constructedTeam == null)
            {
                return null;
            }

            var teamInfo = ConvertToTeamFormInfo(constructedTeam);

            if (!constructedTeam.PlayerMemberships.Any())
            {
                return teamInfo;
            }

            var teamCharacters = new List<Character>();

            foreach (var member in constructedTeam.PlayerMemberships)
            {
                try
                {
                    var character = await _characterService.GetCharacterAsync(member.CharacterId);

                    if (character != null)
                    {
                        teamCharacters.Add(character);
                    }
                    else
                    {
                        _logger.LogError($"Census API returned no data for characterId {member.CharacterId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error fetching Census API data for characterId {member.CharacterId}: {ex}");
                }

                
            }

            teamInfo.Characters = teamCharacters;

            return teamInfo;
        }

        private ConstructedTeamFormInfo ConvertToTeamFormInfo(ConstructedTeam constructedTeam)
        {
            return new ConstructedTeamFormInfo
            {
                Id = constructedTeam.Id,
                Name = constructedTeam.Name,
                Alias = constructedTeam.Alias,
                FactionPreferences = constructedTeam.FactionPreferences
            };
        }

        public async Task AddConstructedTeamToMatch(int constructedTeamId, int matchTeamOrdinal, int factionId)
        {
            throw new NotImplementedException();
        }

        

        public async Task<IEnumerable<ConstructedTeam>> GetConstructedTeams(bool ignoreCollections = false)
        {
            throw new NotImplementedException();
        }

        public async Task SaveConstructedTeam(ConstructedTeam constructedTeam)
        {
            throw new NotImplementedException();
        }

        
    }
}
