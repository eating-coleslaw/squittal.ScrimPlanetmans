using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

//using Microsoft.EntityFrameworkCore;
//using squittal.ScrimPlanetmans.Data;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class WorldService : IWorldService
    {
        //private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusWorld _censusWorld;

        public WorldService(/*IDbContextHelper dbContextHelper,*/ CensusWorld censusWorld)
        {
            //_dbContextHelper = dbContextHelper;
            _censusWorld = censusWorld;
        }


        public async Task<IEnumerable<World>> GetAllWorldsAsync()
        {
            var worlds = await _censusWorld.GetAllWorlds();

            if (worlds == null)
            {
                return null;
            }

            var censusEntities = worlds.Select(ConvertToDbModel);

            return censusEntities.ToList();

            /*
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Worlds.ToListAsync();
            }
            */
        }

        public async Task<World> GetWorldAsync(int worldId)
        {
            var worlds = await GetAllWorldsAsync();
            return worlds.FirstOrDefault(e => e.Id == worldId);
        }

        /*
        public async Task RefreshStore()
        {
            var result = new List<World>();
            var createdEntities = new List<World>();
            
            var worlds = await _censusWorld.GetAllWorlds();
            
            if (worlds != null)
            {
                var censusEntities = worlds.Select(ConvertToDbModel);

                using (var factory = _dbContextHelper.GetFactory())
                {
                    var dbContext = factory.GetDbContext();

                    var storedEntities = await dbContext.Worlds.ToListAsync();

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
                            dbContext.Worlds.Update(storeEntity);
                        }
                    }

                    if (createdEntities.Any())
                    {
                        await dbContext.Worlds.AddRangeAsync(createdEntities);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }
        */

        public static World ConvertToDbModel(CensusWorldModel censusModel)
        {
            return new World
            {
                Id = censusModel.WorldId,
                Name = censusModel.Name.English
            };
        }
    }
}
