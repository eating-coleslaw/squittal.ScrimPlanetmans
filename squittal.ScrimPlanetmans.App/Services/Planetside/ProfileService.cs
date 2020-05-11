using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class ProfileService : IProfileService, IDisposable
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusProfile _censusProfile;
        private readonly CensusLoadout _censusLoadout;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<ProfileService> _logger;

        public string BackupSqlScriptFileName => "dbo.Profile.Table.sql";


        private Dictionary<int, Profile> _loadoutMapping = new Dictionary<int, Profile>();

        private readonly SemaphoreSlim _loadoutSemaphore = new SemaphoreSlim(1);

        public ProfileService(IDbContextHelper dbContextHelper, CensusProfile censusProfile, CensusLoadout censusLoadout, ISqlScriptRunner sqlScriptRunner, ILogger<ProfileService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusProfile = censusProfile;
            _censusLoadout = censusLoadout;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Profiles.ToListAsync();
            }

        }

        public async Task<IEnumerable<Loadout>> GetAllLoadoutsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Loadouts.ToListAsync();

        }

        public async Task<Profile> GetProfileFromLoadoutIdAsync(int loadoutId)
        {
            if (_loadoutMapping == null || _loadoutMapping.Count == 0)
            {
                await SetupLoadoutMappingAsync();
            }

            return _loadoutMapping.GetValueOrDefault(loadoutId, null);
        }

        public async Task<Dictionary<int, Profile>> GetLoadoutMapping()
        {
            if (_loadoutMapping == null || _loadoutMapping.Count == 0)
            {
                await SetupLoadoutMappingAsync();
            }

            return _loadoutMapping;
        }

        private async Task SetupLoadoutMappingAsync()
        {
            await _loadoutSemaphore.WaitAsync();

            try
            {
                if (_loadoutMapping == null || _loadoutMapping.Count == 0)
                {

                    var loadoutsTask = GetAllLoadoutsAsync();
                    var profilesTask = GetAllProfilesAsync();

                    await Task.WhenAll(loadoutsTask, profilesTask);

                    var loadouts = loadoutsTask.Result;
                    var profiles = profilesTask.Result;

                    _loadoutMapping = loadouts.ToDictionary(l => l.Id, l => profiles.FirstOrDefault(p => p.Id == l.ProfileId));
                }
            }
            finally
            {
                _loadoutSemaphore.Release();
            }
        }

        public static bool IsMaxLoadoutId(int? loadoutId)
        {
            return loadoutId switch
            {
                7 => true,
                14 => true,
                21 => true,
                45 => true,
                null => false,
                _ => false,
            };
        }

        public static bool IsMaxProfileId(int profileId)
        {
            return profileId switch
            {
                8 => true,
                16 => true,
                23 => true,
                252 => true,
                _ => false,
            };
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var anyProfiles = await dbContext.Profiles.AnyAsync();
                if (anyProfiles)
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
            IEnumerable<CensusProfileModel> censusProfiles = new List<CensusProfileModel>();

            try
            {
                censusProfiles = await _censusProfile.GetAllProfilesAsync();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all Profiles. Refreshing store from backup...");
                return false;
            }

            if (censusProfiles != null && censusProfiles.Any())
            {
                await UpsertRangeAsync(censusProfiles.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Profiles store");

                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task UpsertRangeAsync(IEnumerable<Profile> censusEntities)
        {
            var createdEntities = new List<Profile>();

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var storedEntities = await dbContext.Profiles.ToListAsync();

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
                        dbContext.Profiles.Update(storeEntity);
                    }
                }

                if (createdEntities.Any())
                {
                    await dbContext.Profiles.AddRangeAsync(createdEntities);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private Profile ConvertToDbModel(CensusProfileModel censusModel)
        {
            return new Profile
            {
                Id = censusModel.ProfileId,
                ProfileTypeId = censusModel.ProfileTypeId,
                FactionId = censusModel.FactionId,
                Name = censusModel.Name.English,
                ImageId = censusModel.ImageId
            };
        }

        public void Dispose()
        {
            _loadoutSemaphore.Dispose();
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusProfile.GetProfilesCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Profiles.CountAsync();
        }

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }
    }
}
