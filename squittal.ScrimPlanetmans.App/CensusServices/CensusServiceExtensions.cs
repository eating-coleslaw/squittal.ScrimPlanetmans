using Microsoft.Extensions.DependencyInjection;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public static class CensusServiceExtensions
    {
        public static IServiceCollection AddCensusHelpers(this IServiceCollection services)
        {
            services.AddSingleton<CensusCharacter>();
            services.AddSingleton<CensusFaction>();
            services.AddSingleton<CensusItem>();
            services.AddSingleton<CensusItemCategory>();
            services.AddSingleton<CensusLoadout>();
            services.AddSingleton<CensusOutfit>();
            services.AddSingleton<CensusProfile>();
            services.AddSingleton<CensusTitle>();
            services.AddSingleton<CensusWorld>();
            services.AddSingleton<CensusZone>();

            return services;
        }
    }
}
