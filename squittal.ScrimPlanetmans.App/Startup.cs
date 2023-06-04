using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using squittal.ScrimPlanetmans.Services;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Services.Rulesets;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using squittal.ScrimPlanetmans.Services.ScrimMatchReports;
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
                options.UseSqlServer(Configuration.GetConnectionString("PlanetmansDbContext"),
                                        sqlServerOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.EnableRetryOnFailure(
                                                maxRetryCount: 5,
                                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                                errorNumbersToAdd: null);
                                        })
                        .EnableSensitiveDataLogging(false));


            services.AddCensusServices(options =>
                options.CensusServiceId = Environment.GetEnvironmentVariable("DaybreakGamesServiceKey", EnvironmentVariableTarget.User));
            services.AddCensusHelpers();

            services.AddSingleton<IDbContextHelper, DbContextHelper>();

            services.AddSingleton<IScrimMessageBroadcastService, ScrimMessageBroadcastService>();

            services.AddTransient<IFactionService, FactionService>();
            services.AddSingleton<IZoneService, ZoneService>();

            services.AddSingleton<IItemService, ItemService>();
            services.AddSingleton<IItemCategoryService, ItemCategoryService>();
            services.AddSingleton<IFacilityService, FacilityService>();
            services.AddTransient<IFacilityTypeService, FacilityTypeService>();
            services.AddTransient<IVehicleService, VehicleService>();

            services.AddTransient<IVehicleTypeService, VehicleTypeService>();
            services.AddTransient<IDeathEventTypeService, DeathEventTypeService>();

            services.AddSingleton<IScrimRulesetManager, ScrimRulesetManager>();

            services.AddSingleton<IScrimMatchDataService, ScrimMatchDataService>();

            services.AddSingleton<IWorldService, WorldService>();
            services.AddSingleton<ICharacterService, CharacterService>();
            services.AddSingleton<IOutfitService, OutfitService>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddTransient<ILoadoutService, LoadoutService>();

            services.AddSingleton<IScrimTeamsManager, ScrimTeamsManager>();
            services.AddSingleton<IScrimPlayersService, ScrimPlayersService>();

            services.AddSingleton<IStatefulTimer, StatefulTimer>();
            services.AddSingleton<IScrimMatchEngine, ScrimMatchEngine>();
            services.AddSingleton<IScrimMatchScorer, ScrimMatchScorer>();

            services.AddSingleton<IConstructedTeamService, ConstructedTeamService>();

            services.AddSingleton<IRulesetDataService, RulesetDataService>();

            services.AddTransient<IScrimMatchReportDataService, ScrimMatchReportDataService>();

            services.AddSingleton<IDbSeeder, DbSeeder>();

            services.AddSingleton<IPeriodicPointsTimer, PeriodicPointsTimer>();
            services.AddSingleton<IScrimRoundEndCheckerService, ScrimRoundEndCheckerService>();

            services.AddSingleton<IOverlayStateService, OverlayStateService>();

            services.AddTransient<IStreamClient, StreamClient>();
            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();
            services.AddSingleton<IWebsocketHealthMonitor, WebsocketHealthMonitor>();

            services.AddHostedService<WebsocketMonitorHostedService>();

            services.AddSingleton<IApplicationDataLoader, ApplicationDataLoader>();
            services.AddHostedService<ApplicationDataLoaderHostedService>();

            services.AddTransient<ISqlScriptRunner, SqlScriptRunner>();

            services.AddTransient<DatabaseMaintenanceService>();
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
