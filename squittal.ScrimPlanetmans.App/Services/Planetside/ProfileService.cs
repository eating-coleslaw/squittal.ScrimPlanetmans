using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
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
        private readonly ILogger<ProfileService> _logger;

        private Dictionary<int, Profile> _loadoutMapping = new Dictionary<int, Profile>();

        private readonly SemaphoreSlim _loadoutSemaphore = new SemaphoreSlim(1);

        public ProfileService(IDbContextHelper dbContextHelper, CensusProfile censusProfile, CensusLoadout censusLoadout, ILogger<ProfileService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusProfile = censusProfile;
            _censusLoadout = censusLoadout;
            _logger = logger;
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            var censusProfiles = await _censusProfile.GetAllProfilesAsync();

            if (censusProfiles == null)
            {
                return null;
            }
            
            var censusEntities = censusProfiles.Select(ConvertToDbModel);
            return censusEntities.ToList();

            /*
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Profiles.ToListAsync();
            }
            */
        }

        public async Task<IEnumerable<Loadout>> GetAllLoadoutsAsync()
        {
            var censusLoadouts = await _censusLoadout.GetAllLoadoutsAsync();

            if (censusLoadouts == null)
            {
                return null;
            }

            var allLoadouts = new List<CensusLoadoutModel>();

            allLoadouts.AddRange(censusLoadouts.ToList());
            allLoadouts.AddRange(GetFakeNsCensusLoadoutModels());

            var censusEntities = allLoadouts.Select(ConvertToDbModel);

            return censusEntities.ToList();

            /*
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Loadouts.ToListAsync();
            }
            */
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
                //if (_loadoutMapping != null || _loadoutMapping.Count > 0)
                //if (_loadoutMapping != null && _loadoutMapping.Count > 0)
                //{
                //    return;
                //}
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

        public static bool IsMaxLoadoutId(int loadoutId)
        {
            return loadoutId switch
            {
                7 => true,
                14 => true,
                21 => true,
                45 => true,
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

        public async Task RefreshStore()
        {
            var censusProfiles = await _censusProfile.GetAllProfilesAsync();

            if (censusProfiles != null)
            {
                await UpsertRangeAsync(censusProfiles.Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Profiles store");
            }

            var censusLoadouts = await _censusLoadout.GetAllLoadoutsAsync();

            if (censusLoadouts != null)
            {
                var allLoadouts = new List<CensusLoadoutModel>();

                allLoadouts.AddRange(censusLoadouts.ToList());
                allLoadouts.AddRange(GetFakeNsCensusLoadoutModels());

                await UpsertRangeAsync(allLoadouts.AsEnumerable().Select(ConvertToDbModel));

                _logger.LogInformation($"Refreshed Loadouts store");
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

        private async Task UpsertRangeAsync(IEnumerable<Loadout> censusEntities)
        {
            var createdEntities = new List<Loadout>();

            using (var factory = _dbContextHelper.GetFactory())
            {
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

        public void Dispose()
        {
            _loadoutSemaphore.Dispose();
        }
    }
}
