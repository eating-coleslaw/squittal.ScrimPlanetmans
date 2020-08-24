using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Data
{
    public class DbSeederHostedService //: IHostedService
    {
        private readonly IDbSeeder _service;

        public DbSeederHostedService(IDbSeeder service)
        {
            _service = service;
        }

        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    return _service.OnApplicationStartup(cancellationToken);
        //}

        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    return _service.OnApplicationShutdown(cancellationToken);
        //}
    }
}
