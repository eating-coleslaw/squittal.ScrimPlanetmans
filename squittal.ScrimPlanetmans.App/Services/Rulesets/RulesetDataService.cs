using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Models.Forms;
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.Rulesets
{
    public class RulesetDataService : IRulesetDataService
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IFacilityService _facilityService;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<RulesetDataService> _logger;

        private ConcurrentDictionary<int, Ruleset> RulesetsMap { get; set; } = new ConcurrentDictionary<int, Ruleset>();

        public int ActiveRulesetId { get; private set; }

        public int CustomDefaultRulesetId { get; private set; }

        private readonly int _rulesetBrowserPageSize = 15;

        public int DefaultRulesetId { get; } = 1;

        private readonly KeyedSemaphoreSlim _rulesetLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _actionRulesLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _itemCategoryRulesLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _facilityRulesLock = new KeyedSemaphoreSlim();

        public AutoResetEvent _defaultRulesetAutoEvent = new AutoResetEvent(true);

        public static Regex RulesetNameRegex { get; } = new Regex("^([A-Za-z0-9()\\[\\]\\-_'.][ ]{0,1}){1,49}[A-Za-z0-9()\\[\\]\\-_'.]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex RulesetDefaultMatchTitleRegex => RulesetNameRegex;

        public RulesetDataService(IDbContextHelper dbContextHelper, IFacilityService facilityService, IScrimMessageBroadcastService messageService, ILogger<RulesetDataService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _facilityService = facilityService;
            _messageService = messageService;
            _logger = logger;
        }

        #region GET methods
        public async Task<PaginatedList<Ruleset>> GetRulesetListAsync(int? pageIndex, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var rulesetsQuery = dbContext.Rulesets
                                                .AsQueryable()
                                                .AsNoTracking()
                                                .OrderByDescending(r => r.Id == ActiveRulesetId)
                                                .ThenByDescending(r => r.IsCustomDefault)
                                                .ThenByDescending(r => r.IsDefault)
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
        
        public  async Task<IEnumerable<Ruleset>> GetAllRulesetsAsync(CancellationToken cancellationToken)
        {
            if (RulesetsMap.Count == 0 || !RulesetsMap.Any())
            {
                await SetUpRulesetsMapAsync(cancellationToken);
            }

            if (RulesetsMap  == null || !RulesetsMap.Any())
            {
                return null;
            }

            return RulesetsMap.Values.ToList();
        }

        public async Task<Ruleset> GetRulesetFromIdAsync(int rulesetId, CancellationToken cancellationToken, bool includeCollections = true)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                Ruleset ruleset;

                ruleset = await dbContext.Rulesets
                                               .Where(r => r.Id == rulesetId)
                                               .FirstOrDefaultAsync(cancellationToken);

                if (includeCollections)
                {
                    var TaskList = new List<Task>();

                    var actionRulesTask = GetRulesetActionRulesAsync(rulesetId, CancellationToken.None);
                    TaskList.Add(actionRulesTask);

                    var itemCategoryRulesTask = GetRulesetItemCategoryRulesAsync(rulesetId, CancellationToken.None);
                    TaskList.Add(itemCategoryRulesTask);

                    var facilityRulesTask = GetRulesetFacilityRulesAsync(rulesetId, CancellationToken.None);
                    TaskList.Add(facilityRulesTask);

                    await Task.WhenAll(TaskList);

                    ruleset.RulesetActionRules = actionRulesTask.Result.ToList();
                    ruleset.RulesetItemCategoryRules = itemCategoryRulesTask.Result.ToList();
                    ruleset.RulesetFacilityRules = facilityRulesTask.Result.ToList();
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

        public async Task<Ruleset> GetRulesetWithFacilityRules(int rulesetId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                Ruleset ruleset;

                    ruleset = await dbContext.Rulesets
                                                .Where(r => r.Id == rulesetId)
                                                .Include("RulesetFacilityRules")
                                                .Include("RulesetFacilityRules.MapRegion")
                                                .FirstOrDefaultAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return ruleset;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetRulesetWithFacilityRules rulesetId {rulesetId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetRulesetWithFacilityRules rulesetId {rulesetId}");
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

        public async Task<IEnumerable<RulesetFacilityRule>> GetRulesetFacilityRulesAsync(int rulesetId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var rules = await dbContext.RulesetFacilityRules
                                               .Where(r => r.RulesetId == rulesetId)
                                               .Include("MapRegion")
                                               .OrderBy(r => r.MapRegion.ZoneId)
                                               .ThenBy(r => r.MapRegion.FacilityName)
                                               .ToListAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return rules;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetRulesetFacilityRulesAsync rulesetId {rulesetId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetRulesetFacilityRulesAsync rulesetId {rulesetId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        public async Task<IEnumerable<RulesetFacilityRule>> GetUnusedRulesetFacilityRulesAsync(int rulesetId, CancellationToken cancellationToken)
        {
            try
            {
                var usedFacilities = await GetRulesetFacilityRulesAsync(rulesetId, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                var allFacilities = await _facilityService.GetScrimmableMapRegionsAsync();
                
                if (usedFacilities == null)
                {
                    return allFacilities?.Select(a => ConvertToFacilityRule(rulesetId, a)).ToList();
                }

                return allFacilities?.Where(a => !usedFacilities.Any(u => u.FacilityId == a.FacilityId))
                                        .Select(a => ConvertToFacilityRule(rulesetId, a))
                                        .OrderBy(r => r.MapRegion.ZoneId)
                                        .ThenBy(r => r.MapRegion.FacilityName)
                                        .ToList();
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: GetRulesetFacilityRulesAsync rulesetId {rulesetId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: GetRulesetFacilityRulesAsync rulesetId {rulesetId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
        }

        private RulesetFacilityRule ConvertToFacilityRule(int rulesetId, MapRegion mapRegion)
        {
            return new RulesetFacilityRule
            {
                RulesetId = rulesetId,
                FacilityId = mapRegion.FacilityId,
                MapRegionId = mapRegion.Id,
                MapRegion = mapRegion
            };
        }
        #endregion GET methods


        #region SAVE / UPDATE methods
        public async Task<bool> UpdateRulesetInfo(Ruleset rulesetUpdate, CancellationToken cancellationToken)
        {
            var updateId = rulesetUpdate.Id;
            
            if (updateId == DefaultRulesetId || rulesetUpdate.IsDefault)
            {
                return false;
            }
            
            var updateName = rulesetUpdate.Name;
            var updateRoundLength = rulesetUpdate.DefaultRoundLength;
            var updateMatchTitle = rulesetUpdate.DefaultMatchTitle;

            if (!IsValidRulesetName(updateName))
            {
                _logger.LogError($"Error updating Ruleset {updateId} info: invalid ruleset name");
                return false;
            }

            if (!IsValidRulesetDefaultRoundLength(updateRoundLength))
            {
                _logger.LogError($"Error updating Ruleset {updateId} info: invalid default round length");
                return false;
            }

            if (!IsValidRulesetDefaultMatchTitle(updateMatchTitle))
            {
                _logger.LogError($"Error updating Ruleset {updateId} info: invalid default match title");
            }

            using (await _rulesetLock.WaitAsync($"{updateId}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeEntity = await GetRulesetFromIdAsync(updateId, cancellationToken, false);

                    if (storeEntity == null)
                    {
                        return false;
                    }

                    var oldName = storeEntity.Name;
                    var oldIsHidden = storeEntity.DefaultRoundLength;

                    storeEntity.Name = updateName;
                    storeEntity.DefaultRoundLength = updateRoundLength;
                    storeEntity.DefaultMatchTitle = updateMatchTitle;
                    storeEntity.DateLastModified = DateTime.UtcNow;

                    dbContext.Rulesets.Update(storeEntity);

                    await dbContext.SaveChangesAsync();

                    await SetUpRulesetsMapAsync(cancellationToken);

                    // TODO: broadcast Ruleset Info Change Message

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating Ruleset {updateId} info: {ex}");
                    return false;
                }
            }
        }

        /*
         * Upsert New or Modified RulesetActionRules for a specific ruleset.
         */
        public async Task SaveRulesetActionRules(int rulesetId, IEnumerable<RulesetActionRule> rules)
        {
            if (rulesetId == DefaultRulesetId)
            {
                return;
            }
            
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

                    var storeRuleset = await dbContext.Rulesets.Where(r => r.Id == rulesetId).FirstOrDefaultAsync();
                    if (storeRuleset != null)
                    {
                        storeRuleset.DateLastModified = DateTime.UtcNow;
                        dbContext.Rulesets.Update(storeRuleset);
                    }

                    await dbContext.SaveChangesAsync();

                    var message = new RulesetRuleChangeMessage(storeRuleset, RulesetRuleChangeType.ActionRule);
                    _messageService.BroadcastRulesetRuleChangeMessage(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving RulesetActionRule changes to database: {ex}");
                }
            }
        }

        /*
         * Upsert New or Modified RulesetItemCategoryRules for a specific ruleset.
         */
        public async Task SaveRulesetItemCategoryRules(int rulesetId, IEnumerable<RulesetItemCategoryRule> rules)
        {
            if (rulesetId == DefaultRulesetId)
            {
                return;
            }

            using (await _itemCategoryRulesLock.WaitAsync($"{rulesetId}"))
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

                    var storeRuleset = await dbContext.Rulesets.Where(r => r.Id == rulesetId).FirstOrDefaultAsync();
                    if (storeRuleset != null)
                    {
                        storeRuleset.DateLastModified = DateTime.UtcNow;
                        dbContext.Rulesets.Update(storeRuleset);
                    }

                    await dbContext.SaveChangesAsync();

                    var message = new RulesetRuleChangeMessage(storeRuleset, RulesetRuleChangeType.ItemCategoryRule);
                    _messageService.BroadcastRulesetRuleChangeMessage(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving RulesetItemCategoryRule changes to database: {ex}");
                }
            }
        }

        /*
         * Upsert New or Modified RulesetFacilityRules for a specific ruleset.
         */
        public async Task SaveRulesetFacilityRules(int rulesetId, IEnumerable<RulesetFacilityRuleChange> rules)
        {
            if (rulesetId == DefaultRulesetId)
            {
                return;
            }

            using (await _facilityRulesLock.WaitAsync($"{rulesetId}"))
            {
                var ruleUpdates = rules.Where(rule => rule.RulesetFacilityRule.RulesetId == rulesetId).ToList();

                if (!ruleUpdates.Any())
                {
                    return;
                }

                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeRules = await dbContext.RulesetFacilityRules.Where(rule => rule.RulesetId == rulesetId).ToListAsync();

                    var newEntities = new List<RulesetFacilityRule>();

                    foreach (var rule in ruleUpdates)
                    {
                        var storeEntity = storeRules.Where(r => r.FacilityId == rule.RulesetFacilityRule.FacilityId).FirstOrDefault();

                        if (storeEntity == null)
                        {
                            if (rule.ChangeType == RulesetFacilityRuleChangeType.Add)
                            {
                                rule.RulesetFacilityRule.MapRegion = null;
                                newEntities.Add(rule.RulesetFacilityRule);
                            }
                        }
                        else
                        {
                            if (rule.ChangeType == RulesetFacilityRuleChangeType.Add)
                            {

                                rule.RulesetFacilityRule.MapRegion = null;
                                storeEntity = rule.RulesetFacilityRule;
                                dbContext.RulesetFacilityRules.Update(storeEntity);
                            }
                            else if (rule.ChangeType == RulesetFacilityRuleChangeType.Remove)
                            {
                                dbContext.RulesetFacilityRules.Remove(storeEntity);
                            }
                        }
                    }

                    if (newEntities.Any())
                    {
                        dbContext.RulesetFacilityRules.AddRange(newEntities);
                    }

                    var storeRuleset = await dbContext.Rulesets.Where(r => r.Id == rulesetId).FirstOrDefaultAsync();
                    if (storeRuleset != null)
                    {
                        storeRuleset.DateLastModified = DateTime.UtcNow;
                        dbContext.Rulesets.Update(storeRuleset);
                    }

                    await dbContext.SaveChangesAsync();

                    var message = new RulesetRuleChangeMessage(storeRuleset, RulesetRuleChangeType.FacilityRule);
                    _messageService.BroadcastRulesetRuleChangeMessage(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving RulesetFacilityRule changes to database: {ex}");
                }
            }
        }

        public async Task<Ruleset> SaveNewRulesetAsync(Ruleset ruleset)
        {
            ruleset = await CreateRulesetAsync(ruleset);

            if (ruleset == null)
            {
                return null;
            }

            var TaskList = new List<Task>();

            var itemCategoryRulesTask = SeedNewRulesetDefaultItemCategoryRules(ruleset.Id);
            TaskList.Add(itemCategoryRulesTask);

            var actionRulesTask = SeedNewRulesetDefaultActionRules(ruleset.Id);
            TaskList.Add(actionRulesTask);

            var facilityRulesTask = SeedNewRulesetDefaultFacilityRules(ruleset.Id);
            TaskList.Add(facilityRulesTask);

            await Task.WhenAll(TaskList);

            return ruleset;
        }

        private async Task<Ruleset> CreateRulesetAsync(Ruleset ruleset)
        {
            if (!IsValidRulesetName(ruleset.Name) || ruleset.IsDefault
                || !IsValidRulesetDefaultRoundLength(ruleset.DefaultRoundLength) || !IsValidRulesetDefaultMatchTitle(ruleset.DefaultMatchTitle))
            {
                return null;
            }


            using (await _rulesetLock.WaitAsync($"{ruleset.Id}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    ruleset.DateCreated = DateTime.UtcNow;

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

        /*
         * Seed default RulesetItemCategoryRules for a newly created ruleset. Will not do anything if the ruleset
         * already has any RulesetItemCategoryRules in the database.
        */
        private async Task SeedNewRulesetDefaultActionRules(int rulesetId)
        {
            using (await _actionRulesLock.WaitAsync($"{rulesetId}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeRulesCount = await dbContext.RulesetActionRules.Where(r => r.RulesetId == rulesetId).CountAsync();

                    if (storeRulesCount > 0)
                    {
                        return;
                    }

                    var defaultRulesetId = await dbContext.Rulesets.Where(r => r.IsDefault).Select(r => r.Id).FirstOrDefaultAsync();

                    var defaultRules = await dbContext.RulesetActionRules.Where(r => r.RulesetId == defaultRulesetId).ToListAsync();

                    if (defaultRules == null)
                    {
                        return;
                    }

                    dbContext.RulesetActionRules.AddRange(defaultRules.Select(r => BuildRulesetActionRule(rulesetId, r.ScrimActionType, r.Points, r.DeferToItemCategoryRules)));

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());

                    return;
                }
            }
        }

        private RulesetActionRule BuildRulesetActionRule(int rulesetId, ScrimActionType actionType, int points = 0, bool deferToItemCategoryRules = false)
        {
            return new RulesetActionRule
            {
                RulesetId = rulesetId,
                ScrimActionType = actionType,
                Points = points,
                DeferToItemCategoryRules = deferToItemCategoryRules,
                ScrimActionTypeDomain = ScrimAction.GetDomainFromActionType(actionType)
            };
        }

        private async Task SeedNewRulesetDefaultItemCategoryRules(int rulesetId)
        {
            using (await _itemCategoryRulesLock.WaitAsync($"{rulesetId}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeRulesCount = await dbContext.RulesetItemCategoryRules.Where(r => r.RulesetId == rulesetId).CountAsync();

                    if (storeRulesCount > 0)
                    {
                        return;
                    }

                    var defaultRulesetId = await dbContext.Rulesets.Where(r => r.IsDefault).Select(r => r.Id).FirstOrDefaultAsync();

                    var defaultRules = await dbContext.RulesetItemCategoryRules.Where(r => r.RulesetId == defaultRulesetId).ToListAsync();

                    if (defaultRules == null)
                    {
                        return;
                    }

                    dbContext.RulesetItemCategoryRules.AddRange(defaultRules.Select(r => BuildRulesetItemCategoryRule(rulesetId, r.ItemCategoryId, r.Points)));

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());

                    return;
                }
            }
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

        private async Task SeedNewRulesetDefaultFacilityRules(int rulesetId)
        {
            using (await _facilityRulesLock.WaitAsync($"{rulesetId}"))
            {
                try
                {
                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    var storeRulesCount = await dbContext.RulesetFacilityRules.Where(r => r.RulesetId == rulesetId).CountAsync();

                    if (storeRulesCount > 0)
                    {
                        return;
                    }

                    var defaultRulesetId = await dbContext.Rulesets.Where(r => r.IsDefault).Select(r => r.Id).FirstOrDefaultAsync();

                    var defaultRules = await dbContext.RulesetFacilityRules.Where(r => r.RulesetId == defaultRulesetId).ToListAsync();

                    if (defaultRules == null)
                    {
                        return;
                    }

                    dbContext.RulesetFacilityRules.AddRange(defaultRules.Select(r => BuildRulesetFacilityRule(rulesetId, r.FacilityId, r.MapRegionId)));

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());

                    return;
                }
            }
        }

        private RulesetFacilityRule BuildRulesetFacilityRule(int rulesetId, int facilityId, int mapRegionId)
        {
            return new RulesetFacilityRule
            {
                RulesetId = rulesetId,
                FacilityId = facilityId,
                MapRegionId = mapRegionId
            };
        }
        #endregion SAVE / UPDATE methods

        #region Helper Methods
        private async Task SetUpRulesetsMapAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var rulesets = await dbContext.Rulesets.ToListAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                var newMap = new ConcurrentDictionary<int, Ruleset>();

                foreach (var rulesetId in RulesetsMap.Keys)
                {
                    if (!rulesets.Any(t => t.Id == rulesetId))
                    {
                        RulesetsMap.TryRemove(rulesetId, out var removeRuleset);
                    }
                }

                foreach (var ruleset in rulesets)
                {
                    RulesetsMap.AddOrUpdate(ruleset.Id, ruleset, (key, oldValue) => ruleset);
                }

                var customDefaultRuleset = rulesets.Where(r => r.IsCustomDefault).FirstOrDefault();
                CustomDefaultRulesetId = customDefaultRuleset != null ? customDefaultRuleset.Id : DefaultRulesetId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed setting up RulesetsMap: {ex}");
            }
        }
        #endregion Helper Methods

        #region Ruleset Activation / Defaulting / Favoriting
        public void SetActiveRulesetId(int rulesetId)
        {
            ActiveRulesetId = rulesetId;
        }

        public async Task<Ruleset> SetCustomDefaultRulesetAsync(int rulesetId)
        {
            _defaultRulesetAutoEvent.WaitOne();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var currentDefaultRuleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.IsCustomDefault);

                var newDefaultRuleset = await GetRulesetFromIdAsync(rulesetId, CancellationToken.None, false);

                if (newDefaultRuleset == null)
                {
                    _defaultRulesetAutoEvent.Set();
                    return null;
                }
                
                
                if (currentDefaultRuleset == null)
                {
                    newDefaultRuleset.IsCustomDefault = true;
                    dbContext.Rulesets.Update(newDefaultRuleset);
                }
                //else if (currentDefaultRuleset != null && currentDefaultRuleset.Id != rulesetId)
                else if (currentDefaultRuleset.Id != rulesetId)
                {
                    currentDefaultRuleset.IsCustomDefault = false;
                    dbContext.Rulesets.Update(currentDefaultRuleset);

                    newDefaultRuleset.IsCustomDefault = true;
                    dbContext.Rulesets.Update(newDefaultRuleset);
                }
                //else if (currentDefaultRuleset != null)
                else
                {
                    _defaultRulesetAutoEvent.Set();
                    return currentDefaultRuleset;
                }
                //else
                //{
                //    _defaultAutoEvent.Set();

                //    return null;
                //}

                await dbContext.SaveChangesAsync();

                _defaultRulesetAutoEvent.Set();

                _logger.LogInformation("Set ruleset {rulesetId} as new custom default ruleset");

                CustomDefaultRulesetId = newDefaultRuleset.Id;

                return newDefaultRuleset;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed setting ruleset {rulesetId} as new custom default ruleset: {ex}");

                _defaultRulesetAutoEvent.Set();

                return null;
            }
        }
        #endregion Ruleset Activation / Defaulting / Favoriting

        public static bool IsValidRulesetName(string name)
        {
            return RulesetNameRegex.Match(name).Success;
        }

        public static bool IsValidRulesetDefaultRoundLength(int seconds)
        {
            return seconds > 0;
        }

        public static bool IsValidRulesetDefaultMatchTitle(string title)
        {
            return RulesetDefaultMatchTitleRegex.Match(title).Success;
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
