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
        private readonly IItemService _itemService;
        private readonly IScrimRulesetManager _rulesetManager;
        private readonly IScrimMatchScorer _matchScorer;
        private readonly IFacilityService _facilityService;
        private readonly IWorldService _worldService;
        private readonly IZoneService _zoneService;
        private readonly ILogger<ApplicationDataLoader> _logger;


        public ApplicationDataLoader(
            IItemService itemService,
            IScrimRulesetManager rulesetManager,
            IScrimMatchScorer matchScorer,
            IFacilityService facilityService,
            IWorldService worldService,
            IZoneService zoneService,
            ILogger<ApplicationDataLoader> logger)
        {
            _itemService = itemService;
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

            //var itemsListTask = _itemService.SetUpItemsListAsync();
            //TaskList.Add(itemsListTask);

            //var weaponsListTask = _itemService.SetUpWeaponsListAsnyc();
            //TaskList.Add(weaponsListTask);

            var weaponCategoriesListTask = _itemService.SetUpWeaponCategoriesListAsync();
            TaskList.Add(weaponCategoriesListTask);

            var activeRulesetTask = _rulesetManager.SetupActiveRuleset();
            TaskList.Add(activeRulesetTask);

            var scrimmableMapRegionsTask = _facilityService.SetUpScrimmableMapRegionsAsync();
            TaskList.Add(scrimmableMapRegionsTask);

            var worldsTask = _worldService.SetupWorldsList();
            TaskList.Add(worldsTask);

            var zonesTask = _zoneService.SetupZonesList();
            TaskList.Add(zonesTask);

            //var rulesetTask = _matchScorer.SetActiveRuleset();
            //TaskList.Add(rulesetTask);

            await Task.WhenAll(TaskList);

            //await _rulesetManager.SetupActiveRuleset();

            await _matchScorer.SetActiveRuleset();
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
