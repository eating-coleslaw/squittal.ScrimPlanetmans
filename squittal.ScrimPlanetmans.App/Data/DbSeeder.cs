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
        private readonly IZoneService _zoneService;
        private readonly IProfileService _profileService;

        public DbSeeder(
            IWorldService worldService,
            IFactionService factionService,
            IItemService itemService,
            IZoneService zoneService,
            IProfileService profileService
        )
        {
            _worldService = worldService;
            _factionService = factionService;
            _itemService = itemService;
            _zoneService = zoneService;
            _profileService = profileService;
        }

        public async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            List<Task> TaskList = new List<Task>();

            Task worldsTask = _worldService.RefreshStore();
            TaskList.Add(worldsTask);

            Task factionsTask = _factionService.RefreshStore();
            TaskList.Add(factionsTask);

            // Won't refresh if already populated
            Task itemsTask = _itemService.RefreshStore();
            TaskList.Add(itemsTask);

            Task zoneTask = _zoneService.RefreshStore();
            TaskList.Add(zoneTask);

            Task profileTask = _profileService.RefreshStore();
            TaskList.Add(profileTask);

            await Task.WhenAll(TaskList);
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
