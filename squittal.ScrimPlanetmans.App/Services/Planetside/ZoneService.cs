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
    public class ZoneService : IZoneService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusZone _censusZone;

        public ZoneService(IDbContextHelper dbContextHelper, CensusZone censusZone)
        {
            _dbContextHelper = dbContextHelper;
            _censusZone = censusZone;
        }

        public async Task<IEnumerable<Zone>> GetAllZonesAsync()
        {
            var Zones = await _censusZone.GetAllZones();
            if (Zones == null)
            {
                return null;
            }

            var censusEntities = Zones.Select(ConvertToDbModel);

            return censusEntities.ToList();

            /*
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Zones.ToListAsync();
            }
            */
        }

        public async Task<Zone> GetZoneAsync(int ZoneId)
        {
            var Zones = await GetAllZonesAsync();
            return Zones.FirstOrDefault(e => e.Id == ZoneId);
        }

        public async Task RefreshStore()
        {
            var result = new List<Zone>();
            var createdEntities = new List<Zone>();

            var Zones = await _censusZone.GetAllZones();

            if (Zones != null)
            {
                var censusEntities = Zones.Select(ConvertToDbModel);

                using (var factory = _dbContextHelper.GetFactory())
                {
                    var dbContext = factory.GetDbContext();

                    var storedEntities = await dbContext.Zones.ToListAsync();

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
                            dbContext.Zones.Update(storeEntity);
                        }
                    }

                    if (createdEntities.Any())
                    {
                        await dbContext.Zones.AddRangeAsync(createdEntities);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public static Zone ConvertToDbModel(CensusZoneModel censusModel)
        {
            return new Zone
            {
                Id = censusModel.ZoneId,
                Name = censusModel.Name.English,
                Description = censusModel.Description.English,
                Code = censusModel.Code,
                HexSize = censusModel.HexSize
            };
        }
    }
}
