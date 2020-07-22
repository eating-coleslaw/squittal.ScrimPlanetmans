using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.Services.Planetside;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Data
{
    public class ApplicationDataLoader : IApplicationDataLoader
    {
        //private readonly IItemService _itemService;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly IScrimRulesetManager _rulesetManager;
        private readonly IScrimMatchScorer _matchScorer;
        private readonly IFacilityService _facilityService;
        private readonly IWorldService _worldService;
        private readonly IZoneService _zoneService;
        private readonly ILogger<ApplicationDataLoader> _logger;


        public ApplicationDataLoader(
            //IItemService itemService,
            IItemCategoryService itemCategoryService,
            IScrimRulesetManager rulesetManager,
            IScrimMatchScorer matchScorer,
            IFacilityService facilityService,
            IWorldService worldService,
            IZoneService zoneService,
            ILogger<ApplicationDataLoader> logger)
        {
            //_itemService = itemService;
            _itemCategoryService = itemCategoryService;
            _rulesetManager = rulesetManager;
            _matchScorer = matchScorer;
            _facilityService = facilityService;
            _worldService = worldService;
            _zoneService = zoneService;
            _logger = logger;
        }

        public async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            List<Task> TaskList = new List<Task>();

            //var weaponCategoriesListTask = _itemService.SetUpWeaponCategoriesListAsync();
            var weaponCategoriesListTask = _itemCategoryService.SetUpWeaponCategoriesListAsync();
            TaskList.Add(weaponCategoriesListTask);

            var activeRulesetTask = _rulesetManager.SetupActiveRuleset();
            TaskList.Add(activeRulesetTask);

            var scrimmableMapRegionsTask = _facilityService.SetUpScrimmableMapRegionsAsync();
            TaskList.Add(scrimmableMapRegionsTask);

            var worldsTask = _worldService.SetupWorldsList();
            TaskList.Add(worldsTask);

            var zonesTask = _zoneService.SetupZonesList();
            TaskList.Add(zonesTask);

            await Task.WhenAll(TaskList);

            await _matchScorer.SetActiveRuleset();
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
