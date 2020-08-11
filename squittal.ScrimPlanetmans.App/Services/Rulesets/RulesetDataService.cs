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

        public async Task<Ruleset> GetRulesetFromIdAsync(int rulesetId, CancellationToken cancellationToken)
        {
            try
            {
                using var factory = _dbContextHelper.GetFactory();
                var dbContext = factory.GetDbContext();

                var ruleset = await dbContext.Rulesets
                                                .Where(r => r.Id == rulesetId)
                                                .Include("RulesetItemCategoryRules")
                                                .Include("RulesetActionRules")
                                                .Include("RulesetItemCategoryRules.ItemCategory")
                                                .FirstOrDefaultAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return ruleset;
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"Task Request cancelled: CancellationToken rulesetId {rulesetId}");
                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Request cancelled: CancellationToken rulesetId {rulesetId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");

                return null;
            }
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
