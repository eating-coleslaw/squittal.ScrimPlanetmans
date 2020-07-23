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
    public class ProfileService : IProfileService, IDisposable
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusProfile _censusProfile;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<ProfileService> _logger;
        
        private ConcurrentDictionary<int, Profile> LoadoutProfilesMap { get; set; } = new ConcurrentDictionary<int, Profile>();

        public string BackupSqlScriptFileName => "dbo.Profile.Table.sql";

        private readonly SemaphoreSlim _mapSetUpSemaphore = new SemaphoreSlim(1);


        public ProfileService(IDbContextHelper dbContextHelper, CensusProfile censusProfile, ISqlScriptRunner sqlScriptRunner, ILogger<ProfileService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusProfile = censusProfile;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            if (LoadoutProfilesMap.Count == 0 || !LoadoutProfilesMap.Any())
            {
                await SetUpLoadoutProfilesMapAsync();
            }

            return GetAllProfiles();
        }

        private IEnumerable<Profile> GetAllProfiles()
        {
            return LoadoutProfilesMap.Values.ToList();
        }

        public async Task<Profile> GetProfileFromLoadoutIdAsync(int loadoutId)
        {
            if (LoadoutProfilesMap.Count == 0 || !LoadoutProfilesMap.Any())
            {
                await SetUpLoadoutProfilesMapAsync();
            }

            return GetProfileFromLoadoutId(loadoutId);
        }

        private Profile GetProfileFromLoadoutId(int loadoutId)
        {
            LoadoutProfilesMap.TryGetValue(loadoutId, out var profile);

            return profile;
        }

        private async Task SetUpLoadoutProfilesMapAsync()
        {
            await _mapSetUpSemaphore.WaitAsync();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeProfiles = await dbContext.Profiles.ToListAsync();

                var storeLoadouts = await dbContext.Loadouts.ToListAsync();

                foreach (var loadoutId in LoadoutProfilesMap.Keys)
                {
                    if (!storeLoadouts.Any(l => l.Id == loadoutId))
                    {
                        LoadoutProfilesMap.TryRemove(loadoutId, out var removedProfile);
                        continue;
                    }
                    
                    var profileId = storeLoadouts.Where(l => l.Id == loadoutId).Select(l => l.ProfileId).FirstOrDefault();

                    if (profileId <= 0)
                    {
                        LoadoutProfilesMap.TryRemove(loadoutId, out var removedProfile);
                    }
                }

                foreach (var profile in storeProfiles)
                {
                    var loadoutId = storeLoadouts.Where(l => l.ProfileId == profile.Id).Select(l => l.ProfileId).FirstOrDefault();
                    if (loadoutId <= 0)
                    {
                        continue;
                    }

                    if (LoadoutProfilesMap.ContainsKey(loadoutId))
                    {
                        LoadoutProfilesMap[loadoutId] = profile;
                    }
                    else
                    {
                        LoadoutProfilesMap.TryAdd(loadoutId, profile);
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
                    await SetUpLoadoutProfilesMapAsync();

                    return;
                }
            }

            var success = await RefreshStoreFromCensus();

            if (!success && canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }

            await SetUpLoadoutProfilesMapAsync();
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
            _mapSetUpSemaphore.Dispose();
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
