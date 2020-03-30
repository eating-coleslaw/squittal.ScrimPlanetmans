using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.Services.Planetside;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Data
{
    public class DbSeeder : IDbSeeder
    {
        private readonly IWorldService _worldService;
        private readonly IFactionService _factionService;
        private readonly IItemService _itemService;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly IZoneService _zoneService;
        private readonly IProfileService _profileService;
        private readonly ILoadoutService _loadoutService;
        private readonly IScrimRulesetManager _rulesetManager;
        private readonly IFacilityService _facilityService;
        private readonly IFacilityTypeService _facilityTypeService;

        public DbSeeder(
            IWorldService worldService,
            IFactionService factionService,
            IItemService itemService,
            IItemCategoryService itemCategoryService,
            IZoneService zoneService,
            IProfileService profileService,
            ILoadoutService loadoutService,
            IScrimRulesetManager rulesetManager,
            IFacilityService facilityService,
            IFacilityTypeService facilityTypeService
        )
        {
            _worldService = worldService;
            _factionService = factionService;
            _itemService = itemService;
            _itemCategoryService = itemCategoryService;
            _zoneService = zoneService;
            _profileService = profileService;
            _loadoutService = loadoutService;
            _rulesetManager = rulesetManager;
            _facilityService = facilityService;
            _facilityTypeService = facilityTypeService;
        }

        public async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            List<Task> TaskList = new List<Task>();

            Task worldsTask = _worldService.RefreshStore(true, true);
            TaskList.Add(worldsTask);

            Task factionsTask = _factionService.RefreshStore(true, true);
            TaskList.Add(factionsTask);

            Task itemsTask = _itemService.RefreshStore(true, true);
            TaskList.Add(itemsTask);
            
            Task itemCategoriesTask = _itemCategoryService.RefreshStore(true, true);
            TaskList.Add(itemCategoriesTask);

            Task zoneTask = _zoneService.RefreshStore(true, true);
            TaskList.Add(zoneTask);

            Task profileTask = _profileService.RefreshStore(true, true);
            TaskList.Add(profileTask);

            Task loadoutsTask = _loadoutService.RefreshStore(true, true);
            TaskList.Add(loadoutsTask);

            Task scrimActionTask = _rulesetManager.SeedScrimActionModels();
            TaskList.Add(scrimActionTask);

            //Task defaultRulesetTask = _rulesetManager.SeedDefaultRuleset();
            //TaskList.Add(defaultRulesetTask);

            Task facilitiesTask = _facilityService.RefreshStore(true, true);
            TaskList.Add(facilitiesTask);
            
            Task facilityTypesTask = _facilityTypeService.RefreshStore(true, true);
            TaskList.Add(facilityTypesTask);

            await Task.WhenAll(TaskList);

            await _rulesetManager.SeedDefaultRuleset();
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
