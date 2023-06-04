using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Services.Rulesets;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimRulesetManager : IScrimRulesetManager
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly IItemService _itemService;
        private readonly IRulesetDataService _rulesetDataService;
        private readonly IScrimMessageBroadcastService _messageService;
        public ILogger<ScrimRulesetManager> _logger;

        public Ruleset ActiveRuleset { get; private set; }

        private readonly int _defaultRulesetId = 1;

        private readonly AutoResetEvent _activateRulesetAutoEvent = new AutoResetEvent(true);

        public ScrimRulesetManager(IDbContextHelper dbContextHelper, IItemCategoryService itemCategoryService, IItemService itemService, IRulesetDataService rulesetDataService, IScrimMessageBroadcastService messageService, ILogger<ScrimRulesetManager> logger)
        {
            _dbContextHelper = dbContextHelper;
            _itemCategoryService = itemCategoryService;
            _itemService = itemService;
            _rulesetDataService = rulesetDataService;
            _messageService = messageService;
            _logger = logger;

            _messageService.RaiseRulesetRuleChangeEvent += async (s, e) => await HandleRulesetRuleChangeMesssage(s, e);

            _messageService.RaiseRulesetSettingChangeEvent += HandleRulesetSettingChangeMessage;
            _messageService.RaiseRulesetOverlayConfigurationChangeEvent += HandleRulesetOverlayConfigurationChangeMessage;
        }

        public async Task<IEnumerable<Ruleset>> GetRulesetsAsync(CancellationToken cancellationToken)
        {
            return await _rulesetDataService.GetAllRulesetsAsync(cancellationToken);
        }

        public async Task<Ruleset> GetActiveRulesetAsync(bool forceRefresh = false)
        {
            if (ActiveRuleset == null)
            {
                return await ActivateDefaultRulesetAsync();
            }
            else if (forceRefresh || ActiveRuleset.RulesetActionRules == null || !ActiveRuleset.RulesetActionRules.Any() || ActiveRuleset.RulesetItemCategoryRules == null || !ActiveRuleset.RulesetItemCategoryRules.Any())
            {
                await SetUpActiveRulesetAsync();
                return ActiveRuleset;
            }
            else
            {
                return ActiveRuleset;
            }
        }

        public async Task<Ruleset> ActivateRulesetAsync(int rulesetId)
        {
            _activateRulesetAutoEvent.WaitOne();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                Ruleset currentActiveRuleset = null;
                
                if (ActiveRuleset != null)
                {
                    currentActiveRuleset = ActiveRuleset;
                
                    if (rulesetId == currentActiveRuleset.Id)
                    {
                        _activateRulesetAutoEvent.Set();

                        return currentActiveRuleset;
                    }
                }

                var newActiveRuleset = await _rulesetDataService.GetRulesetFromIdAsync(rulesetId, CancellationToken.None);

                if (newActiveRuleset == null)
                {
                    _activateRulesetAutoEvent.Set();
                    
                    return null;
                }

                _rulesetDataService.SetActiveRulesetId(rulesetId);

                ActiveRuleset = newActiveRuleset;

                var message = currentActiveRuleset == null
                                    ? new ActiveRulesetChangeMessage(ActiveRuleset)
                                    : new ActiveRulesetChangeMessage(ActiveRuleset, currentActiveRuleset);

                _messageService.BroadcastActiveRulesetChangeMessage(message);

                _activateRulesetAutoEvent.Set();

                return ActiveRuleset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                _activateRulesetAutoEvent.Set();

                return null;
            }
        }

        public async Task<Ruleset> ActivateDefaultRulesetAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var ruleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.IsCustomDefault);

            if (ruleset == null)
            {
                _logger.LogInformation($"No custom default ruleset found. Loading default ruleset...");
                ruleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.IsDefault);
            }

            if (ruleset == null)
            {
                _logger.LogError($"Failed to activate default ruleset: no ruleset found");
                return null;
            }

            ActiveRuleset = await ActivateRulesetAsync(ruleset.Id);

            _logger.LogInformation($"Active ruleset loaded: {ActiveRuleset.Name}");

            return ActiveRuleset;
        }

        public async Task SetUpActiveRulesetAsync()
        {
            _activateRulesetAutoEvent.WaitOne();

            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var currentActiveRuleset = ActiveRuleset;

                if (currentActiveRuleset == null)
                {
                    _logger.LogError($"Failed to set up active ruleset: no ruleset found");

                    _activateRulesetAutoEvent.Set();

                    return;
                }

                var tempRuleset = await _rulesetDataService.GetRulesetFromIdAsync(currentActiveRuleset.Id, CancellationToken.None);

                if (tempRuleset == null)
                {
                    _logger.LogError($"Failed to set up active ruleset: temp ruleset is null");

                    _activateRulesetAutoEvent.Set();

                    return;
                }

                ActiveRuleset = tempRuleset;

                _rulesetDataService.SetActiveRulesetId(ActiveRuleset.Id);

                _logger.LogInformation($"Active ruleset collections loaded: {ActiveRuleset.Name}");

                _activateRulesetAutoEvent.Set();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to set up active ruleset: {ex}");

                _activateRulesetAutoEvent.Set();
            }
        }

        private async Task HandleRulesetRuleChangeMesssage(object sender, ScrimMessageEventArgs<RulesetRuleChangeMessage> e)
        {
            var changedRulesetId = e.Message.Ruleset.Id;

            if (changedRulesetId == ActiveRuleset.Id)
            {
                await SetUpActiveRulesetAsync();
            }
        }

        private void HandleRulesetSettingChangeMessage(object sender, ScrimMessageEventArgs<RulesetSettingChangeMessage> e)
        {
            var ruleset = e.Message.Ruleset;
            
            _activateRulesetAutoEvent.WaitOne();

            if (ruleset.Id != ActiveRuleset.Id)
            {
                _activateRulesetAutoEvent.Set();
                return;
            }

            ActiveRuleset.Name = ruleset.Name;
            ActiveRuleset.DefaultMatchTitle = ruleset.DefaultMatchTitle;
            ActiveRuleset.DefaultRoundLength = ruleset.DefaultRoundLength;
            ActiveRuleset.DefaultEndRoundOnFacilityCapture = ruleset.DefaultEndRoundOnFacilityCapture;

            ActiveRuleset.EnableRoundTimeLimit = ruleset.EnableRoundTimeLimit;
            ActiveRuleset.DefaultRoundLength = ruleset.DefaultRoundLength;
            ActiveRuleset.RoundTimerDirection = ruleset.RoundTimerDirection;

            ActiveRuleset.DefaultEndRoundOnFacilityCapture = ruleset.DefaultEndRoundOnFacilityCapture;

            ActiveRuleset.EndRoundOnPointValueReached = ruleset.EndRoundOnPointValueReached;
            ActiveRuleset.TargetPointValue = ruleset.TargetPointValue;
            ActiveRuleset.InitialPoints = ruleset.InitialPoints;

            ActiveRuleset.MatchWinCondition = ruleset.MatchWinCondition;
            ActiveRuleset.RoundWinCondition = ruleset.RoundWinCondition;

            ActiveRuleset.EnablePeriodicFacilityControlRewards = ruleset.EnablePeriodicFacilityControlRewards;
            ActiveRuleset.PeriodicFacilityControlPoints = ruleset.PeriodicFacilityControlPoints;
            ActiveRuleset.PeriodicFacilityControlInterval = ruleset.PeriodicFacilityControlInterval;
            ActiveRuleset.PeriodFacilityControlPointAttributionType = ruleset.PeriodFacilityControlPointAttributionType;

            _activateRulesetAutoEvent.Set();
        }

        private void HandleRulesetOverlayConfigurationChangeMessage(object sender, ScrimMessageEventArgs<RulesetOverlayConfigurationChangeMessage> e)
        {
            var ruleset = e.Message.Ruleset;
            var overlayConfiguration = e.Message.OverlayConfiguration;

            _activateRulesetAutoEvent.WaitOne();

            if (ruleset.Id != ActiveRuleset.Id)
            {
                _activateRulesetAutoEvent.Set();
                return;
            }

            ActiveRuleset.RulesetOverlayConfiguration.UseCompactLayout = overlayConfiguration.UseCompactLayout;
            ActiveRuleset.RulesetOverlayConfiguration.StatsDisplayType = overlayConfiguration.StatsDisplayType;
            ActiveRuleset.RulesetOverlayConfiguration.ShowStatusPanelScores = overlayConfiguration.ShowStatusPanelScores;

            _activateRulesetAutoEvent.Set();
        }

        public async Task<Ruleset> GetDefaultRulesetAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var ruleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.IsDefault);

            if (ruleset == null)
            {
                return null;
            }

            ruleset = await _rulesetDataService.GetRulesetFromIdAsync(ruleset.Id, CancellationToken.None);

            return ruleset;
        }

        public async Task SeedDefaultRuleset()
        {
            _logger.LogInformation($"Seeding Default Ruleset...");

            var stopWatchSetup = new Stopwatch();
            var stopWatchCollections = new Stopwatch();
            var stopWatchOverlay = new Stopwatch();
            var stopWatchActionRules = new Stopwatch();
            var stopWatchItemRules = new Stopwatch();
            var stopWatchItemCategoryRules = new Stopwatch();
            var stopWatchFacilityRules = new Stopwatch();
            var stopWatchFinalize = new Stopwatch();

            var stopWatchTotal = Stopwatch.StartNew();
            stopWatchSetup.Start();


            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var defaultRulesetId = _defaultRulesetId;

            var storeRuleset = await dbContext.Rulesets.FirstOrDefaultAsync(r => r.Id == defaultRulesetId);

            bool rulesetExistsInDb = false;

            RulesetOverlayConfiguration storeOverlayConfiguration = null;

            var storeActionRules = new List<RulesetActionRule>();
            var storeItemCategoryRules = new List<RulesetItemCategoryRule>();
            var storeItemRules = new List<RulesetItemRule>();
            var storeFacilityRules = new List<RulesetFacilityRule>();

            if (storeRuleset != null)
            {
                stopWatchCollections.Start();

                var storeRulesetWithCollections = await _rulesetDataService.GetRulesetFromIdAsync(storeRuleset.Id, CancellationToken.None, true, true);

                storeOverlayConfiguration = storeRulesetWithCollections.RulesetOverlayConfiguration;

                storeActionRules = storeRulesetWithCollections.RulesetActionRules.ToList();
                storeItemCategoryRules = storeRulesetWithCollections.RulesetItemCategoryRules.ToList();
                storeItemRules = storeRulesetWithCollections.RulesetItemRules.ToList();
                storeFacilityRules = storeRulesetWithCollections.RulesetFacilityRules.ToList();

                rulesetExistsInDb = true;
                
                stopWatchCollections.Stop();
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

            storeRuleset.DefaultMatchTitle = "PS2 Scrims";
            storeRuleset.IsDefault = true;
            storeRuleset.DefaultEndRoundOnFacilityCapture = false;

            storeRuleset.EnableRoundTimeLimit = true;
            storeRuleset.DefaultRoundLength = 900;
            storeRuleset.RoundTimerDirection = TimerDirection.Up;
            storeRuleset.DefaultEndRoundOnFacilityCapture = false;
            storeRuleset.EndRoundOnPointValueReached = false;
            storeRuleset.MatchWinCondition = MatchWinCondition.MostPoints;
            storeRuleset.RoundWinCondition = RoundWinCondition.NotApplicable;
            storeRuleset.EnablePeriodicFacilityControlRewards = false;

            // Get all async collection requests together
            var CollectionsTaskList = new List<Task>(); 

            var allItemCategoryIdsTask = _itemCategoryService.GetItemCategoryIdsAsync();
            CollectionsTaskList.Add(allItemCategoryIdsTask);

            var allWeaponItemCategoryIdsTask = _itemCategoryService.GetWeaponItemCategoryIdsAsync();
            CollectionsTaskList.Add(allWeaponItemCategoryIdsTask);

            var allWeaponItemsTask = _itemService.GetAllWeaponItemsAsync();
            CollectionsTaskList.Add(allWeaponItemsTask);

            await Task.WhenAll(CollectionsTaskList);

            var allItemCategoryIds = allItemCategoryIdsTask.Result;
            var allWeaponItemCategoryIds = allWeaponItemCategoryIdsTask.Result;
            var allWeaponItems = allWeaponItemsTask.Result;

            stopWatchSetup.Stop();

            stopWatchOverlay.Start();
            
            #region Overlay Configuration

            if (storeOverlayConfiguration == null)
            {
                storeOverlayConfiguration = new RulesetOverlayConfiguration()
                {
                    RulesetId = _defaultRulesetId,
                    UseCompactLayout = false,
                    StatsDisplayType = OverlayStatsDisplayType.InfantryScores,
                    ShowStatusPanelScores = null
                };

                dbContext.RulesetOverlayConfigurations.Add(storeOverlayConfiguration);
            }
            else
            {
                storeOverlayConfiguration.UseCompactLayout = false;
                storeOverlayConfiguration.StatsDisplayType = OverlayStatsDisplayType.InfantryScores;
                storeOverlayConfiguration.ShowStatusPanelScores = null;

                dbContext.RulesetOverlayConfigurations.Update(storeOverlayConfiguration);
            }

            #endregion Overlay Configuration

            stopWatchOverlay.Stop();

            stopWatchActionRules.Start();

            #region Action rules
            var defaultActionRules = GetDefaultActionRules();
            var createdActionRules = new List<RulesetActionRule>();
            var allActionRules = new List<RulesetActionRule>();

            var allActionEnumValues = GetScrimActionTypes().Where(a => a != ScrimActionType.None && a != ScrimActionType.Login && a != ScrimActionType.Logout);

            var allActionValues = new List<ScrimActionType>();
            allActionValues.AddRange(allActionEnumValues);
            allActionValues.AddRange(storeActionRules.Select(ar => ar.ScrimActionType).Where(a => !allActionValues.Contains(a)).ToList());

            foreach (var actionType in allActionValues)
            {
                var storeEntity = storeActionRules?.FirstOrDefault(r => r.ScrimActionType == actionType);
                var defaultEntity = defaultActionRules.FirstOrDefault(r => r.ScrimActionType == actionType);

                var isValidAction = storeEntity == null || allActionEnumValues.Any(enumValue => enumValue == storeEntity.ScrimActionType);

                if (storeEntity == null)
                {
                    if (defaultEntity != null)
                    {
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
                else if (isValidAction)
                {
                    if (defaultEntity != null)
                    {
                        storeEntity.Points = defaultEntity.Points;
                        storeEntity.DeferToItemCategoryRules = defaultEntity.DeferToItemCategoryRules;
                        storeEntity.ScrimActionTypeDomain = defaultEntity.ScrimActionTypeDomain;
                    }
                    else
                    {
                        storeEntity.Points = 0;
                        storeEntity.ScrimActionTypeDomain = ScrimAction.GetDomainFromActionType(storeEntity.ScrimActionType);
                    }

                    dbContext.RulesetActionRules.Update(storeEntity);
                    allActionRules.Add(storeEntity);
                }
                else
                {
                    dbContext.RulesetActionRules.Remove(storeEntity);
                }
            }

            if (createdActionRules.Any())
            {
                dbContext.RulesetActionRules.AddRange(createdActionRules);
            }
            #endregion Action rules

            stopWatchActionRules.Stop();

            stopWatchItemCategoryRules.Start();

            #region Item Category Rules
            var defaultItemCategoryRules = GetDefaultItemCategoryRules();
            var createdItemCategoryRules = new List<RulesetItemCategoryRule>();

            var allItemCategoryRules = new List<RulesetItemCategoryRule>();

            foreach (var categoryId in allItemCategoryIds)
            {
                var isWeaponItemCategoryId = (allWeaponItemCategoryIds.Contains(categoryId));

                var storeEntity = storeItemCategoryRules?.FirstOrDefault(r => r.ItemCategoryId == categoryId);
                var defaultEntity = defaultItemCategoryRules.FirstOrDefault(r => r.ItemCategoryId == categoryId);

                if (storeEntity == null)
                {
                    if (defaultEntity != null)
                    {
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
                        storeEntity.IsBanned = defaultEntity != null ? defaultEntity.IsBanned : false;
                        storeEntity.DeferToItemRules = defaultEntity != null ? defaultEntity.DeferToItemRules : false;

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
                dbContext.RulesetItemCategoryRules.AddRange(createdItemCategoryRules);
            }
            #endregion Item Category Rules

            stopWatchItemCategoryRules.Stop();

            stopWatchItemRules.Start();

            #region Item Rules
            var defaultItemRules = GetDefaultItemRules();
            var createdItemRules = new List<RulesetItemRule>();

            var allItemIds = new List<int>(defaultItemRules.Select(r => r.ItemId));
            if (storeItemRules != null)
            {
                allItemIds.AddRange(storeItemRules.Where(r => !allItemIds.Contains(r.ItemId)).Select(r => r.ItemId));
            }

            if (allWeaponItems != null)
            {
                allItemIds.AddRange(allWeaponItems.Where(r => !allItemIds.Contains(r.Id)).Select(r => r.Id));
            }

            var allItemRules = new List<RulesetItemRule>();

            foreach (var itemId in allItemIds)
            {
                var isWeaponItem = (allWeaponItems.Any(r => r.Id == itemId));

                var storeEntity = storeItemRules?.FirstOrDefault(r => r.ItemId == itemId);
                var defaultEntity = defaultItemRules.FirstOrDefault(r => r.ItemId == itemId);

                var categoryId = allWeaponItems.Where(i => i.Id == itemId).Select(i => i.ItemCategoryId).FirstOrDefault();

                var categoryDefersToItems = false;

                if (categoryId != null)
                {
                    categoryDefersToItems = allItemCategoryRules.Any(r => r.ItemCategoryId == categoryId && r.DeferToItemRules);
                }

                if (storeEntity == null)
                {
                    if (defaultEntity != null)
                    {
                        defaultEntity.RulesetId = defaultRulesetId;

                        createdItemRules.Add(defaultEntity);
                        allItemRules.Add(defaultEntity);
                    }
                    else if (isWeaponItem && categoryDefersToItems)
                    {
                        var defaultPoints = allItemCategoryRules.Where(r => r.ItemCategoryId == categoryId).Select(r => r.Points).FirstOrDefault();

                        var newEntity = BuildRulesetItemRule(defaultRulesetId, itemId, (int)categoryId, defaultPoints, false);

                        createdItemRules.Add(newEntity);
                        allItemRules.Add(newEntity);
                    }
                }
                else
                {
                    if (defaultEntity != null)
                    {
                        if (categoryId != null && categoryId != defaultEntity.ItemCategoryId)
                        {
                            defaultEntity.ItemCategoryId = (int)categoryId;
                        }
                        
                        defaultEntity.RulesetId = defaultRulesetId;

                        storeEntity = defaultEntity;

                        dbContext.RulesetItemRules.Update(storeEntity);
                        allItemRules.Add(storeEntity);
                    }
                    else if (!isWeaponItem || !categoryDefersToItems)
                    {
                        dbContext.RulesetItemRules.Remove(storeEntity);
                    }
                }
            }

            if (createdItemRules.Any())
            {
                dbContext.RulesetItemRules.AddRange(createdItemRules);
            }
            #endregion Item Rules

            stopWatchItemRules.Stop();

            stopWatchFacilityRules.Start();

            #region Facility Rules
            var defaultFacilityRules = GetDefaultFacilityRules();

            var createdFacilityRules = new List<RulesetFacilityRule>();

            var allFacilityRules = new List<RulesetFacilityRule>(storeFacilityRules);
            allFacilityRules.AddRange(defaultFacilityRules.Where(d => !allFacilityRules.Any(a => a.FacilityId == d.FacilityId)));

            foreach (var facilityRule in allFacilityRules)
            {
                var storeEntity = storeFacilityRules?.FirstOrDefault(r => r.FacilityId == facilityRule.FacilityId);
                var defaultEntity = defaultFacilityRules.FirstOrDefault(r => r.FacilityId == facilityRule.FacilityId);

                if (storeEntity == null)
                {
                    if (defaultEntity != null)
                    {
                        defaultEntity.RulesetId = defaultRulesetId;

                        createdFacilityRules.Add(defaultEntity);

                    }
                }
                else
                {
                    if (defaultEntity == null)
                    {
                        dbContext.RulesetFacilityRules.Remove(storeEntity);
                        allFacilityRules.Remove(storeEntity);
                    }
                }
            }

            if (createdFacilityRules.Any())
            {
                dbContext.RulesetFacilityRules.AddRange(createdFacilityRules);
            }
            #endregion Facility Rules

            stopWatchFacilityRules.Stop();

            stopWatchFinalize.Start();

            storeRuleset.RulesetOverlayConfiguration = storeOverlayConfiguration;

            storeRuleset.RulesetActionRules = allActionRules;
            storeRuleset.RulesetItemCategoryRules = allItemCategoryRules;
            storeRuleset.RulesetItemRules = allItemRules;
            storeRuleset.RulesetFacilityRules = allFacilityRules;

            if (rulesetExistsInDb)
            {
                dbContext.Rulesets.Update(storeRuleset);
            }
            else
            {
                dbContext.Rulesets.Add(storeRuleset);
            }

            await dbContext.SaveChangesAsync();

            stopWatchFinalize.Stop();

            stopWatchTotal.Stop();
            var elapsedMs = stopWatchTotal.ElapsedMilliseconds;

            var totalTime = $"Finished seeding Default Ruleset (elapsed: {elapsedMs / 1000.0}s)";
            var setupTime = $"\n\t\t   Setup: {stopWatchSetup.ElapsedMilliseconds / 1000.0}s";
            var collectionsTime = $"\n\t\t\t   Get Collections: {stopWatchCollections.ElapsedMilliseconds / 1000.0}s";
            var overlayTime = $"\n\t\t\t   Overlay Configuration: {stopWatchOverlay.ElapsedMilliseconds / 1000.0}s";
            var actionsTime = $"\n\t\t   Action Rules: {stopWatchActionRules.ElapsedMilliseconds / 1000.0}s";
            var itemCatsTime = $"\n\t\t   Item Category Rules: {stopWatchItemCategoryRules.ElapsedMilliseconds / 1000.0}s";
            var itemsTime = $"\n\t\t   Item Rules: {stopWatchItemRules.ElapsedMilliseconds / 1000.0}s";
            var facilitiesTime = $"\n\t\t   Facility Rules: {stopWatchFacilityRules.ElapsedMilliseconds / 1000.0}s";
            var finalizeTime = $"\n\t\t   Finalize: {stopWatchFinalize.ElapsedMilliseconds / 1000.0}s";

            _logger.LogInformation($"{totalTime}{setupTime}{collectionsTime}{overlayTime}{actionsTime}{itemCatsTime}{itemsTime}{facilitiesTime}{finalizeTime}");
        }

        private IEnumerable<RulesetItemCategoryRule> GetDefaultItemCategoryRules()
        {
            return new RulesetItemCategoryRule[]
            {
                BuildRulesetItemCategoryRule(2, 1, false, true),    // Knife
                BuildRulesetItemCategoryRule(3, 1, false, true),    // Pistol
                BuildRulesetItemCategoryRule(4, 1, false, true),    // Shotgun
                BuildRulesetItemCategoryRule(5, 1, false, true),    // SMG
                BuildRulesetItemCategoryRule(6, 1, false, true),    // LMG
                BuildRulesetItemCategoryRule(7, 1, false, true),    // Assault Rifle
                BuildRulesetItemCategoryRule(8, 1, false, true),    // Carbine
                BuildRulesetItemCategoryRule(11, 1, false, true),   // Sniper Rifle
                BuildRulesetItemCategoryRule(13, 0, false, true),   // Rocket Launcher
                BuildRulesetItemCategoryRule(24, 1, false, false),  // Crossbow
                BuildRulesetItemCategoryRule(100, 1, false, false), // Infantry (Nothing)
                BuildRulesetItemCategoryRule(157, 1, false, true),  // Hybrid Rifle (NSX-A Kuwa)

                // Universal Bans
                BuildRulesetItemCategoryRule(12, 0, true, false),  // Scout Rifle
                BuildRulesetItemCategoryRule(14, 0, true, false),  // Heavy Weapon
                BuildRulesetItemCategoryRule(19, 0, true, false),  // Battle Rifle
                BuildRulesetItemCategoryRule(102, 1, true, false)  // Infantry Weapons (AI Mana Turrets)
            };
        }

        private IEnumerable<RulesetItemRule> GetDefaultItemRules()
        {
            return new RulesetItemRule[]
            {
                /* These items are no longer in the Census API
                   6009459
                   6009524
                   6009583
                   803008
                 */
                
                // One-Hit Knives
                BuildRulesetItemRule(271, 2, 0, true),     // Carver
                BuildRulesetItemRule(285, 2, 0, true),     // Ripper
                BuildRulesetItemRule(286, 2, 0, true),     // Lumine Edge
                BuildRulesetItemRule(6005453, 2, 0, true), // Carver AE 
                BuildRulesetItemRule(6005452, 2, 0, true), // Ripper AE 
                BuildRulesetItemRule(6005451, 2, 0, true), // Lumine Edge AE 
                BuildRulesetItemRule(6009600, 2, 0, true), // NS Firebug 

                // Directive Rewards
                BuildRulesetItemRule(800623, 18, 0, true),   // C-4 ARX 
                BuildRulesetItemRule(77822, 7, 0, true),     // Gauss Prime 
                BuildRulesetItemRule(1909, 7, 0, true),      // Darkstar 
                BuildRulesetItemRule(1904, 7, 0, true),      // T1A Unity 
                BuildRulesetItemRule(1919, 8, 0, true),      // Eclipse VE3A 
                BuildRulesetItemRule(1869, 8, 0, true),      // 19A Fortuna 
                BuildRulesetItemRule(1914, 8, 0, true),      // TRAC-Shot 
                BuildRulesetItemRule(6005967, 157, 0, true), // NSX-A Kuwa 
                //BuildRulesetItemRule(6009583, 17, 0, true),  // Infernal Grenade 
                BuildRulesetItemRule(6003418, 17, 0, true),  // NSX Fujin 
                BuildRulesetItemRule(802025, 2, 0, true),    // Auraxium Slasher 
                BuildRulesetItemRule(800626, 2, 0, true),    // Auraxium Force-Blade 
                BuildRulesetItemRule(800624, 2, 0, true),    // Auraxium Mag-Cutter 
                BuildRulesetItemRule(800625, 2, 0, true),    // Auraxium Chainblade 
                BuildRulesetItemRule(803699, 6, 0, true),    // NS-15 Gallows (Bounty Directive) 
                BuildRulesetItemRule(1894, 6, 0, true),      // Betelgeuse 54-A 
                BuildRulesetItemRule(1879, 6, 0, true),      // NC6A GODSAW 
                BuildRulesetItemRule(1924, 6, 0, true),      // T9A "Butcher" 
                BuildRulesetItemRule(6005969, 3, 0, true),   // NSX-A Yawara (NSX Pistol) 
                BuildRulesetItemRule(1959, 3, 0, true),      // The Immortal 
                BuildRulesetItemRule(1889, 3, 0, true),      // The Executive 
                BuildRulesetItemRule(1954, 3, 0, true),      // The President 
                BuildRulesetItemRule(1964, 13, 0, true),     // The Kraken 
                BuildRulesetItemRule(1939, 4, 0, true),      // Chaos 
                BuildRulesetItemRule(1884, 4, 0, true),      // The Brawler 
                BuildRulesetItemRule(1934, 4, 0, true),      // Havoc 
                BuildRulesetItemRule(6005968, 5, 0, true),   // NSX-A Kappa 
                BuildRulesetItemRule(1949, 5, 0, true),      // Skorpios 
                BuildRulesetItemRule(1899, 5, 0, true),      // Tempest 
                BuildRulesetItemRule(1944, 5, 0, true),      // Shuriken 
                BuildRulesetItemRule(1979, 11, 0, true),     // Parsec VX3-A 
                BuildRulesetItemRule(1969, 11, 0, true),     // The Moonshot 
                BuildRulesetItemRule(1974, 11, 0, true),     // Bighorn .50M 

                // Semi-Auto Snipers
                BuildRulesetItemRule(6008652, 11, 0, true), // NSX "Ivory" Daimyo 
                BuildRulesetItemRule(6008670, 11, 0, true), // NSX "Networked" Daimyo 
                BuildRulesetItemRule(804255, 11, 0, true),  // NSX Daimyo 
                BuildRulesetItemRule(26002, 11, 0, true),   // Phantom VA23 
                BuildRulesetItemRule(7337, 11, 0, true),    // Phaseshift VX-S 
                BuildRulesetItemRule(89, 11, 0, true),      // VA39 Spectre 
                BuildRulesetItemRule(24000, 11, 0, true),   // Gauss SPR 
                BuildRulesetItemRule(24002, 11, 0, true),   // Impetus 
                BuildRulesetItemRule(88, 11, 0, true),      // 99SV 
                BuildRulesetItemRule(25002, 11, 0, true),   // KSR-35 

                // Gen-1 SMGs
                BuildRulesetItemRule(29000, 5, 0, true),    // Eridani SX5 
                BuildRulesetItemRule(6002772, 5, 0, true),  // Eridani SX5-AE 
                BuildRulesetItemRule(29005, 5, 0, true),    // Eridani SX5G 
                BuildRulesetItemRule(27000, 5, 0, true),    // AF-4 Cyclone 
                BuildRulesetItemRule(6002824, 5, 0, true),  // AF-4AE Cyclone 
                BuildRulesetItemRule(27005, 5, 0, true),    // AF-4G Cyclone 
                BuildRulesetItemRule(28000, 5, 0, true),    // SMG-46 Armistice 
                BuildRulesetItemRule(6002800, 5, 0, true),  // SMG-46AE Armistice 
                BuildRulesetItemRule(28005, 5, 0, true),    // SMG-46G Armistice 

                // Gen-3 SMGs
                BuildRulesetItemRule(6003925, 5, 0, true), // VE-S Canis 
                BuildRulesetItemRule(6003850, 5, 0, true), // MGR-S1 Gladius 
                BuildRulesetItemRule(6003879, 5, 0, true), // MG-S1 Jackal 

                // Anti-Personnel Explosives
                BuildRulesetItemRule(650, 18, 0, true),     // Tank Mine 
                BuildRulesetItemRule(6005961, 18, 0, true), // Tank Mine 
                BuildRulesetItemRule(6005962, 18, 0, true), // Tank Mine 
                BuildRulesetItemRule(1045, 18, 0, true),    // Proximity Mine 
                BuildRulesetItemRule(1044, 18, 0, true),    // Bouncing Betty 
                BuildRulesetItemRule(429, 18, 0, true),     // Claymore 
                BuildRulesetItemRule(6005243, 18, 0, true), // F.U.S.E. (Anti-Infantry) 
                BuildRulesetItemRule(6005963, 18, 0, true), // Proximity Mine 
                BuildRulesetItemRule(6005422, 18, 0, true), // Proximity Mine 

                // A7 Weapons
                BuildRulesetItemRule(6003943, 3, 0, true),  // NS-357 IA 
                BuildRulesetItemRule(6003793, 3, 0, true),  // NS-44L Showdown 
                BuildRulesetItemRule(6004992, 11, 0, true), // NS-AM8 Shortbow 

                // Campaign Reward Weapons
                //BuildRulesetItemRule(6009524, 17, 0, true), // Condensate Grenade 
                BuildRulesetItemRule(6009515, 2, 0, true),  // NS Icebreaker 
                BuildRulesetItemRule(6009516, 2, 0, true),  // NS Icebreaker 
                BuildRulesetItemRule(6009517, 2, 0, true),  // NS Icebreaker 
                BuildRulesetItemRule(6009518, 2, 0, true),  // NS Icebreaker 
                BuildRulesetItemRule(6009463, 2, 0, true),  // NS Icebreaker 

                // Misc. Weapons
                BuildRulesetItemRule(6050, 17, 0, true),    // Decoy Grenade 
                BuildRulesetItemRule(6004750, 17, 0, true), // Flamewake Grenade 
                //BuildRulesetItemRule(6009459, 17, 0, true), // Lightning Grenade 
                BuildRulesetItemRule(6005472, 17, 0, true), // NSX Raijin 
                BuildRulesetItemRule(6005304, 17, 0, true), // Smoke Grenade 
                BuildRulesetItemRule(882, 17, 0, true),     // Sticky Grenade 
                BuildRulesetItemRule(880, 17, 0, true),     // Sticky Grenade 
                BuildRulesetItemRule(881, 17, 0, true),     // Sticky Grenade 
                BuildRulesetItemRule(6005328, 17, 0, true), // Sticky Grenade 
                BuildRulesetItemRule(804795, 2, 0, true),   // NSX Amaterasu 

                // NSX Weapons
                BuildRulesetItemRule(44705, 17, 0, true),  // Plasma Grenade (NSX Defector Grenade Printer) 
                BuildRulesetItemRule(6008687, 2, 0, true), // Defector Claws 

                // Proposed Bans
                BuildRulesetItemRule(75490, 3, 0, true),  // NS Patriot Flare Gun 
                BuildRulesetItemRule(75521, 3, 0, true),  // VS Patriot Flare Gun 
                BuildRulesetItemRule(803009, 3, 0, true), // VS Triumph Flare Gun 
                BuildRulesetItemRule(75517, 3, 0, true),  // NC Patriot Flare Gun 
                BuildRulesetItemRule(803007, 3, 0, true), // NC Triumph Flare Gun 
                BuildRulesetItemRule(75519, 3, 0, true),  // TR Patriot Flare Gun 
                //BuildRulesetItemRule(803008, 3, 0, true)  // TR Triumph Flare Gun 
            };
        }

        private IEnumerable<RulesetActionRule> GetDefaultActionRules()
        {
            // MaxKillInfantry & MaxKillMax are worth 0 points
            return new RulesetActionRule[]
            {
                BuildRulesetActionRule(ScrimActionType.FirstBaseCapture, 9), // PIL 1: 18
                BuildRulesetActionRule(ScrimActionType.SubsequentBaseCapture, 18), // PIL 1: 36 
                BuildRulesetActionRule(ScrimActionType.InfantryKillMax, 6), // PIL 1: -12
                BuildRulesetActionRule(ScrimActionType.InfantryTeamkillInfantry, -2), // PIL 1: -3
                BuildRulesetActionRule(ScrimActionType.InfantryTeamkillMax, -8), // PIL 1: -15
                BuildRulesetActionRule(ScrimActionType.InfantrySuicide, -2), // PIL 1: -3
                BuildRulesetActionRule(ScrimActionType.MaxTeamkillMax, -8), // PIL 1: -15
                BuildRulesetActionRule(ScrimActionType.MaxTeamkillInfantry, -2), // PIL 1: -3
                BuildRulesetActionRule(ScrimActionType.MaxSuicide, -8), // PIL 1: -12
                BuildRulesetActionRule(ScrimActionType.MaxKillInfantry, 0), // PIL 1: 0
                BuildRulesetActionRule(ScrimActionType.MaxKillMax, 0), // PIL 1: 0
                BuildRulesetActionRule(ScrimActionType.InfantryKillInfantry, 0, true) // PIL 1: 0
            };
        }

        private IEnumerable<RulesetFacilityRule> GetDefaultFacilityRules()
        {
            return new RulesetFacilityRule[]
            {
                /* Hossin */
                BuildRulesetFacilityRule(266000, 4106), // Kessel's Crossing
                BuildRulesetFacilityRule(272000, 4112), // Bridgewater Shipping
                BuildRulesetFacilityRule(283000, 4123), // Nettlemire
                BuildRulesetFacilityRule(286000, 4126), // Four Fingers
                BuildRulesetFacilityRule(287070, 4266), // Fort Liberty
                BuildRulesetFacilityRule(302030, 4173), // Acan South
                BuildRulesetFacilityRule(303030, 4183), // Bitol Eastern
                BuildRulesetFacilityRule(305010, 4201), // Ghanan South
                BuildRulesetFacilityRule(307010, 4221), // Chac Fusion
                
                /* Esamir */
                BuildRulesetFacilityRule(239000, 18010), // Pale Canyon
                BuildRulesetFacilityRule(244610, 18067), // Rime Analtyics
                BuildRulesetFacilityRule(244620, 18068), // The Rink
                BuildRulesetFacilityRule(252020, 18050), // Elli Barracks
                BuildRulesetFacilityRule(254010, 18055), // Eisa Mountain Pass
                
                /* Indar */
                BuildRulesetFacilityRule(219, 2420), // Ceres
                BuildRulesetFacilityRule(230, 2431), // Xenotech
                BuildRulesetFacilityRule(3430, 2456), // Peris Eastern
                BuildRulesetFacilityRule(3620, 2466), // Rashnu
                
                /* Amerish */
                BuildRulesetFacilityRule(210002, 6357) // Wokuk Shipping
            };
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

        private RulesetActionRule BuildRulesetActionRule(ScrimActionType actionType, int points = 0, bool deferToItemCategoryRules = false)
        {
            return new RulesetActionRule
            {
                ScrimActionType = actionType,
                Points = points,
                DeferToItemCategoryRules = deferToItemCategoryRules,
                ScrimActionTypeDomain = ScrimAction.GetDomainFromActionType(actionType)
            };
        }

        private RulesetItemCategoryRule BuildRulesetItemCategoryRule(int rulesetId, int itemCategoryId, int points = 0, bool isBanned = false, bool deferToItemRules = false, bool deferToPlanetsideClassSettings = false, PlanetsideClassRuleSettings planetsideClassSettings = null)
        {
            if (planetsideClassSettings == null)
            {
                planetsideClassSettings = new PlanetsideClassRuleSettings();
            }

            return new RulesetItemCategoryRule
            {
                RulesetId = rulesetId,
                ItemCategoryId = itemCategoryId,
                Points = points,
                IsBanned = isBanned,
                DeferToItemRules = deferToItemRules,

                DeferToPlanetsideClassSettings = deferToPlanetsideClassSettings,

                InfiltratorIsBanned = planetsideClassSettings.InfiltratorIsBanned,
                InfiltratorPoints = planetsideClassSettings.InfiltratorPoints,
                LightAssaultIsBanned = planetsideClassSettings.LightAssaultIsBanned,
                LightAssaultPoints = planetsideClassSettings.LightAssaultPoints,
                MedicIsBanned = planetsideClassSettings.MedicIsBanned,
                MedicPoints = planetsideClassSettings.MedicPoints,
                EngineerIsBanned = planetsideClassSettings.EngineerIsBanned,
                EngineerPoints = planetsideClassSettings.EngineerPoints,
                HeavyAssaultIsBanned = planetsideClassSettings.HeavyAssaultIsBanned,
                HeavyAssaultPoints = planetsideClassSettings.HeavyAssaultPoints,
                MaxIsBanned = planetsideClassSettings.MaxIsBanned,
                MaxPoints = planetsideClassSettings.MaxPoints
            };
        }

        private RulesetItemCategoryRule BuildRulesetItemCategoryRule(int itemCategoryId, int points = 0, bool isBanned = false, bool deferToItemRules = false, bool deferToPlanetsideClassSettings = false, PlanetsideClassRuleSettings planetsideClassSettings = null)
        {
            if (planetsideClassSettings == null)
            {
                planetsideClassSettings = new PlanetsideClassRuleSettings();
            }

            return new RulesetItemCategoryRule
            {
                ItemCategoryId = itemCategoryId,
                Points = points,
                IsBanned = isBanned,
                DeferToItemRules = deferToItemRules,

                DeferToPlanetsideClassSettings = deferToPlanetsideClassSettings,

                InfiltratorIsBanned = planetsideClassSettings.InfiltratorIsBanned,
                InfiltratorPoints = planetsideClassSettings.InfiltratorPoints,
                LightAssaultIsBanned = planetsideClassSettings.LightAssaultIsBanned,
                LightAssaultPoints = planetsideClassSettings.LightAssaultPoints,
                MedicIsBanned = planetsideClassSettings.MedicIsBanned,
                MedicPoints = planetsideClassSettings.MedicPoints,
                EngineerIsBanned = planetsideClassSettings.EngineerIsBanned,
                EngineerPoints = planetsideClassSettings.EngineerPoints,
                HeavyAssaultIsBanned = planetsideClassSettings.HeavyAssaultIsBanned,
                HeavyAssaultPoints = planetsideClassSettings.HeavyAssaultPoints,
                MaxIsBanned = planetsideClassSettings.MaxIsBanned,
                MaxPoints = planetsideClassSettings.MaxPoints
            };
        }

        private RulesetItemRule BuildRulesetItemRule(int rulesetId, int itemId, int itemCategoryId, int points = 0, bool isBanned = false, bool deferToPlanetsideClassSettings = false, PlanetsideClassRuleSettings planetsideClassSettings = null)
        {
            if (planetsideClassSettings == null)
            {
                planetsideClassSettings = new PlanetsideClassRuleSettings();
            }

            return new RulesetItemRule
            {
                RulesetId = rulesetId,
                ItemId = itemId,
                ItemCategoryId = itemCategoryId,
                Points = points,
                IsBanned = isBanned,

                DeferToPlanetsideClassSettings = deferToPlanetsideClassSettings,

                InfiltratorIsBanned = planetsideClassSettings.InfiltratorIsBanned,
                InfiltratorPoints = planetsideClassSettings.InfiltratorPoints,
                LightAssaultIsBanned = planetsideClassSettings.LightAssaultIsBanned,
                LightAssaultPoints = planetsideClassSettings.LightAssaultPoints,
                MedicIsBanned = planetsideClassSettings.MedicIsBanned,
                MedicPoints = planetsideClassSettings.MedicPoints,
                EngineerIsBanned = planetsideClassSettings.EngineerIsBanned,
                EngineerPoints = planetsideClassSettings.EngineerPoints,
                HeavyAssaultIsBanned = planetsideClassSettings.HeavyAssaultIsBanned,
                HeavyAssaultPoints = planetsideClassSettings.HeavyAssaultPoints,
                MaxIsBanned = planetsideClassSettings.MaxIsBanned,
                MaxPoints = planetsideClassSettings.MaxPoints
            };
        }

        private RulesetItemRule BuildRulesetItemRule(int itemId, int itemCategoryId, int points = 0, bool isBanned = false, bool deferToPlanetsideClassSettings = false, PlanetsideClassRuleSettings planetsideClassSettings = null)
        {
            if (planetsideClassSettings == null)
            {
                planetsideClassSettings = new PlanetsideClassRuleSettings();
            }

            return new RulesetItemRule
            {
                ItemId = itemId,
                ItemCategoryId = itemCategoryId,
                Points = points,
                IsBanned = isBanned,

                DeferToPlanetsideClassSettings = deferToPlanetsideClassSettings,

                InfiltratorIsBanned = planetsideClassSettings.InfiltratorIsBanned,
                InfiltratorPoints = planetsideClassSettings.InfiltratorPoints,
                LightAssaultIsBanned = planetsideClassSettings.LightAssaultIsBanned,
                LightAssaultPoints = planetsideClassSettings.LightAssaultPoints,
                MedicIsBanned = planetsideClassSettings.MedicIsBanned,
                MedicPoints = planetsideClassSettings.MedicPoints,
                EngineerIsBanned = planetsideClassSettings.EngineerIsBanned,
                EngineerPoints = planetsideClassSettings.EngineerPoints,
                HeavyAssaultIsBanned = planetsideClassSettings.HeavyAssaultIsBanned,
                HeavyAssaultPoints = planetsideClassSettings.HeavyAssaultPoints,
                MaxIsBanned = planetsideClassSettings.MaxIsBanned,
                MaxPoints = planetsideClassSettings.MaxPoints
            };
        }

        private RulesetFacilityRule BuildRulesetFacilityRule(int facilityId, int mapRegionId)
        {
            return new RulesetFacilityRule
            {
                FacilityId = facilityId,
                MapRegionId = mapRegionId
            };
        }

        public async Task SeedScrimActionModels()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var createdEntities = new List<ScrimAction>();

            var allActionTypeValues = new List<ScrimActionType>();

            var enumValues = (ScrimActionType[])Enum.GetValues(typeof(ScrimActionType));

            allActionTypeValues.AddRange(enumValues);

            var storeEntities = await dbContext.ScrimActions.ToListAsync();

            allActionTypeValues.AddRange(storeEntities.Where(a => !allActionTypeValues.Contains(a.Action)).Select(a => a.Action).ToList());

            allActionTypeValues.Distinct().ToList();

            foreach (var value in allActionTypeValues)
            {
                try
                {

                    var storeEntity = storeEntities.FirstOrDefault(e => e.Action == value);
                    var isValidEnum = enumValues.Any(enumValue => enumValue == value);

                    if (storeEntity == null)
                    {
                        createdEntities.Add(ConvertToDbModel(value));
                    }
                    else if (isValidEnum)
                    {
                        storeEntity = ConvertToDbModel(value);
                        dbContext.ScrimActions.Update(storeEntity);
                    }
                    else
                    {
                        dbContext.ScrimActions.Remove(storeEntity);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            if (createdEntities.Any())
            {
                await dbContext.ScrimActions.AddRangeAsync(createdEntities);
            }

            await dbContext.SaveChangesAsync();

            _logger.LogInformation($"Seeded Scrim Actions store");
        }

        private ScrimAction ConvertToDbModel(ScrimActionType value)
        {
            var name = Enum.GetName(typeof(ScrimActionType), value);

            return new ScrimAction
            {
                Action = value,
                Name = name,
                Description = Regex.Replace(name, @"(\p{Ll})(\p{Lu})", "$1 $2"),
                Domain = ScrimAction.GetDomainFromActionType(value)
            };
        }

        public IEnumerable<ScrimActionType> GetScrimActionTypes()
        {
            return (ScrimActionType[])Enum.GetValues(typeof(ScrimActionType));
        }
    }
}
