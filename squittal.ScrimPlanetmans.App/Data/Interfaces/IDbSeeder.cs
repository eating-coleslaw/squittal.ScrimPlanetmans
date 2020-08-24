using System;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Data
{
    public interface IDbSeeder : IDisposable
    {
        Task SeedDatabase(CancellationToken cancellationToken);
    }
}
