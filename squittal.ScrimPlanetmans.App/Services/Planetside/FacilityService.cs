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

        public string BackupSqlScriptFileName => "CensusBackups\\dbo.MapRegion.Table.sql";

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
            await _mapSetUpSemaphore.WaitAsync();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeRegions = await GetAllStoredScrimmableZoneMapRegionsAsync();

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

        private async Task<IEnumerable<MapRegion>> GetAllStoredScrimmableZoneMapRegionsAsync()
        {
            var realZones = new List<int> { 2, 4, 6, 8, 344 };

            var nonScrimFacilityTypes = new List<int>
            {
                1, // Default
                7, // Warpgate
                10 // Relic Outpost (Desolation)
            };

            var scrimFacilityTypes = new List<int>
            {
                2,  // Amp Station
                3,  // Bio Lab
                4,  // Tech Plant
                5,  // Large Outpost
                6,  // Small Outpost
                8,  // Interlink Facility
                9,  // Construction Outpost
                11, // Containment Site
                12, // Trident Relay
                13, // Seapost
                14, // Large CTF Outpost
                15, // Small CTF Outpost
                16, // Amp Station CTF
                17  // Construction Outpost CTF
            };

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();
            
            return await dbContext.MapRegions.Where(region => realZones.Contains(region.ZoneId) && !nonScrimFacilityTypes.Contains(region.FacilityTypeId) && region.IsCurrent)
                                             .ToListAsync();
        }

        private async Task<IEnumerable<MapRegion>> GetAllStoreMapRegionsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MapRegions.ToListAsync();
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                if (await GetStoreCountAsync() > 0)
                {
                    await SetUpScrimmableMapRegionsAsync();

                    return;
                }
            }

            // Always use backup script if available to get old bases that aren't in the Census API
            if (canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }

            await RefreshStoreFromCensus();

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

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var storeEntities = await GetAllStoreMapRegionsAsync();

            var allEntities = new List<MapRegion>(censusEntities.Select(e => new MapRegion() { Id = e.Id, FacilityId = e.FacilityId }));

            allEntities.AddRange(storeEntities
                                    .Where(s => !allEntities.Any(c => c.Id == s.Id && c.FacilityId == s.FacilityId))
                                    .Select(e => new MapRegion() { Id = e.Id, FacilityId = e.FacilityId }));

            foreach (var entity in allEntities)
            {
                var censusEntity = censusEntities.FirstOrDefault(e => e.Id == entity.Id && e.FacilityId == entity.FacilityId);
                var censusMapRegion = censusEntities.FirstOrDefault(e => e.Id == entity.Id);

                var storeEntity = storeEntities.FirstOrDefault(e => e.Id == entity.Id && e.FacilityId == entity.FacilityId);
                var storeMapRegion = storeEntities.FirstOrDefault(e => e.Id == entity.Id);

                if (censusEntity != null)
                {
                    if (storeEntity == null)
                    {
                        // Brand New MapRegion
                        if (storeMapRegion == null)
                        {
                            censusEntity.IsDeprecated = false;
                            censusEntity.IsCurrent = true;
                    
                            createdEntities.Add(censusEntity);
                        }
                        // Existing MapRegion overwritten with new FacilityID
                        else if (censusEntity.FacilityId != 0)
                        {
                            censusEntity.IsDeprecated = false;
                            censusEntity.IsCurrent = true;
                        
                            createdEntities.Add(censusEntity);

                            storeEntity = storeMapRegion;
                            storeEntity.IsDeprecated = true;
                            storeEntity.IsCurrent = false;

                            dbContext.MapRegions.Update(storeEntity);
                        }
                        // Existing MapRegion is Deleted with no replacement, but still shows up in the Census API
                        else
                        {
                            storeEntity = storeMapRegion;
                            storeEntity.IsDeprecated = true;
                            storeEntity.IsCurrent = true;

                            dbContext.MapRegions.Update(storeEntity);
                        }
                    }
                    // Existing MapRegion updated somehow
                    else
                    {
                        storeEntity = censusEntity;

                        storeEntity.IsDeprecated = false;
                        storeEntity.IsCurrent = true;

                        dbContext.MapRegions.Update(storeEntity);
                    }
                }
                // Existing MapRegion is Deleted with no replacement, and doesn't show up in the Census API
                else
                {
                    if (storeEntity != null)
                    {
                        storeEntity.IsDeprecated = true;
                        storeEntity.IsCurrent = (censusMapRegion != null && censusMapRegion.Id == storeEntity.Id) ? false : true;

                        dbContext.MapRegions.Update(storeEntity);
                    }
                }
            }

            if (createdEntities.Any())
            {
                dbContext.MapRegions.AddRange(createdEntities);
            }

            await dbContext.SaveChangesAsync();
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

            return await dbContext.MapRegions.CountAsync(e => e.IsCurrent);
        }

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
