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
    }
}
