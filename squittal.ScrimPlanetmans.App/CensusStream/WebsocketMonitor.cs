using DaybreakGames.Census.Stream;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using squittal.ScrimPlanetmans.Hubs;
using squittal.ScrimPlanetmans.CensusStream.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketMonitor : StatefulHostedService, IWebsocketMonitor, IDisposable
    {
        private readonly ICensusStreamClient _client;
        //private readonly IWebsocketEventHandler _handler;
        private readonly ILogger<WebsocketMonitor> _logger;

        private readonly IHubContext<EventHub> _hubContext;

        private CensusHeartbeat _lastHeartbeat;

        public override string ServiceName => "CensusMonitor";

        public WebsocketMonitor(ICensusStreamClient censusStreamClient, /*IWebsocketEventHandler handler,*/ ILogger<WebsocketMonitor> logger, IHubContext<EventHub> hubContext)
        {
            _client = censusStreamClient;
            //_handler = handler;
            _logger = logger;

            _hubContext = hubContext;

            _client.Subscribe(CreateSubscription())
                    .OnMessage(OnMessage)
                   .OnDisconnect(OnDisconnect);
            //.Subscribe(CreateSubscription())
        }


        public async Task Subscribe(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting census stream subscription");

            _client.Subscribe(CreateSubscription())
                   .OnMessage(OnMessage)
                   .OnDisconnect(OnDisconnect);


            await _client?.ConnectAsync();
            //_client?.Subscribe(CreateSubscription());

            //var state = await GetStateAsync(cancellationToken);

            //if (!state.IsEnabled)
            //{
            //    await _client.ConnectAsync();
            //    state = await GetStateAsync(cancellationToken);
            //}

            //if (state.IsEnabled)
            //{
            //    _logger.LogInformation("Starting census stream subscription");
            //    _client.Subscribe(CreateSubscription());
            //}
        }

        private CensusStreamSubscription CreateSubscription()
        {
            var subscription = new CensusStreamSubscription
            {
                Characters = new[] { "all" },
                Worlds = new[] { "all" },
                EventNames = new[] { "Death", "PlayerLogin", "PlayerLogout" }
            };

            return subscription;
        }

        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting census stream monitor");

            try
            {
                await _client.ConnectAsync();
            }
            catch (Exception ex)
            {
                await _client?.DisconnectAsync();
                await UpdateStateAsync(false);

                _logger.LogError(91435, ex, "Failed to establish initial connection to Census. Will not attempt to reconnect.");
            }
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping census stream monitor");

            if (_client == null)
            {
                return;
            }

            await _client.DisconnectAsync();
        }

        public override async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await _client?.DisconnectAsync();
        }

        protected override Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((object)_lastHeartbeat);
        }

        private async Task OnMessage(string message)
        {
            if (message == null)
            {
                return;
            }

            JToken jMsg;

            try
            {
                jMsg = JToken.Parse(message);
            }
            catch (Exception)
            {
                _logger.LogError(91097, "Failed to parse message: {0}", message);
                return;
            }

            if (jMsg.Value<string>("type") == "heartbeat")
            {
                _lastHeartbeat = new CensusHeartbeat
                {
                    LastHeartbeat = DateTime.UtcNow,
                    Contents = jMsg.ToObject<object>()
                };

                return;
            }

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

            //await _handler.Process(jMsg);
        }



        private Task OnDisconnect(string error)
        {
            _logger.LogInformation("Websocket Client Disconnected!");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
