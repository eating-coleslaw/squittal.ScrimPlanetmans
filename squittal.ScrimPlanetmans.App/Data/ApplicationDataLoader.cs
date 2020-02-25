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
        private readonly ILogger<ApplicationDataLoader> _logger;


        public ApplicationDataLoader(
            IItemService itemService,
            IScrimRulesetManager rulesetManager,
            IScrimMatchScorer matchScorer,
            ILogger<ApplicationDataLoader> logger)
        {
            _itemService = itemService;
            _rulesetManager = rulesetManager;
            _matchScorer = matchScorer;
            _logger = logger;
        }

        public async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            List<Task> TaskList = new List<Task>();

            var itemsListTask = _itemService.SetUpItemsListAsync();
            TaskList.Add(itemsListTask);

            var weaponsListTask = _itemService.SetUpWeaponsListAsnyc();
            TaskList.Add(weaponsListTask);

            var rulesetTask = _matchScorer.SetActiveRuleset();
            TaskList.Add(rulesetTask);

            await Task.WhenAll(TaskList);
        }

        public async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
