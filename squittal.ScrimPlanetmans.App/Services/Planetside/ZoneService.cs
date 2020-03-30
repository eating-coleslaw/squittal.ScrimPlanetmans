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
    public class ZoneService : IZoneService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusZone _censusZone;

        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<ZoneService> _logger;

        public string BackupSqlScriptFileName => "dbo.Zone.Table.sql";


        private List<Zone> _zones = new List<Zone>();

        public ZoneService(IDbContextHelper dbContextHelper, CensusZone censusZone, ISqlScriptRunner sqlScriptRunner, ILogger<ZoneService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusZone = censusZone;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<IEnumerable<Zone>> GetAllZonesAsync()
        {
            //var Zones = await _censusZone.GetAllZones();
            //if (Zones == null)
            //{
            //    return null;
            //}

            //var censusEntities = Zones.Select(ConvertToDbModel);

            //return censusEntities.ToList();


            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Zones.ToListAsync();
            }
        }

        public IEnumerable<Zone> GetAllZones()
        {
            return _zones;
        }

        public async Task<Zone> GetZoneAsync(int ZoneId)
        {
            var Zones = await GetAllZonesAsync();
            return Zones.FirstOrDefault(e => e.Id == ZoneId);
        }

        public async Task SetupZonesList()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            _zones = await dbContext.Zones.ToListAsync();
        }

        // TODO: actually implement onlyQueryCensusIfEmpty = true
        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false)
        {
            await RefreshStore();
        }

        public async Task RefreshStore()
        {
            var result = new List<Zone>();
            var createdEntities = new List<Zone>();

            IEnumerable<CensusZoneModel> zones = new List<CensusZoneModel>();

            try
            {
                zones = await _censusZone.GetAllZones();
            }
            catch
            {
                _logger.LogError("Census API query failes: get all Zones");
                return;
            }

            if (zones != null && zones.Any())
            {
                var censusEntities = zones.Select(ConvertToDbModel);

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

                    _logger.LogInformation($"Refreshed Zones store");
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

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusZone.GetZonesCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Zones.CountAsync();
        }

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
