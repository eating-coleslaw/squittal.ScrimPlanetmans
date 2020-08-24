using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.Services.Planetside;
//using squittal.ScrimPlanetmans.Services.ScrimMatch;
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

        //private readonly IFactionService _factionService;
        //private readonly IItemService _itemService;
        //private readonly IProfileService _profileService;
        //private readonly ILoadoutService _loadoutService;
        //private readonly IFacilityTypeService _facilityTypeService;
        //private readonly IVehicleService _vehicleService;
        //private readonly IVehicleTypeService _vehicleTypeService;
        //private readonly IDeathEventTypeService _deathTypeService;

        private readonly IDbSeeder _dbSeeder;

        private readonly ILogger<ApplicationDataLoader> _logger;


        public ApplicationDataLoader(
            IItemCategoryService itemCategoryService,
            IScrimRulesetManager rulesetManager,
            IScrimMatchScorer matchScorer,
            IFacilityService facilityService,
            IWorldService worldService,
            IZoneService zoneService,

            //IFactionService factionService,
            //IItemService itemService,
            //IProfileService profileService,
            //ILoadoutService loadoutService,
            //IFacilityTypeService facilityTypeService,
            //IVehicleService vehicleService,
            //IVehicleTypeService vehicleTypeService,
            //IDeathEventTypeService deathTypeService,

            IDbSeeder dbSeeder,

            ILogger<ApplicationDataLoader> logger)
        {
            _itemCategoryService = itemCategoryService;
            _rulesetManager = rulesetManager;
            _matchScorer = matchScorer;
            _facilityService = facilityService;
            _worldService = worldService;
            _zoneService = zoneService;

            //_factionService = factionService;
            //_itemService = itemService;
            //_profileService = profileService;
            //_loadoutService = loadoutService;
            //_rulesetManager = rulesetManager;
            //_facilityTypeService = facilityTypeService;
            //_vehicleService = vehicleService;
            //_vehicleTypeService = vehicleTypeService;
            //_deathTypeService = deathTypeService;

            _dbSeeder = dbSeeder;

            _logger = logger;
        }

        public async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            try
            {

                await _dbSeeder.SeedDatabase(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                //List<Task> TaskList = new List<Task>();

                //Task worldsTask = _worldService.RefreshStore(true, true);
                //TaskList.Add(worldsTask);

                //Task factionsTask = _factionService.RefreshStore(true, true);
                //TaskList.Add(factionsTask);

                //Task itemsTask = _itemService.RefreshStore(true, true);
                //TaskList.Add(itemsTask);

                //Task itemCategoriesTask = _itemCategoryService.RefreshStore(true, true);
                //TaskList.Add(itemCategoriesTask);

                //Task zoneTask = _zoneService.RefreshStore(true, true);
                //TaskList.Add(zoneTask);

                //Task profileTask = _profileService.RefreshStore(true, true);
                //TaskList.Add(profileTask);

                //Task loadoutsTask = _loadoutService.RefreshStore(true, true);
                //TaskList.Add(loadoutsTask);

                //Task scrimActionTask = _rulesetManager.SeedScrimActionModels();
                //TaskList.Add(scrimActionTask);

                //Task facilitiesTask = _facilityService.RefreshStore(true, true);
                //TaskList.Add(facilitiesTask);

                //Task facilityTypesTask = _facilityTypeService.RefreshStore(true, true);
                //TaskList.Add(facilityTypesTask);

                //Task vehicleTask = _vehicleService.RefreshStore(true, false);
                //TaskList.Add(vehicleTask);

                //Task vehicleTypeTask = _vehicleTypeService.SeedVehicleClasses();
                //TaskList.Add(vehicleTypeTask);

                //Task deathTypeTask = _deathTypeService.SeedDeathTypes();
                //TaskList.Add(deathTypeTask);

                //await Task.WhenAll(TaskList);

                //await _rulesetManager.SeedDefaultRuleset();


                List<Task> TaskList = new List<Task>();
                //TaskList = new List<Task>();

                var seedDefaultRulesetTask = _rulesetManager.SeedDefaultRuleset();
                TaskList.Add(seedDefaultRulesetTask);

                var weaponCategoriesListTask = _itemCategoryService.SetUpWeaponCategoriesListAsync();
                TaskList.Add(weaponCategoriesListTask);

                //var activeRulesetTask = _rulesetManager.SetUpActiveRulesetAsync();
                //TaskList.Add(activeRulesetTask);

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
            //finally
            //{
            //    _dbSeeder.Dispose();
            //}
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
