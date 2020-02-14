using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimRulesetManager
    {
        public ILogger<ScrimRulesetManager> _logger;

        private ScrimRuleset _workingRuleset;

        public ScrimRulesetManager(ILogger<ScrimRulesetManager> logger)
        {
            _logger = logger;
        }

        public void InitializeNewRuleset()
        {
            _workingRuleset = new ScrimRuleset
            {
                Name = "Untitled_Ruleset"
            };

        }
    }
}
