using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ScrimRulesetService : IScrimRulesetService
    {
        private readonly ILogger<ScrimRulesetService> _logger;

        public ScrimRulesetService(ILogger<ScrimRulesetService> logger)
        {
            _logger = logger;
        }
        
        public Task<IEnumerable<int>> GetAllRulesetIds()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetAllRulesetNames()
        {
            throw new NotImplementedException();
        }

        public Task<Ruleset> GetDefaultRuleset()
        {
            throw new NotImplementedException();
        }

        public Task<Ruleset> GetLatestRuleset()
        {
            throw new NotImplementedException();
        }

        public Task<Ruleset> GetRulesetFromId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Ruleset> GetRulesetFromName(string name)
        {
            throw new NotImplementedException();
        }

        public Task RefreshStore()
        {
            throw new NotImplementedException();
        }

        public Task SaveActionRule(RulesetActionRule rule)
        {
            throw new NotImplementedException();
        }

        public Task SaveItemCategoryRule(RulesetItemCategoryRule rule)
        {
            throw new NotImplementedException();
        }

        public Task SaveRuleset(Ruleset ruleset)
        {
            throw new NotImplementedException();
        }
    }
}
