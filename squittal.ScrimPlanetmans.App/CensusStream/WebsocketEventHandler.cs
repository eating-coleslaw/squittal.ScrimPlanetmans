using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DaybreakGames.Census;
using DaybreakGames.Census.JsonConverters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using squittal.LivePlanetmans.CensusStream;
using squittal.ScrimPlanetmans.CensusStream.Models;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Shared.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;

//using Microsoft.EntityFrameworkCore;
//using squittal.ScrimPlanetmans.Data;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketEventHandler : IWebsocketEventHandler
    {
        //private readonly IDbContextHelper _dbContextHelper;
        private readonly ICharacterService _characterService;
        //private readonly IScrimTeamsManager _teamsManager;
        private readonly IScrimMatchScorer _scorer;
        private readonly ILogger<WebsocketEventHandler> _logger;
        private readonly Dictionary<string, MethodInfo> _processMethods;

        private bool _isScoringEnabled = false;

        // Credit to Voidwell @Lampjaw
        private readonly JsonSerializer _payloadDeserializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new UnderscorePropertyNamesContractResolver(),
            Converters = new JsonConverter[]
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
        });

        public WebsocketEventHandler(ICharacterService characterService, IScrimMatchScorer scorer, ILogger<WebsocketEventHandler> logger)
        {
            //_dbContextHelper = dbContextHelper;
            _characterService = characterService;
            _scorer = scorer;
            _logger = logger;

            // Credit to Voidwell @ Lampjaw
            _processMethods = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<CensusEventHandlerAttribute>() != null)
                .ToDictionary(m => m.GetCustomAttribute<CensusEventHandlerAttribute>().EventName);
        }

        public void EnabledScoring()
        {
            _isScoringEnabled = true;
        }

        public void DisableScoring()
        {
            _isScoringEnabled = false;
        }

        public async Task Process(JToken message)
        {
            await ProcessServiceEvent(message);
        }

        // Credit to Voidwell @Lampjaw
        private async Task ProcessServiceEvent(JToken message)
        {
            var jPayload = message.SelectToken("payload");

            var payload = jPayload?.ToObject<PayloadBase>(_payloadDeserializer);
            var eventName = payload?.EventName;

            if (eventName == null)
            {
                return;
            }

            _logger.LogDebug("Payload received for event: {0}.", eventName);

            if (!_processMethods.ContainsKey(eventName))
            {
                _logger.LogWarning("No process method found for event: {0}", eventName);
                return;
            }

            if (payload.ZoneId.HasValue && payload.ZoneId.Value > 1000)
            {
                return;
            }

            try
            {
                var inputType = _processMethods[eventName].GetCustomAttribute<CensusEventHandlerAttribute>().PayloadType;
                var inputParam = jPayload.ToObject(inputType, _payloadDeserializer);

                await (Task)_processMethods[eventName].Invoke(this, new[] { inputParam });
            }
            catch (Exception ex)
            {
                _logger.LogError(75642, ex, "Failed to process websocket event: {0}.", eventName);
            }
        }

        #region Payload Handling

        [CensusEventHandler("Death", typeof(DeathPayload))]
        private async Task<Death> Process(DeathPayload payload)
        {
            string attackerId = payload.AttackerCharacterId;
            string victimId = payload.CharacterId;

            bool isValidAttackerId = (attackerId != null && attackerId.Length > 18);
            bool isValidVictimId = (victimId != null && victimId.Length > 18);

            //bool attackerIsTracked = (isValidAttackerId && _teamsManager.IsPlayerTracked(attackerId));
            //bool victimIsTracked = (isValidVictimId && _teamsManager.IsPlayerTracked(victimId));

            //if (!attackerIsTracked && !victimIsTracked)
            //{
            //    return null;
            //}

            try
            {
                var TaskList = new List<Task>();
                Task<OutfitMember> attackerOutfitTask = null;
                Task<OutfitMember> victimOutfitTask = null;

                //int? attackerTeamOrdinal;
                //int? victimTeamOrdinal;

                if (isValidAttackerId == true)
                {
                    attackerOutfitTask = _characterService.GetCharacterOutfitAsync(attackerId);
                    TaskList.Add(attackerOutfitTask);

                    //if (isValidAttackerId)
                    //{
                    //    attackerTeamOrdinal = _teamsManager.GetTeamOrdinalFromPlayerId(attackerId);
                    //}
                }

                if (isValidVictimId == true)
                {
                    victimOutfitTask = _characterService.GetCharacterOutfitAsync(victimId);
                    TaskList.Add(victimOutfitTask);

                    //if (victimIsTracked)
                    //{
                    //    victimTeamOrdinal = _teamsManager.GetTeamOrdinalFromPlayerId(victimId);
                    //}
                }

                await Task.WhenAll(TaskList);
                TaskList.Clear();

                int attackerFactionId = attackerOutfitTask.Result.FactionId;
                int victimFactionId = victimOutfitTask.Result.FactionId;

                //bool onSameTeam = _teamsManager.DoPlayersShareTeam(attackerId, victimId, out int? attackerTeamOrdinal, out int? victimTeamOrdinal);

                DeathEventType deathEventType;

                if (isValidAttackerId == true)
                {
                    if (attackerId == victimId)
                    {
                        deathEventType = DeathEventType.Suicide;
                    }
                    //else if (onSameTeam)
                    //{
                    //    deathEventType = DeathEventType.Teamkill;
                    //}
                    else if (attackerFactionId == victimFactionId)
                    {
                        deathEventType = DeathEventType.Teamkill;
                    }
                    else
                    {
                        deathEventType = DeathEventType.Kill;
                    }
                }
                else
                {
                    deathEventType = DeathEventType.Suicide;
                    attackerId = victimId;
                }

                var dataModel = new Death
                {
                    AttackerCharacterId = attackerId,
                    AttackerFireModeId = payload.AttackerFireModeId,
                    AttackerLoadoutId = payload.AttackerLoadoutId,
                    AttackerVehicleId = payload.AttackerVehicleId,
                    AttackerWeaponId = payload.AttackerWeaponId,
                    //AttackerOutfitId = attackerOutfitTask?.Result?.OutfitId,
                    //AttackerTeamOrdinal = attackerTeamOrdinal,
                    AttackerFactionId = attackerFactionId,
                    CharacterId = victimId,
                    CharacterLoadoutId = payload.CharacterLoadoutId,
                    //CharacterOutfitId = victimOutfitTask?.Result?.OutfitId,
                    //CharacterTeamOrdinal = victimTeamOrdinal,
                    CharacterFactionId = victimFactionId,
                    IsHeadshot = payload.IsHeadshot,
                    DeathEventType = deathEventType,
                    Timestamp = payload.Timestamp,
                    WorldId = payload.WorldId,
                    ZoneId = payload.ZoneId.Value
                };

                if (_isScoringEnabled)
                {
                    _scorer.ScoreDeathEvent(dataModel);
                }

                return dataModel;

                //dbContext.Deaths.Add(dataModel);
                //await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Ignore
                return null;
            }
        }

        [CensusEventHandler("PlayerLogin", typeof(PlayerLoginPayload))]
        private Task<PlayerLogin> Process(PlayerLoginPayload payload)
        {
            //using (var factory = _dbContextHelper.GetFactory())
            //{
            //    var dbContext = factory.GetDbContext();

                try
                {
                    var dataModel = new PlayerLogin
                    {
                        CharacterId = payload.CharacterId,
                        Timestamp = payload.Timestamp,
                        WorldId = payload.WorldId
                    };

                    return Task.FromResult(dataModel);

                    //dbContext.PlayerLogins.Add(dataModel);
                    //await dbContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    //Ignore
                    return null;
                }
            //}
        }

        [CensusEventHandler("PlayerLogout", typeof(PlayerLogoutPayload))]
        private Task<PlayerLogout> Process(PlayerLogoutPayload payload)
        {
            //bool updateCharacter;

            //using (var factory = _dbContextHelper.GetFactory())
            //{
            //    var dbContext = factory.GetDbContext();

                try
                {
                    var dataModel = new PlayerLogout
                    {
                        CharacterId = payload.CharacterId,
                        Timestamp = payload.Timestamp,
                        WorldId = payload.WorldId
                    };

                return Task.FromResult(dataModel);

                //dbContext.PlayerLogouts.Add(dataModel);
                //await dbContext.SaveChangesAsync();
            }
                catch (Exception)
                {
                    //Ignore
                    return null;
                }

                //var lastLoginTime = dbContext.PlayerLogins
                //                                .Where(l => l.CharacterId == payload.CharacterId)
                //                                .OrderByDescending(l => l.Timestamp)
                //                                .Select(l => l.Timestamp)
                //                                .FirstOrDefault();

                //updateCharacter = (lastLoginTime != default && (payload.Timestamp - lastLoginTime) > TimeSpan.FromMinutes(5));
            //}

            //if (updateCharacter)
            //{
            //    await _characterService.UpdateCharacterAsync(payload.CharacterId);
            //}
        }

        [CensusEventHandler("GainExperience", typeof(GainExperiencePayload))]
        private Task<GainExperience> Process(GainExperiencePayload payload)
        {
            var dataModel = new GainExperience
            {
                Id = Guid.NewGuid(),
                ExperienceId = payload.ExperienceId,
                CharacterId = payload.CharacterId,
                Amount = payload.Amount,
                LoadoutId = payload.LoadoutId,
                OtherId = payload.OtherId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            return Task.FromResult(dataModel);
            //return dataModel;
        }

        [CensusEventHandler("FacilityControl", typeof(FacilityControlPayload))]
        private Task<FacilityControl> Process(FacilityControlPayload payload)
        {
            var dataModel = new FacilityControl
            {
                FacilityId = payload.FacilityId,
                NewFactionId = payload.NewFactionId,
                OldFactionId = payload.OldFactionId,
                DurationHeld = payload.DurationHeld,
                OutfitId = payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value,
            };

            return Task.FromResult(dataModel);
        }

        [CensusEventHandler("PlayerFacilityCapture", typeof(PlayerFacilityCapturePayload))]
        private Task<PlayerFacilityCapture> Process(PlayerFacilityCapturePayload payload)
        {
            var dataModel = new PlayerFacilityCapture
            {
                CharacterId = payload.CharacterId,
                FacilityId = payload.FacilityId,
                OutfitId = payload.OutfitId == "0" ? null : payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            return Task.FromResult(dataModel);
        }

        [CensusEventHandler("PlayerFacilityDefend", typeof(PlayerFacilityDefendPayload))]
        private Task<PlayerFacilityDefend> Process(PlayerFacilityDefendPayload payload)
        {
            var dataModel = new PlayerFacilityDefend
            {
                CharacterId = payload.CharacterId,
                FacilityId = payload.FacilityId,
                OutfitId = payload.OutfitId == "0" ? null : payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            return Task.FromResult(dataModel);
        }
        #endregion

        public void Dispose()
        {
            return;
        }
    }
}
