using System.Collections.Generic;
using System.Threading.Tasks;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Services.Planetside;

namespace squittal.ScrimPlanetmans.Services
{
    public class DatabaseMaintenanceService
    {
        private readonly IFacilityTypeService _facilityTypeService;
        private readonly IFacilityService _facilityService;
        private readonly IItemService _itemService;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly IProfileService _profileService;
        private readonly ILoadoutService _loadoutService;
        private readonly IZoneService _zoneService;
        private readonly IWorldService _worldService;
        private readonly IFactionService _factionService;
        private readonly IVehicleService _vehicleService;

        private readonly CensusStoreDataComparisonRow _mapRegions;
        private readonly CensusStoreDataComparisonRow _facilityTypes;
        private readonly CensusStoreDataComparisonRow _items;
        private readonly CensusStoreDataComparisonRow _itemCategories;
        private readonly CensusStoreDataComparisonRow _profiles;
        private readonly CensusStoreDataComparisonRow _loadouts;
        private readonly CensusStoreDataComparisonRow _zones;
        private readonly CensusStoreDataComparisonRow _worlds;
        private readonly CensusStoreDataComparisonRow _factions;
        private readonly CensusStoreDataComparisonRow _vehicles;

        public List<CensusStoreDataComparisonRow> Comparisons { get; private set; } = new List<CensusStoreDataComparisonRow>();

        private bool _isInitialLoadComplete = false;

        public DatabaseMaintenanceService(
            IFacilityTypeService facilityTypeService,
            IFacilityService facilityService,
            IItemService itemService,
            IItemCategoryService itemCategoryService,
            IProfileService profileService,
            ILoadoutService loadoutService,
            IZoneService zoneService,
            IWorldService worldService,
            IFactionService factionService,
            IVehicleService vehicleService
            )
        {
            _facilityService = facilityService;
            _facilityTypeService = facilityTypeService;
            _itemService = itemService;
            _itemCategoryService = itemCategoryService;
            _profileService = profileService;
            _loadoutService = loadoutService;
            _zoneService = zoneService;
            _worldService = worldService;
            _factionService = factionService;
            _vehicleService = vehicleService;

            _mapRegions = new CensusStoreDataComparisonRow("Map Regions", _facilityService);
            _facilityTypes = new CensusStoreDataComparisonRow("Facility Types", _facilityTypeService);
            _items = new CensusStoreDataComparisonRow("Items", _itemService);
            _itemCategories = new CensusStoreDataComparisonRow("Item Categories", _itemCategoryService);
            _profiles = new CensusStoreDataComparisonRow("Profiles", _profileService);
            _loadouts = new CensusStoreDataComparisonRow("Loadouts", _loadoutService);
            _zones = new CensusStoreDataComparisonRow("Zones", _zoneService);
            _worlds = new CensusStoreDataComparisonRow("Worlds", _worldService);
            _factions = new CensusStoreDataComparisonRow("Factions", _factionService);
            _vehicles = new CensusStoreDataComparisonRow("Vehicles", _vehicleService);

            Comparisons.Add(_mapRegions);
            Comparisons.Add(_facilityTypes);
            Comparisons.Add(_items);
            Comparisons.Add(_itemCategories);
            Comparisons.Add(_profiles);
            Comparisons.Add(_loadouts);
            Comparisons.Add(_zones);
            Comparisons.Add(_worlds);
            Comparisons.Add(_factions);
            Comparisons.Add(_vehicles);
        }

        public async Task InitializeCounts()
        {
            if (_isInitialLoadComplete)
            {
                return;
            }
            else
            {
                await SetAllCounts();
                _isInitialLoadComplete = true;
            }
        }

        public async Task SetAllCounts()
        {
            var TaskList = new List<Task>();

            foreach (var comparisonRow in Comparisons)
            {
                TaskList.Add(comparisonRow.SetCounts());
            }

            await Task.WhenAll(TaskList);
        }

        public async Task RefreshAllFromCensus()
        {
            var TaskList = new List<Task>();

            foreach (var comparisonRow in Comparisons)
            {
                TaskList.Add(comparisonRow.RefreshStoreFromCensus());
            }

            await Task.WhenAll(TaskList);
        }

        public async Task RefreshAllFromBackup()
        {
            var TaskList = new List<Task>();

            foreach (var comparisonRow in Comparisons)
            {
                TaskList.Add(comparisonRow.RefreshStoreFromBackup());
            }

            await Task.WhenAll(TaskList);
        }
    }
}
