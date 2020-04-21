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
    public class VehicleService : IVehicleService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusVehicle _censusVehicle;
        //public readonly IVehicleFactionService _vehicleFactionService;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<VehicleService> _logger;

        public string BackupSqlScriptFileName => string.Empty; //throw new NotImplementedException();

        public VehicleService(IDbContextHelper dbContextHelper, CensusVehicle censusVehicle, /*IVehicleFactionService vehicleFactionService,*/ ISqlScriptRunner sqlScriptRunner, ILogger<VehicleService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusVehicle = censusVehicle;
            //_vehicleFactionService = vehicleFactionService;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<Vehicle> GetVehicleInfoAsync(int vehicleId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null)
            {
                return null;
            }

            //var factions = await _vehicleFactionService.GetVehicleFactionsAsync(vehicleId);

            //vehicle.Faction = factions;

            return vehicle;

            //return new VehicleInfo
            //{
            //    Id = vehicle.Id,
            //    Name = vehicle.Name,
            //    Factions = factions?.Select(f => f.FactionId)
            //};
        }

        public Vehicle GetScrimVehicleInfo(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task SetUpScrimmableVehicleInfosList()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusVehicle.GetVehiclesCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Vehicles.CountAsync();
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var anyVehicles = await dbContext.Vehicles.AnyAsync();
                if (anyVehicles)
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
            IEnumerable<CensusVehicleModel> vehicles = new List<CensusVehicleModel>();

            try
            {
                vehicles = await _censusVehicle.GetAllVehicles();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all Vehicles. Refreshing store from backup...");
                return false;
            }

            if (vehicles != null && vehicles.Any())
            {
                var testList = vehicles.Select(ConvertToDbModel);
                
                await UpsertRangeAsync(vehicles.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Vehicles store: {vehicles.Count()} entries");

                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task UpsertRangeAsync(IEnumerable<Vehicle> censusEntities)
        {
            var createdEntities = new List<Vehicle>();

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var storedEntities = await dbContext.Vehicles.ToListAsync();

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
                    dbContext.Vehicles.Update(storeEntity);
                }
            }

            if (createdEntities.Any())
            {
                await dbContext.Vehicles.AddRangeAsync(createdEntities);
            }

            await dbContext.SaveChangesAsync();
        }

        public void RefreshStoreFromBackup()
        {
            throw new NotImplementedException();
        }

        private static Vehicle ConvertToDbModel(CensusVehicleModel censusModel)
        {
            return new Vehicle
            {
                Id = censusModel.VehicleId,
                Name = censusModel.Name?.English,
                Description = censusModel.Description?.English,
                TypeId = censusModel.TypeId,
                TypeName = censusModel.TypeName,
                Cost = censusModel.Cost,
                CostResourceId = censusModel.CostResourceId,
                ImageId = censusModel.ImageId
            };
        }
    }
}
