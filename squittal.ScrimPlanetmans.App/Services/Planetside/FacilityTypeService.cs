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
    public class FacilityTypeService : IFacilityTypeService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusFacility _censusFacility;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<FacilityTypeService> _logger;

        public string BackupSqlScriptFileName => "CensusBackups\\dbo.FacilityType.Table.sql";

        public FacilityTypeService(IDbContextHelper dbContextHelper, CensusFacility censusFacility, ISqlScriptRunner sqlScriptRunner, ILogger<FacilityTypeService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusFacility = censusFacility;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusFacility.GetFacilityTypesCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.FacilityTypes.CountAsync();
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var anyFacilityTypes = await dbContext.FacilityTypes.AnyAsync();
                if (anyFacilityTypes)
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
            IEnumerable<CensusFacilityTypeModel> facilityTypes = new List<CensusFacilityTypeModel>();

            try
            {
                facilityTypes = await _censusFacility.GetAllFacilityTypes();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all Facility Types. Refreshing store from backup...");
                return false;
            }

            if (facilityTypes != null && facilityTypes.Any())
            {
                await UpsertRangeAsync(facilityTypes.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Facility Types store: {facilityTypes.Count()} entries");
                
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task UpsertRangeAsync(IEnumerable<FacilityType> censusEntities)
        {
            var createdEntities = new List<FacilityType>();

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var storedEntities = await dbContext.FacilityTypes.ToListAsync();

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
                    dbContext.FacilityTypes.Update(storeEntity);
                }
            }

            if (createdEntities.Any())
            {
                await dbContext.FacilityTypes.AddRangeAsync(createdEntities);
            }

            await dbContext.SaveChangesAsync();
        }

        private FacilityType ConvertToDbModel(CensusFacilityTypeModel censusModel)
        {
            return new FacilityType
            {
                Id = censusModel.FacilityTypeId,
                Description = censusModel.Description
            };
        }

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
