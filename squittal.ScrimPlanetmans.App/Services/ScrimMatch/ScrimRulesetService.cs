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

        public Task<ScrimRuleset> GetDefaultRuleset()
        {
            throw new NotImplementedException();
        }

        public Task<ScrimRuleset> GetLatestRuleset()
        {
            throw new NotImplementedException();
        }

        public Task<ScrimRuleset> GetRulesetFromId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ScrimRuleset> GetRulesetFromName(string name)
        {
            throw new NotImplementedException();
        }

        public Task RefreshStore()
        {
            throw new NotImplementedException();
        }

        public Task SaveActionRule(ScrimActionRulePoints rule)
        {
            throw new NotImplementedException();
        }

        public Task SaveItemCategoryRule(ItemCategoryRule rule)
        {
            throw new NotImplementedException();
        }

        public Task SaveRuleset(ScrimRuleset ruleset)
        {
            throw new NotImplementedException();
        }
    }
}
