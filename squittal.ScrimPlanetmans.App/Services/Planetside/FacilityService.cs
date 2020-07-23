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
    public class FacilityService : IFacilityService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusFacility _censusFacility;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<FacilityService> _logger;

        private ConcurrentDictionary<int, MapRegion> ScrimmableFacilityMapRegionsMap { get; set; } = new ConcurrentDictionary<int, MapRegion>();
        private readonly SemaphoreSlim _mapSetUpSemaphore = new SemaphoreSlim(1);

        public string BackupSqlScriptFileName => "dbo.MapRegion.Table.sql";

        public FacilityService(IDbContextHelper dbContextHelper, CensusFacility censusFacility, ISqlScriptRunner sqlScriptRunner, ILogger<FacilityService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusFacility = censusFacility;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public Task<MapRegion> GetMapRegionAsync(int mapRegionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<MapRegion> GetMapRegionFromFacilityIdAsync(int facilityId)
        {
            throw new System.NotImplementedException();
        }

        public Task<MapRegion> GetMapRegionFromFacilityNameAsync(string facilityName)
        {
            throw new System.NotImplementedException();
        }

        public Task<MapRegion> GetMapRegionsByFacilityTypeAsync(int facilityTypeId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<MapRegion>> GetScrimmableMapRegionsAsync()
        {
            if (ScrimmableFacilityMapRegionsMap.Count == 0 || !ScrimmableFacilityMapRegionsMap.Any())
            {
                await SetUpScrimmableMapRegionsAsync();
            }

            return GetScrimmableMapRegions();
        }

        private IEnumerable<MapRegion> GetScrimmableMapRegions()
        {
            return ScrimmableFacilityMapRegionsMap.Values.ToList();
        }

        public async Task<MapRegion> GetScrimmableMapRegionFromFacilityIdAsync(int facilityId)
        {
            if (ScrimmableFacilityMapRegionsMap.Count == 0 || !ScrimmableFacilityMapRegionsMap.Any())
            {
                await SetUpScrimmableMapRegionsAsync();
            }

            return GetScrimmableMapRegionFromFacilityId(facilityId);
        }

        private MapRegion GetScrimmableMapRegionFromFacilityId(int facilityId)
        {
            ScrimmableFacilityMapRegionsMap.TryGetValue(facilityId, out var mapRegion);

            return mapRegion;
        }

        public async Task SetUpScrimmableMapRegionsAsync()
        {
            var realZones = new List<int> { 2, 4, 6, 8 };
            var scrimFacilityTypes = new List<int> { 5, 6}; // Small Outpost, Large Outpost

            await _mapSetUpSemaphore.WaitAsync();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeRegions = await dbContext.MapRegions
                                                    .Where(region => realZones.Contains(region.ZoneId)
                                                                        && scrimFacilityTypes.Contains(region.FacilityTypeId))
                                                    .ToListAsync();

                foreach (var facilityId in ScrimmableFacilityMapRegionsMap.Keys)
                {
                    if (!storeRegions.Any(r => r.FacilityId == facilityId))
                    {
                        ScrimmableFacilityMapRegionsMap.TryRemove(facilityId, out var removedItem);
                    }
                }

                foreach (var region in storeRegions)
                {
                    if (ScrimmableFacilityMapRegionsMap.ContainsKey(region.FacilityId))
                    {
                        ScrimmableFacilityMapRegionsMap[region.FacilityId] = region;
                    }
                    else
                    {
                        ScrimmableFacilityMapRegionsMap.TryAdd(region.FacilityId, region);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting up Scrimmable Map Regions Map: {ex}");
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

                var anyMapRegions = await dbContext.MapRegions.AnyAsync();
                if (anyMapRegions)
                {
                    await SetUpScrimmableMapRegionsAsync();

                    return;
                }
            }

            var success = await RefreshStoreFromCensus();

            if (!success && canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }

            await SetUpScrimmableMapRegionsAsync();
        }

        public async Task<bool> RefreshStoreFromCensus()
        {
            IEnumerable<CensusMapRegionModel> mapRegions = new List<CensusMapRegionModel>();

            try
            {
                mapRegions = await _censusFacility.GetAllMapRegions();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all Map Regions. Refreshing store from backup...");
                return false;
            }

            if (mapRegions != null && mapRegions.Any())
            {
                await UpsertRangeAsync(mapRegions.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Map Regions store: {mapRegions.Count()} entries");

                return true;
            }
            else
            {
                return false;
            }

        }

        private async Task UpsertRangeAsync(IEnumerable<MapRegion> censusEntities)
        {
            var createdEntities = new List<MapRegion>();

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var storedEntities = await dbContext.MapRegions.ToListAsync();

                foreach (var censusEntity in censusEntities)
                {
                    var storeEntity = storedEntities.FirstOrDefault(e => e.Id == censusEntity.Id);
                    if (storeEntity == null)
                    {
                        createdEntities.Add(censusEntity);
                    }
                    else
                    {
                        storeEntity = censusEntity;
                        dbContext.MapRegions.Update(storeEntity);
                    }
                }

                if (createdEntities.Any())
                {
                    await dbContext.MapRegions.AddRangeAsync(createdEntities);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private MapRegion ConvertToDbModel(CensusMapRegionModel censusModel)
        {
            return new MapRegion
            {
                Id = censusModel.MapRegionId,
                FacilityId = censusModel.FacilityId,
                FacilityName = censusModel.FacilityName,
                FacilityTypeId = censusModel.FacilityTypeId,
                FacilityType = censusModel.FacilityType,
                ZoneId = censusModel.ZoneId
            };
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusFacility.GetMapRegionsCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MapRegions.CountAsync();
        }

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
