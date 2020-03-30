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
        private readonly IScrimRulesetManager _rulesetManager;
        private readonly IFacilityService _facilityService;

        public DbSeeder(
            IWorldService worldService,
            IFactionService factionService,
            IItemService itemService,
            IItemCategoryService itemCategoryService,
            IZoneService zoneService,
            IProfileService profileService,
            IScrimRulesetManager rulesetManager,
            IFacilityService facilityService
        )
        {
            _worldService = worldService;
            _factionService = factionService;
            _itemService = itemService;
            _itemCategoryService = itemCategoryService;
            _zoneService = zoneService;
            _profileService = profileService;
            _rulesetManager = rulesetManager;
            _facilityService = facilityService;
        }

        public async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            List<Task> TaskList = new List<Task>();

            Task worldsTask = _worldService.RefreshStore(true);
            TaskList.Add(worldsTask);

            Task factionsTask = _factionService.RefreshStore(true);
            TaskList.Add(factionsTask);

            Task itemsTask = _itemService.RefreshStore(true);
            TaskList.Add(itemsTask);
            
            Task itemCategoriesTask = _itemCategoryService.RefreshStore(true);
            TaskList.Add(itemCategoriesTask);

            Task zoneTask = _zoneService.RefreshStore(true);
            TaskList.Add(zoneTask);

            Task profileTask = _profileService.RefreshStore(true);
            TaskList.Add(profileTask);

            Task scrimActionTask = _rulesetManager.SeedScrimActionModels();
            TaskList.Add(scrimActionTask);

            //Task defaultRulesetTask = _rulesetManager.SeedDefaultRuleset();
            //TaskList.Add(defaultRulesetTask);

            Task facilitiesTask = _facilityService.RefreshStore(true);
            TaskList.Add(facilitiesTask);

            await Task.WhenAll(TaskList);

            await _rulesetManager.SeedDefaultRuleset();
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
