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
    public class FacilityService : IFacilityService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusFacility _censusFacility;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<FacilityService> _logger;

        private List<MapRegion> _scrimmableMapRegions = new List<MapRegion>();

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

        public IEnumerable<MapRegion> GetScrimmableMapRegions()
        {
            return _scrimmableMapRegions;
        }

        public MapRegion GetScrimmableMapRegionFromFacilityId(int facilityId)
        {
            return _scrimmableMapRegions.FirstOrDefault(r => r.FacilityId == facilityId);
        }

        public async Task SetUpScrimmableMapRegionsAsync()
        {
            var realZones = new List<int> { 2, 4, 6, 8 };
            var scrimFacilityTypes = new List<int> { 5, 6}; // Small Outpost, Large Outpost

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            _scrimmableMapRegions = await dbContext.MapRegions
                                                    .Where(region => realZones.Contains(region.ZoneId)
                                                                        && scrimFacilityTypes.Contains(region.FacilityTypeId))
                                                    .ToListAsync();
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
                    return;
                }
            }

            var success = await RefreshStoreFromCensus();

            if (!success && canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }
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
