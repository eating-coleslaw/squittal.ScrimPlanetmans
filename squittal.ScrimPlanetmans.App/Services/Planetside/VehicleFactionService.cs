using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class VehicleFactionService : IVehicleFactionService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusVehicle _censusVehicle;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<VehicleService> _logger;

        public string BackupSqlScriptFileName => string.Empty; //throw new NotImplementedException();

        /*
        public VehicleFactionService(IDbContextHelper dbContextHelper, CensusVehicle censusVehicle, ISqlScriptRunner sqlScriptRunner, ILogger<VehicleService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusVehicle = censusVehicle;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<IEnumerable<VehicleFaction>> GetVehicleFactionsAsync(int vehicleId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.VehicleFactions.Where(vf => vf.VehicleId == vehicleId).ToListAsync();
        }

        public async Task<IEnumerable<VehicleFaction>> GetAllVehicleFactionsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.VehicleFactions.ToListAsync();
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusVehicle.GetVehicleFactionsCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.VehicleFactions.CountAsync();
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var anyVehicleFactions = await dbContext.VehicleFactions.AnyAsync();
                if (anyVehicleFactions)
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
            IEnumerable<CensusVehicleFactionModel> vehicleFactions = new List<CensusVehicleFactionModel>();

            try
            {
                vehicleFactions = await _censusVehicle.GetAllVehicleFactions();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all Vehicle Factions. Refreshing store from backup...");
                return false;
            }

            if (vehicleFactions != null && vehicleFactions.Any())
            {
                await UpsertRangeAsync(vehicleFactions.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Vehicle Factions store: {vehicleFactions.Count()} entries");

                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task UpsertRangeAsync(IEnumerable<VehicleFaction> censusEntities)
        {
            var createdEntities = new List<VehicleFaction>();

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var storedEntities = await dbContext.VehicleFactions.ToListAsync();

            foreach (var censusEntity in censusEntities)
            {
                var storeEntity = storedEntities.FirstOrDefault(e => e.VehicleId == censusEntity.VehicleId && e.FactionId == censusEntity.FactionId);
                if (storeEntity == null)
                {
                    createdEntities.Add(censusEntity);
                }
                else
                {
                    storeEntity = censusEntity;
                    dbContext.VehicleFactions.Update(storeEntity);
                }
            }

            if (createdEntities.Any())
            {
                await dbContext.VehicleFactions.AddRangeAsync(createdEntities);
            }

            await dbContext.SaveChangesAsync();
        }

        public void RefreshStoreFromBackup()
        {
            throw new NotImplementedException();
        }

        private static VehicleFaction ConvertToDbModel(CensusVehicleFactionModel censusModel)
        {
            return new VehicleFaction
            {
                VehicleId = censusModel.VehicleId,
                FactionId = censusModel.FactionId
            };
        }
        */
    }
}
