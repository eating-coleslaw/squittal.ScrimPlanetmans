using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
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
        private readonly IItemService _itemService;
        public ILogger<ScrimRulesetManager> _logger;

        private Ruleset _workingRuleset;
        private Ruleset ActiveRuleset { get; set; }

        private readonly int _defaultRulesetId;


        public ScrimRulesetManager(IDbContextHelper dbContextHelper, IItemService itemService, ILogger<ScrimRulesetManager> logger)
        {
            _dbContextHelper = dbContextHelper;
            _itemService = itemService;
            _logger = logger;

            _defaultRulesetId = 1;
        }

        public void InitializeNewRuleset()
        {
            _workingRuleset = new Ruleset
            {
                Name = "Untitled_Ruleset"
            };

        }

        public async Task<Ruleset> GetActiveRuleset()
        {
            if (ActiveRuleset == null)
            {
                return await GetDefaultRuleset();
            }
            else if (!ActiveRuleset.ActionRules.Any() || !ActiveRuleset.ItemCategoryRules.Any())
            {
                await SetupActiveRuleset();
                return ActiveRuleset;
            }
            else
            {
                return ActiveRuleset;
            }
        }


        public async Task<Ruleset> ActivateRuleset(int rulesetId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var currentActiveRuleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.IsActive == true);

            var newActiveRuleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.Id == rulesetId);

            if (newActiveRuleset == null)
            {
                return null;
            }

            if (currentActiveRuleset != null && currentActiveRuleset.Id != rulesetId)
            {
                currentActiveRuleset.IsActive = false;
                dbContext.Rulesets.Update(currentActiveRuleset);
            }
            else
            {
                newActiveRuleset.IsActive = true;
                dbContext.Rulesets.Update(newActiveRuleset);
            }

            await dbContext.SaveChangesAsync();

            ActiveRuleset = newActiveRuleset;
            ActiveRuleset.ActionRules = await dbContext.RulesetActionRules.Where(r => r.RulesetId == rulesetId).ToListAsync();
            ActiveRuleset.ItemCategoryRules = await dbContext.RulesetItemCategoryRules.Where(r => r.RulesetId == rulesetId).ToListAsync();

            return ActiveRuleset;
        }


        public async Task SetupActiveRuleset()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var ruleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.IsActive == true);

            if (ruleset == null)
            {
                _logger.LogError($"Failed to setup active ruleset: no ruleset found");
                return;
            }

            ActiveRuleset = ruleset;
            ActiveRuleset.ActionRules = await dbContext.RulesetActionRules.Where(r => r.RulesetId == ruleset.Id).ToListAsync();
            ActiveRuleset.ItemCategoryRules = await dbContext.RulesetItemCategoryRules.Where(r => r.RulesetId == ruleset.Id).ToListAsync();

            foreach (var rule in ActiveRuleset.ItemCategoryRules)
            {
                rule.ItemCategory = _itemService.GetWeaponItemCategory(rule.ItemCategoryId);
            }

            _logger.LogInformation($"Active ruleset loaded: {ActiveRuleset.Name}");
        }

        public async Task<Ruleset> GetDefaultRuleset()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var ruleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.Id == _defaultRulesetId);

                if (ruleset == null)
                {
                    return null;
                }

                ruleset.ActionRules = await dbContext.RulesetActionRules.Where(r => r.RulesetId == _defaultRulesetId).ToListAsync();
                ruleset.ItemCategoryRules = await dbContext.RulesetItemCategoryRules.Where(r => r.RulesetId == _defaultRulesetId).ToListAsync();

                return ruleset;
            }
        }

        public async Task<Ruleset> GetRulesetFromId(int rulesetId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var ruleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.Id == rulesetId);

                if (ruleset == null)
                {
                    return null;
                }

                var actionRules = await dbContext.RulesetActionRules.Where(r => r.RulesetId == rulesetId).ToListAsync();
                var itemCategoryRules = await dbContext.RulesetItemCategoryRules.Where(r => r.RulesetId == rulesetId).ToListAsync();

                ruleset.ActionRules = actionRules;
                ruleset.ItemCategoryRules = itemCategoryRules;

                return ruleset;
            }
        }

        public async Task SeedDefaultRuleset()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var defaultRulesetId = _defaultRulesetId;

                var storeRuleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.Id == defaultRulesetId);

                bool rulesetExistsInDb = false;

                var storeActionRules = new List<RulesetActionRule>();
                var storeItemCategoryRules = new List<RulesetItemCategoryRule>();

                if (storeRuleset != null)
                {
                    storeActionRules = await dbContext.RulesetActionRules.Where(r => r.RulesetId == storeRuleset.Id).ToListAsync();
                    
                    storeItemCategoryRules = await dbContext.RulesetItemCategoryRules.Where(r => r.RulesetId == storeRuleset.Id).ToListAsync();

                    rulesetExistsInDb = true;
                }
                else
                {
                    var utcNow = DateTime.UtcNow;
                    var newRuleset = new Ruleset
                    {
                        Name = "Default",
                        DateCreated = utcNow
                    };

                    storeRuleset = newRuleset;
                }


                storeRuleset.IsDefault = true;

                var activeRuleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.IsActive == true);
                if (activeRuleset == null || activeRuleset.Id == defaultRulesetId)
                {
                    storeRuleset.IsActive = true;
                }



                // Action Rules
                var defaultActionRules = GetDefaultActionRules();
                var createdActionRules = new List<RulesetActionRule>();
                var allActionRules = new List<RulesetActionRule>();

                foreach (var actionType in GetScrimActionTypes())
                {
                    var storeEntity = storeActionRules?.FirstOrDefault(r => r.ScrimActionType == actionType);
                    var defaultEntity = defaultActionRules.FirstOrDefault(r => r.ScrimActionType == actionType);

                    if (storeEntity == null)
                    {
                        if (defaultEntity != null) {
                            defaultEntity.RulesetId = defaultRulesetId;
                            createdActionRules.Add(defaultEntity);
                            allActionRules.Add(defaultEntity);
                        }
                        else
                        {
                            var newEntity = BuildRulesetActionRule(defaultRulesetId, actionType, 0);
                            createdActionRules.Add(newEntity);
                            allActionRules.Add(newEntity);
                        }
                    }
                    else
                    {
                        storeEntity.Points = defaultEntity != null ? defaultEntity.Points : 0;
                        dbContext.RulesetActionRules.Update(storeEntity);
                        allActionRules.Add(storeEntity);
                    }
                }

                if (createdActionRules.Any())
                {
                    await dbContext.RulesetActionRules.AddRangeAsync(createdActionRules);
                }

                // Item Category Rules
                var defaultItemCategoryRules = GetDefaultItemCategoryRules();
                var createdItemCategoryRules = new List<RulesetItemCategoryRule>();
                var allItemCategoryIds = await _itemService.GetItemCategoryIdsAsync();
                var allWeaponItemCategoryIds = await _itemService.GetWeaponItemCategoryIdsAsync();

                var allItemCategoryRules = new List<RulesetItemCategoryRule>();

                foreach (var categoryId in allItemCategoryIds)
                {
                    var isWeaponItemCategoryId = (allWeaponItemCategoryIds.Contains(categoryId)); 
                    
                    var storeEntity = storeItemCategoryRules?.FirstOrDefault(r => r.ItemCategoryId == categoryId);
                    var defaultEntity = defaultItemCategoryRules.FirstOrDefault(r => r.ItemCategoryId == categoryId);

                    if (storeEntity == null)
                    {
                        if (defaultEntity != null) {
                            defaultEntity.RulesetId = defaultRulesetId;
                            
                            createdItemCategoryRules.Add(defaultEntity);
                            allItemCategoryRules.Add(defaultEntity);
                        }
                        else if (isWeaponItemCategoryId)
                        {
                            var newEntity = BuildRulesetItemCategoryRule(defaultRulesetId, categoryId, 0);
                            createdItemCategoryRules.Add(newEntity);
                            allItemCategoryRules.Add(newEntity);

                        }
                    }
                    else
                    {
                        if (isWeaponItemCategoryId)
                        {
                            storeEntity.Points = defaultEntity != null ? defaultEntity.Points : 0;

                            dbContext.RulesetItemCategoryRules.Update(storeEntity);
                            allItemCategoryRules.Add(storeEntity);
                        }
                        else
                        {
                            dbContext.RulesetItemCategoryRules.Remove(storeEntity);
                        }
                    }
                }

                if (createdItemCategoryRules.Any())
                {
                    await dbContext.RulesetItemCategoryRules.AddRangeAsync(createdItemCategoryRules);
                }

                storeRuleset.ActionRules = allActionRules; // await dbContext.RulesetActionRules.Where(r => r.RulesetId == storeRuleset.Id).ToListAsync();
                storeRuleset.ItemCategoryRules = allItemCategoryRules; // await dbContext.RulesetItemCategoryRules.Where(r => r.RulesetId == storeRuleset.Id).ToListAsync();

                if (rulesetExistsInDb)
                {
                    dbContext.Rulesets.Update(storeRuleset);
                }
                else
                {
                    dbContext.Rulesets.Add(storeRuleset);
                }

                await dbContext.SaveChangesAsync();

                ActiveRuleset = storeRuleset;
                ActiveRuleset.ActionRules = allActionRules.ToList();
                ActiveRuleset.ItemCategoryRules = allItemCategoryRules.ToList();
            }
        }

        private IEnumerable<RulesetItemCategoryRule> GetDefaultItemCategoryRules()
        {
            var categories = GetDefaultScoredItemCategories();

            return categories.Select(c => BuildRulesetItemCategoryRule(c, 2)).ToArray();
        }

        private IEnumerable<int> GetDefaultScoredItemCategories()
        {
            return new int[]
            {
                2,   // Knife
                3,   // Pistol
                5,   // SMG
                6,   // LMG
                7,   // Assault Rifle
                8,   // Carbine
                11,  // Sniper Rifle
                12,  // Scout Rifle
                19,  // Battle Rifle
                24,  // Crossbow
                100, // Infantry
                102, // Infantry Weapons
                157  // Hybrid Rifle
            };
        }

        private IEnumerable<RulesetActionRule> GetDefaultActionRules()
        {
            // MaxKillInfantry & MaxKillMax are worth 0 points
            return new RulesetActionRule[]
            {
                BuildRulesetActionRule(ScrimActionType.FirstBaseCapture, 18),
                BuildRulesetActionRule(ScrimActionType.SubsequentBaseCapture, 36),
                BuildRulesetActionRule(ScrimActionType.InfantryKillMax, 12),
                BuildRulesetActionRule(ScrimActionType.InfantryTeamkillInfantry, -3),
                BuildRulesetActionRule(ScrimActionType.InfantryTeamkillMax, -15),
                BuildRulesetActionRule(ScrimActionType.InfantrySuicide, -3),
                BuildRulesetActionRule(ScrimActionType.MaxTeamkillMax, -15),
                BuildRulesetActionRule(ScrimActionType.MaxTeamkillInfantry, -3),
                BuildRulesetActionRule(ScrimActionType.MaxSuicide, -12)
            };
        }

        private RulesetActionRule BuildRulesetActionRule(int rulesetId, ScrimActionType actionType, int points = 0)
        {
            return new RulesetActionRule
            {
                RulesetId = rulesetId,
                ScrimActionType = actionType,
                Points = points
            };
        }

        private RulesetActionRule BuildRulesetActionRule(ScrimActionType actionType, int points = 0)
        {
            return new RulesetActionRule
            {
                ScrimActionType = actionType,
                Points = points
            };
        }

        private RulesetItemCategoryRule BuildRulesetItemCategoryRule(int rulesetId, int itemCategoryId, int points = 0)
        {
            return new RulesetItemCategoryRule
            {
                RulesetId = rulesetId,
                ItemCategoryId = itemCategoryId,
                Points = points
            };
        }

        private RulesetItemCategoryRule BuildRulesetItemCategoryRule(int itemCategoryId, int points = 0)
        {
            return new RulesetItemCategoryRule
            {
                ItemCategoryId = itemCategoryId,
                Points = points
            };
        }

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

        public IEnumerable<ScrimActionType> GetScrimActionTypes()
        {
            return (ScrimActionType[])Enum.GetValues(typeof(ScrimActionType));
        }
    }
}
