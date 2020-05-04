using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ScrimMatchDataService : IScrimMatchDataService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly ILogger<ScrimMatchDataService> _logger;

        public string CurrentMatchId { get ; set; }
        public int CurrentMatchRound { get; set; } = 0;

        public ScrimMatchDataService(IDbContextHelper dbContextHelper, ILogger<ScrimMatchDataService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _logger = logger;
        }

        IEnumerable<Data.Models.ScrimMatch> IScrimMatchDataService.GetAllMatches()
        {
            throw new NotImplementedException();
        }

        public async Task<Data.Models.ScrimMatch> GetCurrentMatch()
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                return await dbContext.ScrimMatches.FirstOrDefaultAsync(sm => sm.Id == CurrentMatchId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                
                return null;
            }
        }

        public async Task SaveToCurrentMatch(Data.Models.ScrimMatch scrimMatch)
        {
            var id = scrimMatch.Id;

            var oldMatchId = CurrentMatchId;

            CurrentMatchId = id;

            try
            {
                CurrentMatchId = id;

                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeEntity = await dbContext.ScrimMatches.FirstOrDefaultAsync(sm => sm.Id == id);

                if (storeEntity == null)
                {
                    dbContext.ScrimMatches.Add(scrimMatch);
                }
                else
                {
                    storeEntity = scrimMatch;
                    dbContext.ScrimMatches.Update(storeEntity);
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                CurrentMatchId = oldMatchId;

                _logger.LogError(ex.ToString());
            }
        }
    }
}
