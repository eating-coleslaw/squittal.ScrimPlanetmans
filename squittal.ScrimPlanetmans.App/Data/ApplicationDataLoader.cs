using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.Services.Planetside;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Data
{
    public class ApplicationDataLoader : IApplicationDataLoader
    {
        private readonly IItemCategoryService _itemCategoryService;
        private readonly IScrimRulesetManager _rulesetManager;
        private readonly IScrimMatchScorer _matchScorer;
        private readonly IFacilityService _facilityService;
        private readonly IWorldService _worldService;
        private readonly IZoneService _zoneService;
        private readonly IDbSeeder _dbSeeder;
        private readonly ILogger<ApplicationDataLoader> _logger;


        public ApplicationDataLoader(
            IItemCategoryService itemCategoryService,
            IScrimRulesetManager rulesetManager,
            IScrimMatchScorer matchScorer,
            IFacilityService facilityService,
            IWorldService worldService,
            IZoneService zoneService,
            IDbSeeder dbSeeder,
            ILogger<ApplicationDataLoader> logger)
        {
            _itemCategoryService = itemCategoryService;
            _rulesetManager = rulesetManager;
            _matchScorer = matchScorer;
            _facilityService = facilityService;
            _worldService = worldService;
            _zoneService = zoneService;
            _dbSeeder = dbSeeder;
            _logger = logger;
        }

        public async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            try
            {

                await _dbSeeder.SeedDatabase(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                List<Task> TaskList = new List<Task>();

                var seedDefaultRulesetTask = _rulesetManager.SeedDefaultRuleset();
                TaskList.Add(seedDefaultRulesetTask);

                var weaponCategoriesListTask = _itemCategoryService.SetUpWeaponCategoriesListAsync();
                TaskList.Add(weaponCategoriesListTask);

                var scrimmableMapRegionsTask = _facilityService.SetUpScrimmableMapRegionsAsync();
                TaskList.Add(scrimmableMapRegionsTask);

                var worldsMapTask = _worldService.SetUpWorldsMap();
                TaskList.Add(worldsMapTask);

                var zonesTask = _zoneService.SetupZonesMapAsync();
                TaskList.Add(zonesTask);

                await Task.WhenAll(TaskList);

                cancellationToken.ThrowIfCancellationRequested();

                await _rulesetManager.ActivateDefaultRulesetAsync();

                await _rulesetManager.SetUpActiveRulesetAsync();

                await _matchScorer.SetActiveRulesetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed loading application data: {ex}");
            }
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
