using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace squittal.ScrimPlanetmans.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new PlanetmansDbContext(
                serviceProvider.GetRequiredService<
                DbContextOptions<PlanetmansDbContext>>()))
            {
                context.Database.Migrate();
                return;
            }
        }
    }
}
