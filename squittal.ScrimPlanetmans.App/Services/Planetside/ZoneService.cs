using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.Planetside;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class ZoneService : IZoneService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusZone _censusZone;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<ZoneService> _logger;

        private ConcurrentDictionary<int, Zone> ZonesMap { get; set; } = new ConcurrentDictionary<int, Zone>();
        private readonly SemaphoreSlim _mapSetUpSemaphore = new SemaphoreSlim(1);
        
        public string BackupSqlScriptFileName => "CensusBackups\\dbo.Zone.Table.sql";

        public ZoneService(IDbContextHelper dbContextHelper, CensusZone censusZone, ISqlScriptRunner sqlScriptRunner, ILogger<ZoneService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusZone = censusZone;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<IEnumerable<Zone>> GetAllZones()
        {
            if (ZonesMap.Count == 0 || !ZonesMap.Any())
            {
                await SetupZonesMapAsync();
            }

            return ZonesMap.Values.ToList();
        }

        public async Task<IEnumerable<Zone>> GetAllZonesAsync()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Zones.ToListAsync();
            }
        }

        public async Task<Zone> GetZoneAsync(int zoneId)
        {
            if (ZonesMap.Count == 0 || !ZonesMap.Any())
            {
                await SetupZonesMapAsync();
            }

            ZonesMap.TryGetValue(zoneId, out var zone);

            return zone;
        }

        public async Task SetupZonesMapAsync()
        {
            await _mapSetUpSemaphore.WaitAsync();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeZones = await dbContext.Zones.ToListAsync();

                foreach (var zoneId in ZonesMap.Keys)
                {
                    if (!storeZones.Any(z => z.Id == zoneId))
                    {
                        ZonesMap.TryRemove(zoneId, out var removedZone);
                    }
                }

                foreach (var zone in storeZones)
                {
                    if (ZonesMap.ContainsKey(zone.Id))
                    {
                        ZonesMap[zone.Id] = zone;
                    }
                    else
                    {
                        ZonesMap.TryAdd(zone.Id, zone);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting up Zones Map: {ex}");
            }
            finally
            {
                _mapSetUpSemaphore.Release();
            }
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var anyZones = await dbContext.Zones.AnyAsync();
                if (anyZones)
                {
                    await SetupZonesMapAsync();

                    return;
                }
            }

            var success = await RefreshStoreFromCensus();

            if (!success && canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }

            await SetupZonesMapAsync();
        }

        public async Task<bool> RefreshStoreFromCensus()
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
                _logger.LogError("Census API query failed: get all Zones. Refreshing store from backup...");
                return false;
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

                return true;
            }
            else
            {
                return false;
            }
        }

        public static Zone ConvertToDbModel(CensusZoneModel censusModel)
        {
            return new Zone
            {
                Id = censusModel.ZoneId,
                Name = censusModel.Name.English,
                Description = censusModel.Description?.English ?? (string)null,
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
