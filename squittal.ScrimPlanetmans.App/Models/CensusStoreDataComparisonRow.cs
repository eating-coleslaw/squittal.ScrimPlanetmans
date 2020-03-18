
using squittal.ScrimPlanetmans.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Models
{
    public class CensusStoreDataComparisonRow
    {
        private readonly ICountableStoreService _countService;

        private readonly IUpdateable _refreshService;
        
        public string Name { get; set; }
        public int StoreCount { get; set; } = 0;
        public int CensusCount { get; set; } = 0;
        public bool IsRefreshable { get; private set; } = false;

        public bool IsLoadingStoreCount { get; private set; } = false;
        public bool IsLoadingCensusCount { get; private set; } = false;
        public bool IsRefreshingStore { get; private set; } = false;

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        // TODO: add CancellationTokens

        public CensusStoreDataComparisonRow(string name, ICountableStoreService countService)
        {
            Name = name;
            _countService = countService;
            IsRefreshable = false;
        }

        public CensusStoreDataComparisonRow(string name, ICountableStoreService countService, IUpdateable refreshService)
        {
            Name = name;
            _countService = countService;
            _refreshService = refreshService;
            IsRefreshable = true;
        }

        public async Task SetCounts()
        {
            _autoEvent.WaitOne();

            //await SetStoreCount();
            //await SetCensusCount();

            var TaskList = new List<Task>();

            var storeCountTask = SetStoreCount(true);
            TaskList.Add(storeCountTask);

            var censusCountTask = SetCensusCount(true);
            TaskList.Add(censusCountTask);

            await Task.WhenAll(TaskList);

            _autoEvent.Set();

            //if (_countService != null)
            //{
            //    StoreCount = await _countService.GetStoreCountAsync();
            //    CensusCount = await _countService.GetCensusCountAsync();
            //}
        }

        public async Task SetStoreCount(bool noLock = false)
        {
            if (!noLock)
            {
                _autoEvent.WaitOne();
            }

            if (_countService != null && !IsLoadingStoreCount)
            {
                IsLoadingStoreCount = true;
                StoreCount = await _countService.GetStoreCountAsync();
                IsLoadingStoreCount = false;
            }

            if (!noLock)
            {
                _autoEvent.Set();
            }
        }

        public async Task SetCensusCount(bool noLock = false)
        {
            if (!noLock)
            {
                _autoEvent.WaitOne();
            }

            if (_countService != null && !IsLoadingCensusCount)
            {
                IsLoadingCensusCount = true;
                CensusCount = await _countService.GetCensusCountAsync();
                IsLoadingCensusCount = false;
            }

            if (!noLock)
            {
                _autoEvent.Set();
            }
        }

        public async Task RefreshStore()
        {
            _autoEvent.WaitOne();
            if (IsRefreshable && !IsRefreshingStore)
            {
                IsRefreshingStore = true;
                await _refreshService.RefreshStore();
                IsRefreshingStore = false;
            }
            _autoEvent.Set();

            await SetStoreCount();
            await SetCensusCount();
        }
    }
}