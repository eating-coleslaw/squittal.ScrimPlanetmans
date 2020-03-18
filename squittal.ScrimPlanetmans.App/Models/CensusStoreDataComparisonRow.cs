
using squittal.ScrimPlanetmans.Services;
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

        public async Task SetCount()
        {
            if (_countService != null)
            {
                StoreCount = await _countService.GetStoreCountAsync();
                CensusCount = await _countService.GetCensusCountAsync();
            }
        }

        public async Task RefreshStore()
        {
            if (IsRefreshable)
            {
                await _refreshService.RefreshStore();
            }
        }
    }
}