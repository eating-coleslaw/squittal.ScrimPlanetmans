using squittal.ScrimPlanetmans.Models;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans
{
    public abstract class StatefulHostedService : IStatefulHostedService
    {
        protected bool _isRunning { get; set; } = false;

        public abstract string ServiceName { get; }


        public virtual async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            _isRunning = true;
            await StartInternalAsync(cancellationToken);
        }

        public virtual Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await UpdateStateAsync(true);
            await StartInternalAsync(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await UpdateStateAsync(false);
            await StopInternalAsync(cancellationToken);
        }

        public async Task<ServiceState> GetStateAsync(CancellationToken cancellationToken)
        {
            var details = await GetStatusAsync(cancellationToken);

            return new ServiceState
            {
                Name = ServiceName,
                IsEnabled = _isRunning,
                Details = details
            };
        }

        protected async Task UpdateStateAsync(bool isEnabled)
        {
            _isRunning = isEnabled;
            var state = await GetStateAsync(CancellationToken.None);
        }

        protected virtual Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(null as object);
        }

        public abstract Task StartInternalAsync(CancellationToken cancellationToken);
        public abstract Task StopInternalAsync(CancellationToken cancellationToken);
    }
}
