using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class FactionService : IFactionService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusFaction _censusFaction;
        private readonly ILogger<FactionService> _logger;

        public FactionService(IDbContextHelper dbContextHelper, CensusFaction censusFaction, ILogger<FactionService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusFaction = censusFaction;
            _logger = logger;
        }

        public async Task<IEnumerable<Faction>> GetAllFactionsAsync()
        {
            //var factions = await _censusFaction.GetAllFactions();

            //if (factions == null)
            //{
            //    return null;
            //}

            //var censusEntities = factions.Select(ConvertToDbModel);

            //return censusEntities;


            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Factions.ToListAsync();
            }

        }

        public async Task<Faction> GetFactionAsync(int factionId)
        {
            var factions = await GetAllFactionsAsync();
            return factions.FirstOrDefault(f => f.Id == factionId);
        }

        public string GetFactionAbbrevFromId(int factionId)
        {
            return factionId switch
            {
                1 => "VS",
                2 => "NC",
                3 => "TR",
                4 => "NSO",
                _ => string.Empty
            };
        }

        public async Task RefreshStore()
        {
            var createdEntities = new List<Faction>();

            var factions = await _censusFaction.GetAllFactions();

            if (factions != null)
            {
                var censusEntities = factions.Select(ConvertToDbModel);

                using (var factory = _dbContextHelper.GetFactory())
                {
                    var dbContext = factory.GetDbContext();

                    var storedEntities = await dbContext.Factions.ToListAsync();

                    foreach (var censusEntity in censusEntities)
                    {
                        var storeEntity = storedEntities.FirstOrDefault(storedEntity => storedEntity.Id == censusEntity.Id);
                        if (storeEntity == null)
                        {
                            createdEntities.Add(censusEntity);
                        }
                        else
                        {
                            storeEntity = censusEntity;
                            dbContext.Factions.Update(storeEntity);
                        }
                    }

                    if (createdEntities.Any())
                    {
                        await dbContext.Factions.AddRangeAsync(createdEntities);
                    }

                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Refreshed Factions store");
                }
            }
        }

        public static Faction ConvertToDbModel(CensusFactionModel censusModel)
        {
            return new Faction
            {
                Id = censusModel.FactionId,
                Name = censusModel.Name.English,
                ImageId = censusModel.ImageId,
                CodeTag = censusModel.CodeTag,
                UserSelectable = censusModel.UserSelectable
            };
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusFaction.GetFactionsCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Factions.CountAsync();
        }
    }
}
