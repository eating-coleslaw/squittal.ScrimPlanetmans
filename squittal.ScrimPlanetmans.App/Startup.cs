using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using squittal.ScrimPlanetmans.App.Data;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.Services;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;

namespace squittal.ScrimPlanetmans.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSignalR();

            services.AddDbContext<PlanetmansDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PlanetmansDbContext")));

            services.AddCensusServices(options =>
                options.CensusServiceId = Environment.GetEnvironmentVariable("DaybreakGamesServiceKey", EnvironmentVariableTarget.User));
            services.AddCensusHelpers();

            services.AddSingleton<IDbContextHelper, DbContextHelper>();

            services.AddTransient<IFactionService, FactionService>();
            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<IZoneService, ZoneService>();

            services.AddSingleton<IWorldService, WorldService>();
            services.AddSingleton<ICharacterService, CharacterService>();
            services.AddSingleton<IOutfitService, OutfitService>();
            services.AddSingleton<IProfileService, ProfileService>();

            services.AddSingleton<ScrimTeamsManagerService>();
            services.AddSingleton<PlanetsideDataService>();
            services.AddSingleton<WebsocketMonitorService>();

            services.AddSingleton<IScrimTeamsManager, ScrimTeamsManager>();
            services.AddSingleton<IScrimPlayersService, ScrimPlayersService>();

            services.AddSingleton<IStatefulTimer, StatefulTimer>(); // TODO: should/can this be Transient?
            services.AddSingleton<IScrimMatchEngine, ScrimMatchEngine>();
            services.AddSingleton<IScrimMatchScorer, ScrimMatchScorer>();

            services.AddSingleton<IDbSeeder, DbSeeder>();

            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();

            services.AddHostedService<WebsocketMonitorHostedService>();
            services.AddHostedService<DbSeederHostedService>();

            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
