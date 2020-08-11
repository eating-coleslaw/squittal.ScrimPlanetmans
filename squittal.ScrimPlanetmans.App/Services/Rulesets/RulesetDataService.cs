using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Rulesets
{
    public class RulesetDataService : IRulesetDataService
    {
        private readonly ILogger<RulesetDataService> _logger;

        public RulesetDataService(ILogger<RulesetDataService> logger)
        {
            _logger = logger;
        }
        
        public Task<IEnumerable<int>> GetAllRulesetIdsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetAllRulesetNamesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Ruleset> GetDefaultRulesetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Ruleset> GetLatestRulesetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Ruleset> GetRulesetFromIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Ruleset> GetRulesetFromNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task RefreshStoreAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveActionRuleAsync(RulesetActionRule rule)
        {
            throw new NotImplementedException();
        }

        public Task SaveItemCategoryRuleAsync(RulesetItemCategoryRule rule)
        {
            throw new NotImplementedException();
        }

        public Task SaveRulesetAsync(Ruleset ruleset)
        {
            throw new NotImplementedException();
        }
    }
}
