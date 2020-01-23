using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using squittal.ScrimPlanetmans.App.Data;
using squittal.ScrimPlanetmans.Hubs;
using squittal.ScrimPlanetmans.CensusStream;
using Microsoft.AspNetCore.SignalR;
using squittal.ScrimPlanetmans.Services;

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

            services.AddCensusServices(options =>
                options.CensusServiceId = Environment.GetEnvironmentVariable("DaybreakGamesServiceKey", EnvironmentVariableTarget.User));

            services.AddSingleton<WebsocketMonitorService>();

            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();
            services.AddHostedService<WebsocketMonitorHostedService>();
            
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
                endpoints.MapHub<EventHub>("/eventhub");
            });

            app.Use(async (context, next) =>
            {
                var hubContext = context.RequestServices
                                        .GetRequiredService<IHubContext<EventHub>>();
                await next();
            });
        }
    }
}
