using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class FactionService : IFactionService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusFaction _censusFaction;

        public FactionService(IDbContextHelper dbContextHelper, CensusFaction censusFaction)
        {
            _dbContextHelper = dbContextHelper;
            _censusFaction = censusFaction;
        }

        public async Task<IEnumerable<Faction>> GetAllFactionsAsync()
        {
            var factions = await _censusFaction.GetAllFactions();

            if (factions == null)
            {
                return null;
            }

            var censusEntities = factions.Select(ConvertToDbModel);
            
            return censusEntities;

            /*
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Factions.ToListAsync();
            }
            */
        }

        public async Task<Faction> GetFactionAsync(int factionId)
        {
            var factions = await GetAllFactionsAsync();
            return factions.FirstOrDefault(f => f.Id == factionId);
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
    }
}
