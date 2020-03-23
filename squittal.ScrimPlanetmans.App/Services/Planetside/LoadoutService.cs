using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.Data;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class LoadoutService : ILoadoutService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusLoadout _censusLoadout;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<LoadoutService> _logger;

        public string BackupSqlScriptFileName => "dbo.Loadout.Table.sql";


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

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
