// Credit to Lampjaw @ Voidwell.DaybreakGames for everything except player, world, & facility filtering
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
using Websocket.Client;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketMonitor : StatefulHostedService, IWebsocketMonitor, IDisposable
    {
        private readonly IStreamClient _client;
        private readonly IWebsocketHealthMonitor _healthMonitor;
        private readonly IWebsocketEventHandler _handler;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<WebsocketMonitor> _logger;

        private StreamState _lastStateChange;
        private Timer _timer;

        public override string ServiceName => "CensusMonitor";

        public List<string> CharacterSubscriptions = new List<string>();

        public HashSet<string> SubscribedCharacterIDs = new HashSet<string>();

        public int? SubscribedFacilityId;
        public int? SubscribedWorldId;


        public WebsocketMonitor(IStreamClient client, IWebsocketHealthMonitor healthMonitor, IWebsocketEventHandler handler,
            IScrimMessageBroadcastService messageService, ILogger<WebsocketMonitor> logger)
        {
            _client = client;
            _healthMonitor = healthMonitor;
            _handler = handler;
            _messageService = messageService;
            _logger = logger;

            _client.OnMessage(OnMessage)
                   .OnDisconnect(OnDisconnect);

            _messageService.RaiseTeamPlayerChangeEvent += ReceiveTeamPlayerChangeEvent;
            _messageService.RaiseMatchConfigurationUpdateEvent += ReceiveMatchConfigurationUpdateEvent;
        }


        #region Subscription Setup
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
                Worlds = new[] { "1", "10", "13", "17", "19", "40", "1000", "2000" },
                EventNames = eventNames
            };

            return subscription;
        }

        public void AddCharacterSubscription(string characterId)
        {
            //if (CharacterSubs.Contains(characterId))
            //{
            //    return;
            //}

            SubscribedCharacterIDs.Add(characterId);
            
            //if (CharacterSubscriptions.Contains(characterId))
            //{
            //    return;
            //}

            //CharacterSubscriptions.Add(characterId);
        }

        public void AddCharacterSubscriptions(IEnumerable<string> characterIds)
        {
            if (!characterIds.Any())
            {
                return;
            }

            _logger.LogInformation(string.Join(",", characterIds));

            SubscribedCharacterIDs.UnionWith(characterIds);

            _logger.LogInformation(SubscribedCharacterIDs.Count().ToString());

            //var newCharacterIds = characterIds.Where(id => !CharacterSubscriptions.Contains(id)).ToList();

            //CharacterSubscriptions.AddRange(newCharacterIds);

            //_logger.LogInformation(CharacterSubscriptions.Count().ToString());
        }

        public void RemoveCharacterSubscription(string characterId)
        {
            SubscribedCharacterIDs.Remove(characterId);
            
            //if (CharacterSubscriptions.Contains(characterId))
            //{
            //    CharacterSubscriptions.Remove(characterId);
            //}
        }

        public void RemoveCharacterSubscriptions(IEnumerable<string> characterIds)
        {
            SubscribedCharacterIDs.ExceptWith(characterIds);
            
            //foreach (var id in characterIds)
            //{
            //    RemoveCharacterSubscription(id);
            //}
        }

        public void RemoveAllCharacterSubscriptions()
        {
            SubscribedCharacterIDs.Clear();

            //CharacterSubscriptions.Clear();
        }

        public void SetFacilitySubscription(int facilityId)
        {
            var oldFacilityId = SubscribedFacilityId;

            if (facilityId == oldFacilityId)
            {
                return;
            }

            SubscribedFacilityId = facilityId;

            var oldFacilityIdString = oldFacilityId == null ? "null" : ((int)oldFacilityId).ToString();
            var subscribedFacilityIdString = SubscribedFacilityId == null ? "null" : ((int)SubscribedFacilityId).ToString();

            if (oldFacilityIdString != subscribedFacilityIdString)
            {
                _logger.LogInformation($"SubscribedFacilityId changed: {oldFacilityIdString} => {subscribedFacilityIdString}");
            }
        }

        public void SetWorldSubscription(int worldId)
        {
            var oldWorldId = SubscribedWorldId;

            if (worldId == oldWorldId)
            {
                return;
            }

            SubscribedWorldId = worldId;

            var oldWorldIdString = oldWorldId == null ? "null" : ((int)oldWorldId).ToString();
            var subscribedWorldIdString = SubscribedWorldId == null ? "null" : ((int)SubscribedWorldId).ToString();

            if (oldWorldIdString != subscribedWorldIdString)
            {
                _logger.LogInformation($"SubscribedWorldId changed: {oldWorldIdString} => {subscribedWorldIdString}");
            }    

        }
        #endregion Subscription Setup


        #region Stateful Hosted Service
        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting census stream monitor");

            await _client.ConnectAsync(CreateSubscription());

            _timer = new Timer(CheckDataHealth, null, 0, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping census stream monitor");

            if (_client == null)
            {
                return;
            }

            await _client?.DisconnectAsync();
            _timer?.Dispose();
        }

        public override async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await _client?.DisconnectAsync();
        }

        protected override Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((object)_lastStateChange);
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
        #endregion Stateful Hosted Service

        #region Message Payload Handling

        #pragma warning disable CS1998
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
                UpdateStateDetails(jMsg.ToObject<object>());

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
        #pragma warning restore CS1998

        private Task OnDisconnect(DisconnectionInfo info)
        {
            UpdateStateDetails(info.Exception?.Message ?? info.Type.ToString());
            _healthMonitor.ClearAllWorlds();
            return Task.CompletedTask;
        }

        private void UpdateStateDetails(object contents)
        {
            _lastStateChange = new Models.StreamState
            {
                LastStateChangeTime = DateTime.UtcNow,
                Contents = contents
            };
        }

        private async void CheckDataHealth(object state)
        {
            if (!_isRunning)
            {
                _healthMonitor.ClearAllWorlds();
                _timer?.Dispose();
                return;
            }

            if (!_healthMonitor.IsHealthy())
            {
                _logger.LogError(45234, "Census stream has failed health checks. Attempting resetting connection.");

                try
                {
                    await _client?.ReconnectAsync();
                }
                catch (Exception ex)
                {
                    UpdateStateDetails(ex.Message);

                    _logger.LogError(45235, ex, "Failed to reestablish connection to Census");
                }
            }
        }

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
                //if (CharacterSubscriptions.Contains(characterId))
                if (SubscribedCharacterIDs.Contains(characterId))
                {
                    _logger.LogDebug($"[cid] Payload receive for message: {message}");
                    return true;
                }
            }

            var attackerId = payload.Value<string>("attacker_character_id");

            if (!string.IsNullOrWhiteSpace(attackerId))
            {
                //if (CharacterSubscriptions.Contains(attackerId))
                if (SubscribedCharacterIDs.Contains(attackerId))
                {
                    _logger.LogDebug($"[aid] Payload receive for message: {message}");
                }
                //return CharacterSubscriptions.Contains(attackerId);
                return SubscribedCharacterIDs.Contains(attackerId);
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

            if (matchesWorld)
            {
                var timestamp = payload.Value<string>("timestamp");

                var idsString = $"Sub: w{SubscribedWorldId} f{SubscribedFacilityId} | PL: w{worldId} f{facilityId} t{timestamp}";

                var matchMessage = matchesFacility
                                        ? $"FacilityControl payload matching World & Facility detected. {idsString}"
                                        : $"FacilityControl payload matching World but not Facility detected. {idsString}";

                //_logger.LogInformation(matchMessage);
                _logger.LogDebug(matchMessage);
            }

            return (matchesFacility && matchesWorld);
        }

        #endregion Message Payload Handling

        #region Send/Receive Broadcast Events
        private void ReceiveTeamPlayerChangeEvent(object sender, ScrimMessageEventArgs<TeamPlayerChangeMessage> e)
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

        private void ReceiveMatchConfigurationUpdateEvent(object sender, ScrimMessageEventArgs<MatchConfigurationUpdateMessage> e)
        {
            var message = e.Message;
            var matchConfiguration = message.MatchConfiguration;

            SetFacilitySubscription(matchConfiguration.FacilityId);
            SetWorldSubscription(matchConfiguration.WorldId);

            if (matchConfiguration.SaveEventsToDatabase)
            {
                EnableEventStoring();
            }
            else
            {
                DisableEventStoring();
            }
        }
        #endregion Send/Receive Broadcast Events

        #region Scoring Toggles
        public void EnableScoring() => _handler.EnabledScoring();

        public void DisableScoring() => _handler.DisableScoring();

        public void EnableEventStoring() => _handler.EnabledEventStoring();

        public void DisableEventStoring() => _handler.DisableEventStoring();
        #endregion Scoring Toggles

        public void Dispose()
        {
            _client?.Dispose();
            _timer?.Dispose();

            _messageService.RaiseTeamPlayerChangeEvent -= ReceiveTeamPlayerChangeEvent;
            _messageService.RaiseMatchConfigurationUpdateEvent -= ReceiveMatchConfigurationUpdateEvent;
        }
    }
}
