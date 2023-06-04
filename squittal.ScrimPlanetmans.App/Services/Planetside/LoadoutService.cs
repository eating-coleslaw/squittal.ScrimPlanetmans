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
    public class LoadoutService : ILoadoutService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusLoadout _censusLoadout;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<LoadoutService> _logger;

        public string BackupSqlScriptFileName => "CensusBackups\\dbo.Loadout.Table.sql";


        public LoadoutService(IDbContextHelper dbContextHelper, CensusLoadout censusLoadout, ISqlScriptRunner sqlScriptRunner, ILogger<LoadoutService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusLoadout = censusLoadout;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusLoadout.GetLoadoutsCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Loadouts.CountAsync();
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var anyLoadouts = await dbContext.Loadouts.AnyAsync();
                if (anyLoadouts)
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
            IEnumerable<CensusLoadoutModel> censusLoadouts = new List<CensusLoadoutModel>();

            try
            {
                censusLoadouts = await _censusLoadout.GetAllLoadoutsAsync();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all Loadouts. Refreshing store from backup...");
                return false;
            }

            if (censusLoadouts != null && censusLoadouts.Any())
            {
                var allLoadouts = new List<CensusLoadoutModel>();

                allLoadouts.AddRange(censusLoadouts.ToList());
                //allLoadouts.AddRange(GetFakeNsCensusLoadoutModels());

                await UpsertRangeAsync(allLoadouts.AsEnumerable().Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Loadouts store");

                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task UpsertRangeAsync(IEnumerable<Loadout> censusEntities)
        {
            var createdEntities = new List<Loadout>();

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var storedEntities = await dbContext.Loadouts.ToListAsync();

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
                    dbContext.Loadouts.Update(storeEntity);
                }
            }

            if (createdEntities.Any())
            {
                await dbContext.Loadouts.AddRangeAsync(createdEntities);
            }

            await dbContext.SaveChangesAsync();
        }

        private Loadout ConvertToDbModel(CensusLoadoutModel censusModel)
        {
            return new Loadout
            {
                Id = censusModel.LoadoutId,
                ProfileId = censusModel.ProfileId,
                FactionId = censusModel.FactionId,
                CodeName = censusModel.CodeName,
            };
        }

        private IEnumerable<CensusLoadoutModel> GetFakeNsCensusLoadoutModels()
        {
            var nsLoadouts = new List<CensusLoadoutModel>
            {
                GetNewCensusLoadoutModel(28, 190, 4, "NS Infiltrator"),
                GetNewCensusLoadoutModel(29, 191, 4, "NS Light Assault"),
                GetNewCensusLoadoutModel(30, 192, 4, "NS Combat Medic"),
                GetNewCensusLoadoutModel(31, 193, 4, "NS Engineer"),
                GetNewCensusLoadoutModel(32, 194, 4, "NS Heavy Assault"),
                GetNewCensusLoadoutModel(45, 252, 4, "NS Defector")
            };

            return nsLoadouts;
        }

        private CensusLoadoutModel GetNewCensusLoadoutModel(int loadoutId, int profileId, int factionId, string codeName)
        {
            return new CensusLoadoutModel()
            {
                LoadoutId = loadoutId,
                ProfileId = profileId,
                FactionId = factionId,
                CodeName = codeName
            };
        }

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
