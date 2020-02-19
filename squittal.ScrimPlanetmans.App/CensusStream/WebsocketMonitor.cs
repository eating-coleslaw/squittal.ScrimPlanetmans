using DaybreakGames.Census.Stream;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using squittal.ScrimPlanetmans.Hubs;
using squittal.ScrimPlanetmans.CensusStream.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using squittal.ScrimPlanetmans.Shared.Models;
using System.Linq;
using squittal.ScrimPlanetmans.ScrimMatch.Events;
using squittal.ScrimPlanetmans.Models;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketMonitor : StatefulHostedService, IWebsocketMonitor, IDisposable
    {
        private readonly ICensusStreamClient _client;
        private readonly IWebsocketEventHandler _handler;
        private readonly ILogger<WebsocketMonitor> _logger;

        private CensusHeartbeat _lastHeartbeat;

        public override string ServiceName => "CensusMonitor";

        public List<string> CharacterSubscriptions = new List<string>();

        public event EventHandler<SimpleMessageEventArgs> RaiseSimpleMessageEvent;
        public delegate void SimpleMessageEventHandler(object sender, SimpleMessageEventArgs e);

        protected virtual void OnRaiseSimpleMessageChangeEvent(SimpleMessageEventArgs e)
        {
            RaiseSimpleMessageEvent?.Invoke(this, e);
        }

        private void SendSimpleMessageAddedMessage(string s)
        {
            OnRaiseSimpleMessageChangeEvent(new SimpleMessageEventArgs(s));
        }

        public WebsocketMonitor(ICensusStreamClient censusStreamClient, IWebsocketEventHandler handler, ILogger<WebsocketMonitor> logger)
        {
            _client = censusStreamClient;
            _handler = handler;
            _logger = logger;

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
            var eventNames = new List<string>
            {
                "Death",
                "PlayerLogin",
                "PlayerLogout"
            };

            eventNames.AddRange(ExperienceEventsBuilder.GetExperienceEvents());

            var subscription = new CensusStreamSubscription
            {
                Characters = new[] { "all" },
                Worlds = new[] { "all" },
                //EventNames = new[] { "Death", "PlayerLogin", "PlayerLogout" }
                EventNames = eventNames
            };

            return subscription;
        }

        public void AddCharacterSubscriptions(IEnumerable<string> characterIds)
        {
            if (!characterIds.Any())
            {
                return;
            }

            _logger.LogInformation(string.Join(",", characterIds));

            var newCharacterIds = characterIds.Where(id => !CharacterSubscriptions.Contains(id)).ToList();

            CharacterSubscriptions.AddRange(newCharacterIds);

            _logger.LogInformation(CharacterSubscriptions.Count().ToString());
        }

        public void RemoveCharacterSubscription(string characterId)
        {
            if (CharacterSubscriptions.Contains(characterId))
            {
                CharacterSubscriptions.Remove(characterId);
            }
        }

        public void RemoveCharacterSubscriptions(IEnumerable<string> characterIds)
        {
            foreach (var id in characterIds)
            {
                RemoveCharacterSubscription(id);
            }
        }

        public void RemoveAllCharacterSubscriptions()
        {
            CharacterSubscriptions.Clear();
        }

        public bool PayloadContainsSubscribedCharacter(JToken message)
        {
            var payload = message.SelectToken("payload");

            if (payload == null)
            {
                return false;
            }

            var characterId = payload.Value<string>("character_id");


            if (!string.IsNullOrWhiteSpace(characterId))
            {
                if (CharacterSubscriptions.Contains(characterId))
                {
                    _logger.LogDebug($"[cid] Payload receive for message: {message.ToString()}");
                    return true;
                }
            }

            var attackerId = payload.Value<string>("attacker_character_id");

            if (!string.IsNullOrWhiteSpace(attackerId))
            {
                if (CharacterSubscriptions.Contains(attackerId))
                {
                    _logger.LogDebug($"[aid] Payload receive for message: {message.ToString()}");
                }
                return CharacterSubscriptions.Contains(attackerId);
            }

            return false;
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

            //if (jMsg.SelectToken("payload").Value<string>("event_name") == "PlayerLogin")
            //{
            //    await _hubContext.Clients.All.SendAsync("ReceivePlayerLoginMessage", message);
            //}

            //else if (jMsg.SelectToken("payload").Value<string>("event_name") == "PlayerLogout")
            //{
            //    await _hubContext.Clients.All.SendAsync("ReceivePlayerLogoutMessage", message);
            //}

            if (PayloadContainsSubscribedCharacter(jMsg))
            {
                //await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

                SendSimpleMessageAddedMessage(message);

                await _handler.Process(jMsg);
            }

            //await _handler.Process(jMsg);
        }

        private Task OnDisconnect(string error)
        {
            _logger.LogInformation("Websocket Client Disconnected!");

            return Task.CompletedTask;
        }

        public async Task<ServiceState> GetStatus()
        {
            var status = await GetStateAsync(CancellationToken.None);

            if (status == null)
            {
                return null;
            }

            return status;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
