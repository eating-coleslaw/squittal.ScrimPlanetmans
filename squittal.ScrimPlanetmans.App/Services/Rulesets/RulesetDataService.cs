using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Rulesets
{
    public class RulesetDataService : IRulesetDataService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly ILogger<RulesetDataService> _logger;

        private readonly int _rulesetBrowserPageSize = 15;

        private readonly KeyedSemaphoreSlim _rulesetLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _actionRulesLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _itemCategoryRulesLock = new KeyedSemaphoreSlim();

        public RulesetDataService(IDbContextHelper dbContextHelper, ILogger<RulesetDataService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _logger = logger;
        }
        
        public async Task<PaginatedList<Ruleset>> GetRulesetListAsync(int? pageIndex, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var rulesetsQuery = dbContext.Rulesets
                                                    .AsQueryable()
                                                    .AsNoTracking()
                                                    .OrderByDescending(r => r.IsActive)
                                                    .ThenByDescending(r => r.IsDefault)
                                                    .ThenByDescending(r => r.IsCustomDefault)
                                                    .ThenBy(r => r.Name);

                var paginatedList = await PaginatedList<Ruleset>.CreateAsync(rulesetsQuery, pageIndex ?? 1, _rulesetBrowserPageSize, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                if (paginatedList == null)
                {
                    return null;
                }

                return paginatedList;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetRulesetsAsync page {pageIndex}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetRulesetsAsync page {pageIndex}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<Ruleset> GetRulesetFromIdAsync(int rulesetId, CancellationToken cancellationToken, bool includeCollections = true)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                Ruleset ruleset;

                if (includeCollections)
                {
                    ruleset = await dbContext.Rulesets
                                                .Where(r => r.Id == rulesetId)
                                                .Include("RulesetItemCategoryRules")
                                                .Include("RulesetActionRules")
                                                .Include("RulesetItemCategoryRules.ItemCategory")
                                                .FirstOrDefaultAsync(cancellationToken);
                }
                else
                {
                    ruleset = await dbContext.Rulesets
                                               .Where(r => r.Id == rulesetId)
                                               .FirstOrDefaultAsync(cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();

                return ruleset;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetRulesetFromIdAsync rulesetId {rulesetId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetRulesetFromIdAsync rulesetId {rulesetId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<RulesetActionRule>> GetRulesetActionRulesAsync(int rulesetId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var rules = await dbContext.RulesetActionRules
                                               .Where(r => r.RulesetId == rulesetId)
                                               .ToListAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return rules;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetRulesetActionRulesAsync rulesetId {rulesetId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetRulesetActionRulesAsync rulesetId {rulesetId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        /*
         * Upsert New or Modified RulesetActionRules for a specific ruleset.
         */
        public async Task SaveRulesetActionRules(int rulesetId, IEnumerable<RulesetActionRule> rules)
        {
            using (await _actionRulesLock.WaitAsync($"{rulesetId}"))
            {
                var ruleUpdates = rules.Where(rule => rule.RulesetId == rulesetId).ToList();

                if (!ruleUpdates.Any())
                {
                    return;
                }

                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeRules = await dbContext.RulesetActionRules.Where(rule => rule.RulesetId == rulesetId).ToListAsync();

                    var newEntities = new List<RulesetActionRule>();

                    foreach (var rule in ruleUpdates)
                    {
                        var storeEntity = storeRules.Where(r => r.ScrimActionType == rule.ScrimActionType).FirstOrDefault();

                        if (storeEntity == null)
                        {
                            newEntities.Add(rule);
                        }
                        else
                        {
                            storeEntity = rule;
                            dbContext.RulesetActionRules.Update(storeEntity);
                        }
                    }

                    if (newEntities.Any())
                    {
                        dbContext.RulesetActionRules.AddRange(newEntities);
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving RulesetActionRule changes to database: {ex}");
                }
            }
        }

        public async Task<IEnumerable<RulesetItemCategoryRule>> GetRulesetItemCategoryRulesAsync(int rulesetId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var rules = await dbContext.RulesetItemCategoryRules
                                               .Where(r => r.RulesetId == rulesetId)
                                               .Include("ItemCategory")
                                               .OrderBy(r => r.ItemCategory.Domain)
                                               .ThenBy(r => r.ItemCategory.Name)
                                               .ToListAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return rules;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetRulesetItemCategoryRulesAsync rulesetId {rulesetId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetRulesetItemCategoryRulesAsync rulesetId {rulesetId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        /*
         * Upsert New or Modified RulesetItemCategoryRules for a specific ruleset.
         */
        public async Task SaveRulesetItemCategoryRules(int rulesetId, IEnumerable<RulesetItemCategoryRule> rules)
        {
            var ruleUpdates = rules.Where(rule => rule.RulesetId == rulesetId).ToList();

            if (!ruleUpdates.Any())
            {
                return;
            }
            
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var storeRules = await dbContext.RulesetItemCategoryRules.Where(rule => rule.RulesetId == rulesetId).ToListAsync();

                var newEntities = new List<RulesetItemCategoryRule>();

                foreach (var rule in ruleUpdates)
                {
                    var storeEntity = storeRules.Where(r => r.ItemCategoryId == rule.ItemCategoryId).FirstOrDefault();

                    if (storeEntity == null)
                    {
                        newEntities.Add(rule);
                    }
                    else
                    {
                        storeEntity = rule;
                        dbContext.RulesetItemCategoryRules.Update(storeEntity);
                    }
                }

                if (newEntities.Any())
                {
                    dbContext.RulesetItemCategoryRules.AddRange(newEntities);
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving RulesetItemCategoryRule changes to database: {ex}");
            }
        }

        public async Task<Ruleset> CreateRulesetAsync(Ruleset ruleset)
        {
            if (!IsValidRulesetName(ruleset.Name))
            {
                return null;
            }

            using (await _rulesetLock.WaitAsync($"{ruleset.Id}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    dbContext.Rulesets.Add(ruleset);

                    await dbContext.SaveChangesAsync();
                    
                    // TODO: Set Up default Action Type, Item Category, and Facility values

                    return ruleset;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());

                    return null;
                }
            }
        }

        private bool IsValidRulesetName(string name)
        {
            return true;
        }

        private bool IsValidDefaultRoundSeconds(int seconds)
        {
            return true;
        }

        private bool IsValidDefaultRounds(int rounds)
        {
            return true;
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
