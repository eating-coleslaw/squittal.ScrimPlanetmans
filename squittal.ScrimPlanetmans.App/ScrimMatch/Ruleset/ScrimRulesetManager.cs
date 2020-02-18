using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimRulesetManager : IScrimRulesetManager
    {
        private readonly IDbContextHelper _dbContextHelper;
        public ILogger<ScrimRulesetManager> _logger;

        private Ruleset _workingRuleset;

        public ScrimRulesetManager(IDbContextHelper dbContextHelper, ILogger<ScrimRulesetManager> logger)
        {
            _dbContextHelper = dbContextHelper; 
            _logger = logger;
        }

        public void InitializeNewRuleset()
        {
            _workingRuleset = new Ruleset
            {
                Name = "Untitled_Ruleset"
            };

        }

        public async Task SeedDefaultRuleset()
        {
            throw new NotImplementedException();
        }

        //public async Task SeedDefaultRuleset()
        //{
        //    using (var factory = _dbContextHelper.GetFactory())
        //    {
        //        var dbContext = factory.GetDbContext();

        //        var storeEntity = factory.ScrimRulesets.FirstOrDefaultAsync(r => r.Id == 0 && r.Name)
        //    }
        //}

        public async Task SeedScrimActionModels()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var createdEntities = new List<ScrimAction>();

                var enumValues = (ScrimActionType[])Enum.GetValues(typeof(ScrimActionType));

                var storeEntities = await dbContext.ScrimActions.ToListAsync();

                foreach (var value in enumValues)
                {
                    var storeEntity = storeEntities.FirstOrDefault(e => e.Action == value);
                    if (storeEntity == null)
                    {
                        createdEntities.Add(ConvertToDbModel(value));
                    }
                    else
                    {
                        storeEntity = ConvertToDbModel(value);
                        dbContext.ScrimActions.Update(storeEntity);
                    }
                }

                if (createdEntities.Any())
                {
                    await dbContext.ScrimActions.AddRangeAsync(createdEntities);
                }

                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"Seeded Scrim Actions store");
            }
        }

        private ScrimAction ConvertToDbModel(ScrimActionType value)
        {
            var name = Enum.GetName(typeof(ScrimActionType), value);

            return new ScrimAction
            {
                Action = value,
                Name = name,
                Description = Regex.Replace(name, @"(\p{Ll})(\p{Lu})", "$1 $2")
            };
        }
    }
}
