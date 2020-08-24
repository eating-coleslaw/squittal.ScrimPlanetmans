using System;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Data
{
    public interface IDbSeeder : IDisposable
    {
        //Task OnApplicationStartup(CancellationToken cancellationToken);
        //Task OnApplicationShutdown(CancellationToken cancellationToken);
        Task SeedDatabase(CancellationToken cancellationToken);
    }
}
