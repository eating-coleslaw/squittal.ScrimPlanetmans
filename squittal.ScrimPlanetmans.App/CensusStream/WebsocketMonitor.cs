using DaybreakGames.Census.Stream;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using squittal.ScrimPlanetmans.CensusStream.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using squittal.ScrimPlanetmans.Models;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketMonitor : StatefulHostedService, IWebsocketMonitor, IDisposable
    {
        private readonly ICensusStreamClient _client;
        private readonly IWebsocketEventHandler _handler;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<WebsocketMonitor> _logger;

        private CensusHeartbeat _lastHeartbeat;

        public override string ServiceName => "CensusMonitor";

        public List<string> CharacterSubscriptions = new List<string>();

        public int? SubscribedFacilityId;
        public int? SubscribedWorldId;


        public WebsocketMonitor(ICensusStreamClient censusStreamClient, IWebsocketEventHandler handler, IScrimMessageBroadcastService messageService, ILogger<WebsocketMonitor> logger)
        {
            _client = censusStreamClient;
            _handler = handler;
            _messageService = messageService;
            _logger = logger;

            _client.Subscribe(CreateSubscription())
                    .OnMessage(OnMessage)
                   .OnDisconnect(OnDisconnect);

            _messageService.RaiseTeamPlayerChangeEvent += ReceiveTeamPlayerChangeEvent;
            _messageService.RaiseMatchConfigurationUpdateEvent += ReceiveMatchConfigurationUpdateEvent;
        }


        public async Task Subscribe(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting census stream subscription");

            _client.Subscribe(CreateSubscription())
                   .OnMessage(OnMessage)
                   .OnDisconnect(OnDisconnect);


            await _client?.ConnectAsync();
        }

        private CensusStreamSubscription CreateSubscription()
        {
            var eventNames = new List<string>
            {
                "Death",
                "PlayerLogin",
                "PlayerLogout",
                "FacilityControl",
                "VehicleDestroy"
            };

            eventNames.AddRange(ExperienceEventsBuilder.GetExperienceEvents());

            var subscription = new CensusStreamSubscription
            {
                Characters = new[] { "all" },
                Worlds = new[] { "all" },
                EventNames = eventNames
            };

            return subscription;
        }

        public void AddCharacterSubscription(string characterId)
        {
            if (CharacterSubscriptions.Contains(characterId))
            {
                return;
            }

            CharacterSubscriptions.Add(characterId);
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

        public void SetFacilitySubscription(int facilityId)
        {
            SubscribedFacilityId = facilityId;
        }

        public void SetWorldSubscription(int worldId)
        {
            SubscribedWorldId = worldId;
        }

        public void EnableScoring() => _handler.EnabledScoring();

        public void DisableScoring() => _handler.DisableScoring();

        public void EnableEventStoring() => _handler.EnabledEventStoring();

        public void DisableEventStoring() => _handler.DisableEventStoring();

        private bool PayloadContainsSubscribedCharacter(JToken message)
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

        private bool PayloadContainsSubscribedFacility(JToken message)
        {
            var payload = message.SelectToken("payload");

            if (payload == null)
            {
                return false;
            }

            var eventName = payload.Value<string>("event_name");
            if (eventName != "FacilityControl")
            {
                return false;
            }

            var facilityId = payload.Value<int?>("facility_id");
            var worldId = payload.Value<int?>("world_id");


            bool matchesFacility = false;
            bool matchesWorld = false;

            if (facilityId != null && facilityId > 0)
            {
                matchesFacility = facilityId == SubscribedFacilityId;
            }

            if (worldId != null && worldId > 0)
            {
                matchesWorld = worldId == SubscribedWorldId;
            }

            return (matchesFacility && matchesWorld);
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

            if (PayloadContainsSubscribedCharacter(jMsg) || PayloadContainsSubscribedFacility(jMsg))
            {
                #pragma warning disable CS4014
                Task.Run(() =>
                {
                    _handler.Process(jMsg);

                }).ConfigureAwait(false);
                #pragma warning restore CS4014
            }
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

        private void SendSimpleMessage(string s)
        {
            _messageService.BroadcastSimpleMessage(s);
        }

        private void ReceiveTeamPlayerChangeEvent(object sender, TeamPlayerChangeEventArgs e)
        {
            var message = e.Message;
            var player = message.Player;

            switch (message.ChangeType)
            {
                case TeamPlayerChangeType.Add:
                    AddCharacterSubscription(player.Id);
                    break;

                case TeamPlayerChangeType.Remove:
                    RemoveCharacterSubscription(player.Id);
                    break;
            }
        }

        private void ReceiveMatchConfigurationUpdateEvent(object sender, MatchConfigurationUpdateEventArgs e)
        {
            var message = e.Message;
            var matchConfiguration = message.MatchConfiguration;

            SubscribedFacilityId = matchConfiguration.FacilityId;
            SubscribedWorldId = matchConfiguration.WorldId;

            if (matchConfiguration.SaveEventsToDatabase)
            {
                EnableEventStoring();
            }
            else
            {
                DisableEventStoring();
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
