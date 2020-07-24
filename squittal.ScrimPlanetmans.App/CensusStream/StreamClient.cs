// Credit to Lampjaw @ Voidwell.DaybreakGames
using DaybreakGames.Census.Stream;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Websocket.Client;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class StreamClient : IStreamClient
    {
        private const string CensusWebsocketEndpoint = "wss://push.planetside2.com/streaming";
        private const string CensusServiceNamespace = "ps2:v2";

        private readonly string CensusServiceKey;

        private static readonly Func<ClientWebSocket> wsFactory = new Func<ClientWebSocket>(() =>
        {
            return new ClientWebSocket { Options = { KeepAliveInterval = TimeSpan.FromSeconds(5) } };
        });
        
        private static readonly JsonSerializerSettings sendMessageSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly ILogger<StreamClient> _logger;


        public StreamClient(ILogger<StreamClient> logger)
        {
            _logger = logger;

            CensusServiceKey = Environment.GetEnvironmentVariable("DaybreakGamesServiceKey", EnvironmentVariableTarget.User);
        }

        private Func<string, Task> _onMessage;
        private Func<DisconnectionInfo, Task> _onDisconnected;

        private IWebsocketClient _client;

        public StreamClient OnDisconnect(Func<DisconnectionInfo, Task> onDisconnect)
        {
            _onDisconnected = onDisconnect;
            return this;
        }

        public StreamClient OnMessage(Func<string, Task> onMessage)
        {
            _onMessage = onMessage;
            return this;
        }

        public async Task ConnectAsync(CensusStreamSubscription subscription)
        {
            _client = new WebsocketClient(GetEndpoint(), wsFactory)
            {
                ReconnectTimeout = TimeSpan.FromSeconds(35),
                ErrorReconnectTimeout = TimeSpan.FromSeconds(30)
            };

            _client.DisconnectionHappened.Subscribe(info =>
            {
                _logger.LogWarning(75421, $"Stream disconnected: {info.Type}: {info.Exception}");

                if (_onDisconnected != null)
                {
                    Task.Run(() => _onDisconnected(info));
                }
            });

            _client.ReconnectionHappened.Subscribe(info =>
            {
                _logger.LogInformation($"Stream reconnection occured: {info.Type}");

                if (subscription.EventNames.Any())
                {
                    var sMessage = JsonConvert.SerializeObject(subscription, sendMessageSettings);

                    _client.Send(sMessage);

                    _logger.LogInformation($"Subscribed to census with: {sMessage}");
                }
            });

            _client.MessageReceived.Subscribe(msg =>
            {
                if (_onMessage != null)
                {
                    Task.Run(() => _onMessage(msg.Text));
                }
            });

            await _client.Start();
        }

        public Task DisconnectAsync()
        {
            _client?.Dispose();
            return Task.CompletedTask;
        }

        public Task ReconnectAsync()
        {
            return _client?.Reconnect();
        }

        private Uri GetEndpoint()
        {
            return new Uri($"{CensusWebsocketEndpoint}?environment={CensusServiceNamespace}&service-id=s:{CensusServiceKey}");
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
