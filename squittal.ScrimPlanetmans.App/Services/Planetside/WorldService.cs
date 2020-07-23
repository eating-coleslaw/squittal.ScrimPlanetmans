using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Models.Planetside;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using squittal.ScrimPlanetmans.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class WorldService : IWorldService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusWorld _censusWorld;
        private readonly ISqlScriptRunner _sqlScriptRunner;
        private readonly ILogger<ProfileService> _logger;

        private ConcurrentDictionary<int, World> WorldsMap { get; set; } = new ConcurrentDictionary<int, World>();

        public string BackupSqlScriptFileName => "dbo.World.Table.sql";


        public WorldService(IDbContextHelper dbContextHelper, CensusWorld censusWorld, ISqlScriptRunner sqlScriptRunner, ILogger<ProfileService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _censusWorld = censusWorld;
            _sqlScriptRunner = sqlScriptRunner;
            _logger = logger;
        }


        public async Task<IEnumerable<World>> GetAllWorldsAsync()
        {
            if (WorldsMap.Count == 0 || !WorldsMap.Any())
            {
                await SetUpWorldsMap();
            }

            return GetAllWorlds();
        }

        private IEnumerable<World> GetAllWorlds()
        {
            return WorldsMap.Values.ToList();
        }

        public async Task<World> GetWorldAsync(int worldId)
        {
            if (WorldsMap.Count == 0 || !WorldsMap.Any())
            {
                await SetUpWorldsMap();
            }

            return GetWorld(worldId);
        }

        private World GetWorld(int worldId)
        {
            WorldsMap.TryGetValue(worldId, out var world);

            return world;
        }

        public async Task SetUpWorldsMap()
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeWorlds = await dbContext.Worlds.Where(z => z.Id != 25).ToListAsync(); // RIP Briggs

                foreach (var worldId in WorldsMap.Keys)
                {
                    if (!storeWorlds.Any(r => r.Id == worldId))
                    {
                        WorldsMap.TryRemove(worldId, out var removedWorld);
                    }
                }

                foreach (var world in storeWorlds)
                {
                    if (WorldsMap.ContainsKey(world.Id))
                    {
                        WorldsMap[world.Id] = world;
                    }
                    else
                    {
                        WorldsMap.TryAdd(world.Id, world);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting up Worlds Map: {ex}");
            }
        }

        public async Task RefreshStore(bool onlyQueryCensusIfEmpty = false, bool canUseBackupScript = false)
        {
            if (onlyQueryCensusIfEmpty)
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var anyWorlds = await dbContext.Worlds.AnyAsync();
                if (anyWorlds)
                {
                    await SetUpWorldsMap();

                    return;
                }
            }

            var success = await RefreshStoreFromCensus();

            if (!success && canUseBackupScript)
            {
                RefreshStoreFromBackup();
            }

            await SetUpWorldsMap();
        }

        public async Task<bool> RefreshStoreFromCensus()
        {
            var result = new List<World>();
            var createdEntities = new List<World>();

            IEnumerable<CensusWorldModel> worlds = new List<CensusWorldModel>();

            try
            {
                worlds = await _censusWorld.GetAllWorlds();
            }
            catch
            {
                _logger.LogError("Census API query failed: get all Worlds. Refreshing store from backup...");
                return false;
            }
            
            if (worlds != null && worlds.Any())
            {
                var censusEntities = worlds.Select(ConvertToDbModel);

                using (var factory = _dbContextHelper.GetFactory())
                {
                    var dbContext = factory.GetDbContext();

                    var storedEntities = await dbContext.Worlds.ToListAsync();

                    foreach (var censusEntity in censusEntities)
                    {
                        var storeEntity = storedEntities.FirstOrDefault(storedEntity => storedEntity.Id == censusEntity.Id);
                        if (storeEntity == null)
                        {
                            createdEntities.Add(censusEntity);
                        }
                        else
                        {
                            storeEntity = censusEntity;
                            dbContext.Worlds.Update(storeEntity);
                        }
                    }

                    if (createdEntities.Any())
                    {
                        await dbContext.Worlds.AddRangeAsync(createdEntities);
                    }

                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Refreshed Worlds store");
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static World ConvertToDbModel(CensusWorldModel censusModel)
        {
            return new World
            {
                Id = censusModel.WorldId,
                Name = censusModel.Name.English
            };
        }

        public async Task<int> GetCensusCountAsync()
        {
            return await _censusWorld.GetWorldsCount();
        }

        public async Task<int> GetStoreCountAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Worlds.CountAsync();
        }

        public void RefreshStoreFromBackup()
        {
            _sqlScriptRunner.RunSqlScript(BackupSqlScriptFileName);
        }

        public static bool IsJaegerWorldId(int worldId)
        {
            return worldId == 19;
        }
    }
}
