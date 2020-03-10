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
        private readonly ILogger<ApplicationDataLoader> _logger;


        public ApplicationDataLoader(
            IItemService itemService,
            IScrimRulesetManager rulesetManager,
            IScrimMatchScorer matchScorer,
            IFacilityService facilityService,
            ILogger<ApplicationDataLoader> logger)
        {
            _itemService = itemService;
            _rulesetManager = rulesetManager;
            _matchScorer = matchScorer;
            _facilityService = facilityService;
            _logger = logger;
        }

        public async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            List<Task> TaskList = new List<Task>();

            var itemsListTask = _itemService.SetUpItemsListAsync();
            TaskList.Add(itemsListTask);

            var weaponsListTask = _itemService.SetUpWeaponsListAsnyc();
            TaskList.Add(weaponsListTask);

            var activeRulesetTask = _rulesetManager.SetupActiveRuleset();
            TaskList.Add(activeRulesetTask);

            var scrimmableMapRegionsTask = _facilityService.SetUpScimmableMapRegionsAsync();
            TaskList.Add(scrimmableMapRegionsTask);

            //var rulesetTask = _matchScorer.SetActiveRuleset();
            //TaskList.Add(rulesetTask);

            await Task.WhenAll(TaskList);

            await _matchScorer.SetActiveRuleset();
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
