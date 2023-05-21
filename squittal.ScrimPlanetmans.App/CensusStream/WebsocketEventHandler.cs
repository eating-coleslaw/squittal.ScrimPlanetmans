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
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.Models.Planetside.Events;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Services.ScrimMatch;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketEventHandler : IWebsocketEventHandler
    {
        private readonly IItemService _itemService;
        private readonly IFacilityService _facilityService;
        private readonly IVehicleService _vehicleService;
        private readonly IScrimTeamsManager _teamsManager;
        private readonly IScrimMatchScorer _scorer;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IScrimMatchDataService _scrimMatchService;
        private readonly IWebsocketHealthMonitor _healthMonitor;
        private readonly ILogger<WebsocketEventHandler> _logger;


        private readonly Dictionary<string, MethodInfo> _processMethods;

        private bool _isScoringEnabled = false;
        private bool _isEventStoringEnabled = false;

        private PayloadUniquenessFilter<DeathPayload> _deathFilter = new PayloadUniquenessFilter<DeathPayload>();
        private PayloadUniquenessFilter<VehicleDestroyPayload> _vehicleDestroyFilter = new PayloadUniquenessFilter<VehicleDestroyPayload>();
        private PayloadUniquenessFilter<GainExperiencePayload> _experienceFilter = new PayloadUniquenessFilter<GainExperiencePayload>();
        private PayloadUniquenessFilter<PlayerLoginPayload> _loginFilter = new PayloadUniquenessFilter<PlayerLoginPayload>();
        private PayloadUniquenessFilter<PlayerLogoutPayload> _logoutFilter = new PayloadUniquenessFilter<PlayerLogoutPayload>();
        private PayloadUniquenessFilter<FacilityControlPayload> _facilityControlFilter = new PayloadUniquenessFilter<FacilityControlPayload>();


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

        public WebsocketEventHandler(IScrimTeamsManager teamsManager, IScrimMatchScorer scorer, IItemService itemService, IFacilityService facilityService,
            IVehicleService vehicleService, IScrimMessageBroadcastService messageService, IScrimMatchDataService scrimMatchService,
            IDbContextHelper dbContextHelper, IWebsocketHealthMonitor healthMonitor, ILogger<WebsocketEventHandler> logger)
        {
            _teamsManager = teamsManager;
            _itemService = itemService;
            _messageService = messageService;
            _facilityService = facilityService;
            _vehicleService = vehicleService;
            _scorer = scorer;
            _dbContextHelper = dbContextHelper;
            _scrimMatchService = scrimMatchService;
            _healthMonitor = healthMonitor;
            _logger = logger;


            // Credit to Voidwell @ Lampjaw
            _processMethods = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<CensusEventHandlerAttribute>() != null)
                .ToDictionary(m => m.GetCustomAttribute<CensusEventHandlerAttribute>().EventName);
        }

        public void EnabledScoring() => _isScoringEnabled = true;

        public void DisableScoring() => _isScoringEnabled = false;

        public void EnabledEventStoring() => _isEventStoringEnabled = true;

        public void DisableEventStoring() => _isEventStoringEnabled = false;


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

            _healthMonitor.ReceivedEvent(payload.WorldId, eventName);

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
                switch (eventName)
                {
                    case "Death":
                        var deathParam = jPayload.ToObject<DeathPayload>(_payloadDeserializer);
                        await Process(deathParam);
                        break;

                    case "PlayerLogin":
                        var loginParam = jPayload.ToObject<PlayerLoginPayload>(_payloadDeserializer);
                        await Process(loginParam);
                        break;

                    case "PlayerLogout":
                        var logoutParam = jPayload.ToObject<PlayerLogoutPayload>(_payloadDeserializer);
                        await Process(logoutParam);
                        break;

                    case "GainExperience":
                        var experienceParam = jPayload.ToObject<GainExperiencePayload>(_payloadDeserializer);
                        await Process(experienceParam);
                        break;

                    case "FacilityControl":
                        var controlParam = jPayload.ToObject<FacilityControlPayload>(_payloadDeserializer);
                        await Process(controlParam);
                        break;

                    case "VehicleDestroy":
                        var vehicleDestroyParam = jPayload.ToObject<VehicleDestroyPayload>(_payloadDeserializer);
                        await Process(vehicleDestroyParam);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(75642, ex, "Failed to process websocket event: {0}. {1}", eventName, jPayload.ToString());
            }
        }

        #region Payload Handling

        #region Death Payload
        [CensusEventHandler("Death", typeof(DeathPayload))]
        private async Task<ScrimDeathActionEvent> Process(DeathPayload payload)
        {
            if (!await _deathFilter.TryFilterNewPayload(payload, p => p.Timestamp.ToString("s")))
            {
                _logger.LogWarning("Duplicate Death payload detected, excluded");
                return null;
            }

            string attackerId = payload.AttackerCharacterId;
            string victimId = payload.CharacterId;

            bool isValidAttackerId = (attackerId != null && attackerId.Length > 18);
            bool isValidVictimId = (victimId != null && victimId.Length > 18);

            Player attackerPlayer;
            Player victimPlayer;

            bool involvesBenchedPlayer = false;

            ScrimDeathActionEvent deathEvent = new ScrimDeathActionEvent
            {
                Timestamp = payload.Timestamp,
                ZoneId = payload.ZoneId,
                IsHeadshot = payload.IsHeadshot
            };

            var weaponItem = await _itemService.GetWeaponItemAsync((int)payload.AttackerWeaponId);
            if (weaponItem != null)
            {
                deathEvent.Weapon = new ScrimActionWeaponInfo()
                {
                    Id = weaponItem.Id,
                    ItemCategoryId = weaponItem.ItemCategoryId,
                    Name = weaponItem.Name,
                    IsVehicleWeapon = weaponItem.IsVehicleWeapon
                };
            }
            else if (payload.AttackerWeaponId != null)
            {
                deathEvent.Weapon = new ScrimActionWeaponInfo()
                {
                    Id = (int)payload.AttackerWeaponId
                };
            }


            try
            {
                if (isValidAttackerId == true)
                {
                    deathEvent.AttackerCharacterId = attackerId;
                    deathEvent.AttackerLoadoutId = payload.AttackerLoadoutId;

                    attackerPlayer = _teamsManager.GetPlayerFromId(attackerId);
                    deathEvent.AttackerPlayer = attackerPlayer;

                    if (attackerPlayer != null)
                    {
                        _teamsManager.SetPlayerLoadoutId(attackerId, deathEvent.AttackerLoadoutId);

                        involvesBenchedPlayer = involvesBenchedPlayer || attackerPlayer.IsBenched;
                    }
                }

                if (isValidVictimId == true)
                {
                    deathEvent.VictimCharacterId = victimId;
                    deathEvent.VictimLoadoutId = payload.CharacterLoadoutId;

                    victimPlayer = _teamsManager.GetPlayerFromId(victimId);
                    deathEvent.VictimPlayer = victimPlayer;

                    if (victimPlayer != null)
                    {
                        _teamsManager.SetPlayerLoadoutId(victimId, deathEvent.VictimLoadoutId);

                        involvesBenchedPlayer = involvesBenchedPlayer || victimPlayer.IsBenched;
                    }
                }

                deathEvent.ActionType = GetDeathScrimActionType(deathEvent);

                if (deathEvent.ActionType != ScrimActionType.OutsideInterference)
                {
                    deathEvent.DeathType = GetDeathEventType(deathEvent.ActionType);

                    if (deathEvent.DeathType == DeathEventType.Suicide)
                    {
                        deathEvent.AttackerPlayer = deathEvent.VictimPlayer;
                        deathEvent.AttackerCharacterId = deathEvent.VictimCharacterId;
                        deathEvent.AttackerLoadoutId = deathEvent.VictimLoadoutId;
                    }

                    if (_isScoringEnabled && !involvesBenchedPlayer)
                    {
                        var scoringResult = await _scorer.ScoreDeathEvent(deathEvent);
                        deathEvent.Points = scoringResult.Points;
                        deathEvent.IsBanned = scoringResult.IsBanned;

                        var currentMatchId = _scrimMatchService.CurrentMatchId;
                        var currentRound = _scrimMatchService.CurrentMatchRound;

                        if (_isEventStoringEnabled && !string.IsNullOrWhiteSpace(currentMatchId))
                        {
                            var dataModel = new ScrimDeath
                            {
                                ScrimMatchId = currentMatchId,
                                Timestamp = deathEvent.Timestamp,
                                AttackerCharacterId = deathEvent.AttackerPlayer.Id,
                                VictimCharacterId = deathEvent.VictimPlayer.Id,
                                ScrimMatchRound = currentRound,
                                ActionType = deathEvent.ActionType,
                                DeathType = deathEvent.DeathType,
                                ZoneId = (int)deathEvent.ZoneId,
                                WorldId = payload.WorldId,
                                AttackerTeamOrdinal = deathEvent.AttackerPlayer.TeamOrdinal,
                                AttackerFactionId = deathEvent.AttackerPlayer.FactionId,
                                AttackerNameFull = deathEvent.AttackerPlayer.NameFull,
                                AttackerLoadoutId = deathEvent.AttackerPlayer.LoadoutId,
                                AttackerOutfitId = deathEvent.AttackerPlayer.IsOutfitless ? null : deathEvent.AttackerPlayer.OutfitId,
                                AttackerOutfitAlias = deathEvent.AttackerPlayer.IsOutfitless ? null : deathEvent.AttackerPlayer.OutfitAlias,
                                AttackerIsOutfitless = deathEvent.AttackerPlayer.IsOutfitless,
                                VictimTeamOrdinal = deathEvent.VictimPlayer.TeamOrdinal,
                                VictimFactionId = deathEvent.VictimPlayer.FactionId,
                                VictimNameFull = deathEvent.VictimPlayer.NameFull,
                                VictimLoadoutId = deathEvent.VictimPlayer.LoadoutId,
                                VictimOutfitId = deathEvent.VictimPlayer.IsOutfitless ? null : deathEvent.VictimPlayer.OutfitId,
                                VictimOutfitAlias = deathEvent.VictimPlayer.IsOutfitless ? null : deathEvent.VictimPlayer.OutfitAlias,
                                VictimIsOutfitless = deathEvent.VictimPlayer.IsOutfitless,
                                WeaponId = deathEvent.Weapon?.Id,
                                WeaponItemCategoryId = deathEvent.Weapon?.ItemCategoryId,
                                IsVehicleWeapon = deathEvent.Weapon?.IsVehicleWeapon,
                                AttackerVehicleId = deathEvent.AttackerVehicleId,
                                IsHeadshot = deathEvent.IsHeadshot,
                                Points = deathEvent.Points,
                                //AttackerResultingPoints = deathEvent.AttackerPlayer.EventAggregate.Points,
                                //AttackerResultingNetScore = deathEvent.AttackerPlayer.EventAggregate.NetScore,
                                //VictimResultingPoints = deathEvent.VictimPlayer.EventAggregate.Points,
                                //VictimResultingNetScore = deathEvent.VictimPlayer.EventAggregate.NetScore
                            };

                            using var factory = _dbContextHelper.GetFactory();
                            var dbContext = factory.GetDbContext();

                            dbContext.ScrimDeaths.Add(dataModel);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }

                _messageService.BroadcastScrimDeathActionEventMessage(new ScrimDeathActionEventMessage(deathEvent));

                return deathEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        private ScrimActionType GetDeathScrimActionType(ScrimDeathActionEvent death)
        {
            // Determine if this is involves a non-tracked player
            if ((death.AttackerPlayer == null && !string.IsNullOrWhiteSpace(death.AttackerCharacterId))
                    || (death.VictimPlayer == null && !string.IsNullOrWhiteSpace(death.VictimCharacterId)))
            {
                return ScrimActionType.OutsideInterference;
            }

            var attackerIsVehicle = (death.Weapon != null && death.Weapon.IsVehicleWeapon);

            var attackerIsMax = death.AttackerLoadoutId == null
                                    ? false
                                    : ProfileService.IsMaxLoadoutId(death.AttackerLoadoutId);

            var victimIsMax = death.VictimLoadoutId == null
                                    ? false
                                    : ProfileService.IsMaxLoadoutId(death.VictimLoadoutId);

            var sameTeam = _teamsManager.DoPlayersShareTeam(death.AttackerPlayer, death.VictimPlayer);
            var samePlayer = (death.AttackerPlayer == death.VictimPlayer || death.AttackerPlayer == null);

            if (samePlayer)
            {
                return victimIsMax
                            ? ScrimActionType.MaxSuicide
                            : ScrimActionType.InfantrySuicide;
            }
            else if (sameTeam)
            {
                if (attackerIsVehicle)
                {
                    return victimIsMax
                                ? ScrimActionType.VehicleTeamkillMax
                                : ScrimActionType.VehicleTeamkillInfantry;
                }
                else if (attackerIsMax)
                {
                    return victimIsMax
                                ? ScrimActionType.MaxTeamkillMax
                                : ScrimActionType.MaxTeamkillInfantry;
                }
                else
                {
                    return victimIsMax
                                ? ScrimActionType.InfantryTeamkillMax
                                : ScrimActionType.InfantryTeamkillInfantry;
                }
            }
            else
            {
                if (attackerIsVehicle)
                {
                    return victimIsMax
                                ? ScrimActionType.VehicleKillMax
                                : ScrimActionType.VehicleKillInfantry;
                }
                else if (attackerIsMax)
                {
                    return victimIsMax
                                ? ScrimActionType.MaxKillMax
                                : ScrimActionType.MaxKillInfantry;
                }
                else
                {
                    return victimIsMax
                                ? ScrimActionType.InfantryKillMax
                                : ScrimActionType.InfantryKillInfantry;
                }
            }
        }

        private DeathEventType GetDeathEventType(ScrimActionType scrimActionType)
        {
            return scrimActionType switch
            {
                ScrimActionType.MaxSuicide => DeathEventType.Suicide,
                ScrimActionType.InfantrySuicide => DeathEventType.Suicide,
                ScrimActionType.MaxTeamkillMax => DeathEventType.Teamkill,
                ScrimActionType.MaxTeamkillInfantry => DeathEventType.Teamkill,
                ScrimActionType.InfantryTeamkillMax => DeathEventType.Teamkill,
                ScrimActionType.InfantryTeamkillInfantry => DeathEventType.Teamkill,
                ScrimActionType.VehicleTeamkillMax => DeathEventType.Teamkill,
                ScrimActionType.VehicleTeamkillInfantry => DeathEventType.Teamkill,
                ScrimActionType.MaxKillMax => DeathEventType.Kill,
                ScrimActionType.MaxKillInfantry => DeathEventType.Kill,
                ScrimActionType.InfantryKillMax => DeathEventType.Kill,
                ScrimActionType.InfantryKillInfantry => DeathEventType.Kill,
                ScrimActionType.VehicleKillMax => DeathEventType.Kill,
                ScrimActionType.VehicleKillInfantry => DeathEventType.Kill,
                _ => DeathEventType.Kill
            };
        }
        #endregion

        #region Vehicle Destroy Payloads
        [CensusEventHandler("VehicleDestroy", typeof(VehicleDestroyPayload))]
        private async Task<ScrimVehicleDestructionActionEvent> Process(VehicleDestroyPayload payload)
        {
            if (!await _vehicleDestroyFilter.TryFilterNewPayload(payload, p => p.Timestamp.ToString("s")))
            {
                _logger.LogWarning("Duplicate Vehicle Destroy payload detected, excluded");
                return null;
            }

            string attackerId = payload.AttackerCharacterId;
            string victimId = payload.CharacterId;

            bool isValidAttackerId = (attackerId != null && attackerId.Length > 18);
            bool isValidVictimId = (victimId != null && victimId.Length > 18);

            Player attackerPlayer;
            Player victimPlayer;

            bool involvesBenchedPlayer = false;

            // Don't bother tracking players destroying unclaimed vehicles
            if (!isValidVictimId)
            {
                return null;
            }

            ScrimVehicleDestructionActionEvent destructionEvent = new ScrimVehicleDestructionActionEvent
            {
                Timestamp = payload.Timestamp,
                ZoneId = payload.ZoneId,
            };

            var weaponItem = await _itemService.GetWeaponItemAsync((int)payload.AttackerWeaponId);
            if (weaponItem != null)
            {
                destructionEvent.Weapon = new ScrimActionWeaponInfo
                {
                    Id = weaponItem.Id,
                    ItemCategoryId = weaponItem.ItemCategoryId,
                    Name = weaponItem.Name,
                    IsVehicleWeapon = weaponItem.IsVehicleWeapon
                };
            }
            else if (payload.AttackerWeaponId != null)
            {
                destructionEvent.Weapon = new ScrimActionWeaponInfo()
                {
                    Id = (int)payload.AttackerWeaponId
                };
            }

            var attackerVehicle = await _vehicleService.GetVehicleInfoAsync((int)payload.AttackerVehicleId);
            if (attackerVehicle != null)
            {
                destructionEvent.AttackerVehicle = new ScrimActionVehicleInfo(attackerVehicle);
            }

            var victimVehicle = await _vehicleService.GetVehicleInfoAsync(payload.VehicleId);
            if (victimVehicle != null)
            {
                destructionEvent.VictimVehicle = new ScrimActionVehicleInfo(victimVehicle);
            }

            try
            {
                if (isValidAttackerId == true)
                {
                    destructionEvent.AttackerCharacterId = attackerId;
                    destructionEvent.AttackerLoadoutId = payload.AttackerLoadoutId;

                    attackerPlayer = _teamsManager.GetPlayerFromId(attackerId);
                    destructionEvent.AttackerPlayer = attackerPlayer;

                    if (attackerPlayer != null)
                    {
                        _teamsManager.SetPlayerLoadoutId(attackerId, destructionEvent.AttackerLoadoutId);

                        involvesBenchedPlayer = involvesBenchedPlayer || attackerPlayer.IsBenched;
                    }
                }

                if (isValidVictimId == true)
                {
                    destructionEvent.VictimCharacterId = victimId;

                    victimPlayer = _teamsManager.GetPlayerFromId(victimId);
                    destructionEvent.VictimPlayer = victimPlayer;

                    if (victimPlayer != null)
                    {
                        involvesBenchedPlayer = involvesBenchedPlayer || victimPlayer.IsBenched;
                    }
                }

                destructionEvent.DeathType = GetVehicleDestructionDeathType(destructionEvent);

                destructionEvent.ActionType = GetVehicleDestructionScrimActionType(destructionEvent);


                if (destructionEvent.ActionType != ScrimActionType.OutsideInterference)
                {
                    if (destructionEvent.DeathType == DeathEventType.Suicide)
                    {
                        destructionEvent.AttackerPlayer = destructionEvent.VictimPlayer;
                        destructionEvent.AttackerCharacterId = destructionEvent.VictimCharacterId;
                        destructionEvent.AttackerVehicle = destructionEvent.VictimVehicle;
                    }

                    if (_isScoringEnabled && !involvesBenchedPlayer)
                    {
                        var scoringResult = await _scorer.ScoreVehicleDestructionEvent(destructionEvent);
                        destructionEvent.Points = scoringResult.Points;
                        destructionEvent.IsBanned = scoringResult.IsBanned;

                        var currentMatchId = _scrimMatchService.CurrentMatchId;
                        var currentRound = _scrimMatchService.CurrentMatchRound;

                        if (_isEventStoringEnabled && !string.IsNullOrWhiteSpace(currentMatchId))
                        {
                            var dataModel = new ScrimVehicleDestruction
                            {
                                ScrimMatchId = currentMatchId,
                                Timestamp = destructionEvent.Timestamp,
                                AttackerCharacterId = destructionEvent.AttackerPlayer.Id,
                                VictimCharacterId = destructionEvent.VictimPlayer.Id,
                                VictimVehicleId = destructionEvent.VictimVehicle != null ? destructionEvent.VictimVehicle.Id : payload.VehicleId,
                                AttackerVehicleId = destructionEvent.AttackerVehicle?.Id,
                                ScrimMatchRound = currentRound,
                                ActionType = destructionEvent.ActionType,
                                DeathType = destructionEvent.DeathType,
                                AttackerVehicleClass = destructionEvent.AttackerVehicle?.Type,
                                VictimVehicleClass = destructionEvent.VictimVehicle?.Type,
                                AttackerTeamOrdinal = destructionEvent.AttackerPlayer.TeamOrdinal,
                                VictimTeamOrdinal = destructionEvent.VictimPlayer.TeamOrdinal,
                                AttackerFactionId = destructionEvent.AttackerPlayer.FactionId,
                                AttackerNameFull = destructionEvent.AttackerPlayer.NameFull,
                                AttackerLoadoutId = destructionEvent.AttackerPlayer?.LoadoutId,
                                AttackerOutfitId = destructionEvent.AttackerPlayer.IsOutfitless ? null : destructionEvent.AttackerPlayer.OutfitId,
                                AttackerOutfitAlias = destructionEvent.AttackerPlayer.IsOutfitless ? null : destructionEvent.AttackerPlayer.OutfitAlias,
                                AttackerIsOutfitless = destructionEvent.AttackerPlayer.IsOutfitless,
                                VictimFactionId = destructionEvent.VictimPlayer.FactionId,
                                VictimNameFull = destructionEvent.VictimPlayer.NameFull,
                                VictimLoadoutId = destructionEvent.VictimPlayer?.LoadoutId,
                                VictimOutfitId = destructionEvent.VictimPlayer.IsOutfitless ? null : destructionEvent.VictimPlayer.OutfitId,
                                VictimOutfitAlias = destructionEvent.VictimPlayer.IsOutfitless ? null : destructionEvent.VictimPlayer.OutfitAlias,
                                VictimIsOutfitless = destructionEvent.VictimPlayer.IsOutfitless,
                                WeaponId = destructionEvent.Weapon?.Id,
                                WeaponItemCategoryId = destructionEvent.Weapon?.ItemCategoryId,
                                IsVehicleWeapon = destructionEvent.Weapon?.IsVehicleWeapon,
                                ZoneId = (int)destructionEvent.ZoneId,
                                WorldId = payload.WorldId,
                                Points = destructionEvent.Points,
                                //AttackerResultingPoints = destructionEvent.AttackerPlayer.EventAggregate.Points,
                                //AttackerResultingNetScore = destructionEvent.AttackerPlayer.EventAggregate.NetScore,
                                //VictimResultingPoints = destructionEvent.VictimPlayer.EventAggregate.Points,
                                //VictimResultingNetScore = destructionEvent.VictimPlayer.EventAggregate.NetScore
                            };

                            using var factory = _dbContextHelper.GetFactory();
                            var dbContext = factory.GetDbContext();

                            dbContext.ScrimVehicleDestructions.Add(dataModel);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }

                _messageService.BroadcastScrimVehicleDestructionActionEventMessage(new ScrimVehicleDestructionActionEventMessage(destructionEvent));

                return destructionEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        private DeathEventType GetVehicleDestructionDeathType(ScrimVehicleDestructionActionEvent destruction)
        {
            var sameTeam = _teamsManager.DoPlayersShareTeam(destruction.AttackerPlayer, destruction.VictimPlayer);
            var samePlayer = (destruction.AttackerPlayer == destruction.VictimPlayer || destruction.AttackerPlayer == null);

            if (samePlayer)
            {
                return DeathEventType.Suicide;
            }
            else if (sameTeam)
            {
                return DeathEventType.Teamkill;
            }
            else
            {
                return DeathEventType.Kill;
            }
        }

        private ScrimActionType GetVehicleDestructionScrimActionType(ScrimVehicleDestructionActionEvent destruction)
        {

            // TODO: determine what a bailed-then-crashed undamaged vehicle looks like
            // Determine if this is involves a non-tracked player
            if ((destruction.AttackerPlayer == null && !string.IsNullOrWhiteSpace(destruction.AttackerCharacterId))
                    || (destruction.VictimPlayer == null && !string.IsNullOrWhiteSpace(destruction.VictimCharacterId)))
            {
                return ScrimActionType.OutsideInterference;
            }

            var attackerIsVehicle = ((destruction.Weapon != null && destruction.Weapon.IsVehicleWeapon) || (destruction.AttackerVehicle != null && destruction.AttackerVehicle.Type != VehicleType.Unknown));

            var attackerIsMax = destruction.AttackerLoadoutId == null
                                                ? false
                                                : ProfileService.IsMaxLoadoutId(destruction.AttackerLoadoutId);

            if (destruction.DeathType == DeathEventType.Suicide)
            {
                return destruction.VictimVehicle.Type switch
                {
                    VehicleType.Interceptor => ScrimActionType.InterceptorSuicide,
                    VehicleType.ESF => ScrimActionType.EsfSuicide,
                    VehicleType.Valkyrie => ScrimActionType.ValkyrieSuicide,
                    VehicleType.Liberator => ScrimActionType.LiberatorSuicide,
                    VehicleType.Galaxy => ScrimActionType.GalaxySuicide,
                    VehicleType.Bastion => ScrimActionType.BastionSuicide,

                    VehicleType.Flash => ScrimActionType.FlashSuicide,
                    VehicleType.Harasser => ScrimActionType.HarasserSuicide,
                    VehicleType.ANT => ScrimActionType.AntSuicide,
                    VehicleType.Sunderer => ScrimActionType.SundererSuicide,
                    VehicleType.Lightning => ScrimActionType.LightningSuicide,
                    VehicleType.MBT => ScrimActionType.MbtSuicide,

                    _ => ScrimActionType.Unknown,
                };
            }
            else if (destruction.DeathType == DeathEventType.Teamkill)
            {
                if (attackerIsVehicle)
                {
                    return destruction.VictimVehicle.Type switch
                    {
                        VehicleType.Interceptor => ScrimActionType.VehicleTeamDestroyInterceptor,
                        VehicleType.ESF => ScrimActionType.VehicleTeamDestroyEsf,
                        VehicleType.Valkyrie => ScrimActionType.VehicleTeamDestroyValkyrie,
                        VehicleType.Liberator => ScrimActionType.VehicleTeamDestroyLiberator,
                        VehicleType.Galaxy => ScrimActionType.VehicleTeamDestroyGalaxy,
                        VehicleType.Bastion => ScrimActionType.VehicleTeamDestroyBastion,

                        VehicleType.Flash => ScrimActionType.VehicleTeamDestroyFlash,
                        VehicleType.Harasser => ScrimActionType.VehicleTeamDestroyHarasser,
                        VehicleType.ANT => ScrimActionType.VehicleTeamDestroyAnt,
                        VehicleType.Sunderer => ScrimActionType.VehicleTeamDestroySunderer,
                        VehicleType.Lightning => ScrimActionType.VehicleTeamDestroyLightning,
                        VehicleType.MBT => ScrimActionType.VehicleTeamDestroyMbt,

                        _ => ScrimActionType.Unknown,
                    };
                }
                else if (attackerIsMax)
                {
                    return destruction.VictimVehicle.Type switch
                    {
                        VehicleType.Interceptor => ScrimActionType.MaxTeamDestroyInterceptor,
                        VehicleType.ESF => ScrimActionType.MaxTeamDestroyEsf,
                        VehicleType.Valkyrie => ScrimActionType.MaxTeamDestroyValkyrie,
                        VehicleType.Liberator => ScrimActionType.MaxTeamDestroyLiberator,
                        VehicleType.Galaxy => ScrimActionType.MaxTeamDestroyGalaxy,
                        VehicleType.Bastion => ScrimActionType.MaxTeamDestroyBastion,

                        VehicleType.Flash => ScrimActionType.MaxTeamDestroyFlash,
                        VehicleType.Harasser => ScrimActionType.MaxTeamDestroyHarasser,
                        VehicleType.ANT => ScrimActionType.MaxTeamDestroyAnt,
                        VehicleType.Sunderer => ScrimActionType.MaxTeamDestroySunderer,
                        VehicleType.Lightning => ScrimActionType.MaxTeamDestroyLightning,
                        VehicleType.MBT => ScrimActionType.MaxTeamDestroyMbt,

                        _ => ScrimActionType.Unknown,
                    };
                }
                else
                {
                    return destruction.VictimVehicle.Type switch
                    {
                        VehicleType.Interceptor => ScrimActionType.InfantryTeamDestroyInterceptor,
                        VehicleType.ESF => ScrimActionType.InfantryTeamDestroyEsf,
                        VehicleType.Valkyrie => ScrimActionType.InfantryTeamDestroyValkyrie,
                        VehicleType.Liberator => ScrimActionType.InfantryTeamDestroyLiberator,
                        VehicleType.Galaxy => ScrimActionType.InfantryTeamDestroyGalaxy,
                        VehicleType.Bastion => ScrimActionType.InfantryTeamDestroyBastion,

                        VehicleType.Flash => ScrimActionType.InfantryTeamDestroyFlash,
                        VehicleType.Harasser => ScrimActionType.InfantryTeamDestroyHarasser,
                        VehicleType.ANT => ScrimActionType.InfantryTeamDestroyAnt,
                        VehicleType.Sunderer => ScrimActionType.InfantryTeamDestroySunderer,
                        VehicleType.Lightning => ScrimActionType.InfantryTeamDestroyLightning,
                        VehicleType.MBT => ScrimActionType.InfantryTeamDestroyMbt,

                        _ => ScrimActionType.Unknown,
                    };
                }
            }
            else if (destruction.DeathType == DeathEventType.Kill)
            {
                if (attackerIsVehicle)
                {
                    return destruction.VictimVehicle.Type switch
                    {
                        VehicleType.Interceptor => ScrimActionType.VehicleDestroyInterceptor,
                        VehicleType.ESF => ScrimActionType.VehicleDestroyEsf,
                        VehicleType.Valkyrie => ScrimActionType.VehicleDestroyValkyrie,
                        VehicleType.Liberator => ScrimActionType.VehicleDestroyLiberator,
                        VehicleType.Galaxy => ScrimActionType.VehicleDestroyGalaxy,
                        VehicleType.Bastion => ScrimActionType.VehicleDestroyBastion,

                        VehicleType.Flash => ScrimActionType.VehicleDestroyFlash,
                        VehicleType.Harasser => ScrimActionType.VehicleDestroyHarasser,
                        VehicleType.ANT => ScrimActionType.VehicleDestroyAnt,
                        VehicleType.Sunderer => ScrimActionType.VehicleDestroySunderer,
                        VehicleType.Lightning => ScrimActionType.VehicleDestroyLightning,
                        VehicleType.MBT => ScrimActionType.VehicleDestroyMbt,

                        _ => ScrimActionType.Unknown,
                    };
                }
                else if (attackerIsMax)
                {
                    return destruction.VictimVehicle.Type switch
                    {
                        VehicleType.Interceptor => ScrimActionType.MaxDestroyInterceptor,
                        VehicleType.ESF => ScrimActionType.MaxDestroyEsf,
                        VehicleType.Valkyrie => ScrimActionType.MaxDestroyValkyrie,
                        VehicleType.Liberator => ScrimActionType.MaxDestroyLiberator,
                        VehicleType.Galaxy => ScrimActionType.MaxDestroyGalaxy,
                        VehicleType.Bastion => ScrimActionType.MaxDestroyBastion,

                        VehicleType.Flash => ScrimActionType.MaxDestroyFlash,
                        VehicleType.Harasser => ScrimActionType.MaxDestroyHarasser,
                        VehicleType.ANT => ScrimActionType.MaxDestroyAnt,
                        VehicleType.Sunderer => ScrimActionType.MaxDestroySunderer,
                        VehicleType.Lightning => ScrimActionType.MaxDestroyLightning,
                        VehicleType.MBT => ScrimActionType.MaxDestroyMbt,

                        _ => ScrimActionType.Unknown,
                    };
                }
                else
                {
                    return destruction.VictimVehicle.Type switch
                    {
                        VehicleType.Interceptor => ScrimActionType.InfantryDestroyInterceptor,
                        VehicleType.ESF => ScrimActionType.InfantryDestroyEsf,
                        VehicleType.Valkyrie => ScrimActionType.InfantryDestroyValkyrie,
                        VehicleType.Liberator => ScrimActionType.InfantryDestroyLiberator,
                        VehicleType.Galaxy => ScrimActionType.InfantryDestroyGalaxy,
                        VehicleType.Bastion => ScrimActionType.InfantryDestroyBastion,

                        VehicleType.Flash => ScrimActionType.InfantryDestroyFlash,
                        VehicleType.Harasser => ScrimActionType.InfantryDestroyHarasser,
                        VehicleType.ANT => ScrimActionType.InfantryDestroyAnt,
                        VehicleType.Sunderer => ScrimActionType.InfantryDestroySunderer,
                        VehicleType.Lightning => ScrimActionType.InfantryDestroyLightning,
                        VehicleType.MBT => ScrimActionType.InfantryDestroyMbt,

                        _ => ScrimActionType.Unknown,
                    };
                }
            }
            else
            {
                return ScrimActionType.Unknown;
            }
        }

        #endregion Vehicle Destroy Payloads

        #region Login / Logout Payloads
        [CensusEventHandler("PlayerLogin", typeof(PlayerLoginPayload))]
        private async Task<PlayerLogin> Process(PlayerLoginPayload payload)
        {
            if (!await _loginFilter.TryFilterNewPayload(payload, p => p.Timestamp.ToString("s")))
            {
                _logger.LogWarning("Duplicate Player Login payload detected, excluded");
                return null;
            }

            var characterId = payload.CharacterId;

            var player = _teamsManager.GetPlayerFromId(characterId);

            // TODO: use ScrimActionLoginEvent instead of PlayerLogin

            var dataModel = new PlayerLogin
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };

            _scorer.HandlePlayerLogin(dataModel);

            _messageService.BroadcastPlayerLoginMessage(new PlayerLoginMessage(player, dataModel));

            return dataModel;
        }

        [CensusEventHandler("PlayerLogout", typeof(PlayerLogoutPayload))]
        private async Task<PlayerLogout> Process(PlayerLogoutPayload payload)
        {
            if (!await _logoutFilter.TryFilterNewPayload(payload, p => p.Timestamp.ToString("s")))
            {
                _logger.LogWarning("Duplicate Player Logout payload detected, excluded");
                return null;
            }

            var characterId = payload.CharacterId;

            var player = _teamsManager.GetPlayerFromId(characterId);

            // TODO: use ScrimActionLogoutEvent instead of PlayerLogout

            var dataModel = new PlayerLogout
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };

            _scorer.HandlePlayerLogout(dataModel);

            _messageService.BroadcastPlayerLogoutMessage(new PlayerLogoutMessage(player, dataModel));

            return dataModel;
        }
        #endregion

        #region GainExperience Payloads
        [CensusEventHandler("GainExperience", typeof(GainExperiencePayload))]
        private async Task Process(GainExperiencePayload payload)
        {
            if (!await _experienceFilter.TryFilterNewPayload(payload, p => p.Timestamp.ToString("s")))
            {
                _logger.LogWarning("Duplicate Gain Experience payload detected, excluded");
                return;
            }

            var experienceId = payload.ExperienceId;
            var experienceType = ExperienceEventsBuilder.GetExperienceTypeFromId(experienceId);

            var baseEvent = new ScrimExperienceGainActionEvent
            {
                Timestamp = payload.Timestamp,
                ZoneId = payload.ZoneId,

                ExperienceType = experienceType,
                ExperienceGainInfo = new ScrimActionExperienceGainInfo
                {
                    Id = experienceId,
                    Amount = payload.Amount
                },
                LoadoutId = payload.LoadoutId
            };

            try
            {
                switch (experienceType)
                {
                    case ExperienceType.Revive:
                        await ProcessRevivePayload(baseEvent, payload);
                        return;

                    case ExperienceType.DamageAssist:
                        await ProcessAssistPayload(baseEvent, payload);
                        return;

                    case ExperienceType.UtilityAssist:
                        //ProcessAssistPayload(baseEvent, payload);
                        return;

                    case ExperienceType.PointControl:
                        await ProcessPointControlPayload(baseEvent, payload);
                        return;

                    case ExperienceType.GrenadeAssist:
                        await ProcessAssistPayload(baseEvent, payload);
                        return;

                    case ExperienceType.HealSupportAssist:
                        await ProcessAssistPayload(baseEvent, payload);
                        return;

                    case ExperienceType.ProtectAlliesAssist:
                        await ProcessAssistPayload(baseEvent, payload);
                        return;

                    case ExperienceType.SpotAssist:
                        await ProcessAssistPayload(baseEvent, payload);
                        return;

                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task ProcessRevivePayload(ScrimExperienceGainActionEvent baseEvent, GainExperiencePayload payload)
        {
            var reviveEvent = new ScrimReviveActionEvent(baseEvent);

            string medicId = payload.CharacterId;
            string revivedId = payload.OtherId;

            bool isValidMedicId = (medicId != null && medicId.Length > 18);
            bool isValidRevivedId = (revivedId != null && revivedId.Length > 18);

            Player medicPlayer;
            Player revivedPlayer;
            Player lastKilledByPlayer;


            bool involvesBenchedPlayer = false;

            if (isValidMedicId == true)
            {
                reviveEvent.MedicCharacterId = medicId;

                medicPlayer = _teamsManager.GetPlayerFromId(medicId);
                reviveEvent.MedicPlayer = medicPlayer;

                if (medicPlayer != null)
                {
                    _teamsManager.SetPlayerLoadoutId(medicId, reviveEvent.LoadoutId);

                    involvesBenchedPlayer = involvesBenchedPlayer || medicPlayer.IsBenched;
                }
            }

            if (isValidRevivedId == true)
            {
                reviveEvent.RevivedCharacterId = revivedId;

                revivedPlayer = _teamsManager.GetPlayerFromId(revivedId);
                reviveEvent.RevivedPlayer = revivedPlayer;

                if (revivedPlayer != null)
                {
                    involvesBenchedPlayer = involvesBenchedPlayer || revivedPlayer.IsBenched;

                    lastKilledByPlayer = _teamsManager.GetLastKilledByPlayer(revivedId);
                    if (lastKilledByPlayer != null)
                    {
                        reviveEvent.LastKilledByCharacterId = lastKilledByPlayer.Id;
                        reviveEvent.LastKilledByPlayer = lastKilledByPlayer;
                    }
                }
            }

            reviveEvent.ActionType = GetReviveScrimActionType(reviveEvent);

            if (reviveEvent.ActionType != ScrimActionType.OutsideInterference)
            {
                if (_isScoringEnabled && !involvesBenchedPlayer)
                {
                    var scoringResult = await _scorer.ScoreReviveEvent(reviveEvent);
                    reviveEvent.Points = scoringResult.Result.Points;
                    reviveEvent.IsBanned = scoringResult.Result.IsBanned;

                    reviveEvent.EnemyPoints = scoringResult.EnemyResult.Points;
                    reviveEvent.EnemyActionType = scoringResult.EnemyActionType;

                    // TODO: remove these two redundant sets
                    //reviveEvent.LastKilledByPlayer = scoringResult.LastKilledByPlayer;
                    //reviveEvent.LastKilledByCharacterId = scoringResult.LastKilledByPlayer?.Id;

                    var currentMatchId = _scrimMatchService.CurrentMatchId;
                    var currentRound = _scrimMatchService.CurrentMatchRound;

                    if (_isEventStoringEnabled && !string.IsNullOrWhiteSpace(currentMatchId))
                    {
                        var dataModel = new ScrimRevive
                        {
                            ScrimMatchId = currentMatchId,
                            Timestamp = reviveEvent.Timestamp,
                            MedicCharacterId = reviveEvent.MedicPlayer.Id,
                            RevivedCharacterId = reviveEvent.RevivedPlayer.Id,
                            ScrimMatchRound = currentRound,
                            ActionType = reviveEvent.ActionType,
                            MedicTeamOrdinal = reviveEvent.MedicPlayer.TeamOrdinal,
                            RevivedTeamOrdinal = reviveEvent.RevivedPlayer.TeamOrdinal,
                            MedicLoadoutId = reviveEvent.MedicPlayer?.LoadoutId != null ? reviveEvent.MedicPlayer.LoadoutId : null,
                            RevivedLoadoutId = reviveEvent.RevivedPlayer?.LoadoutId != null ? reviveEvent.RevivedPlayer.LoadoutId : null,
                            ExperienceGainId = reviveEvent.ExperienceGainInfo.Id,
                            ExperienceGainAmount = reviveEvent.ExperienceGainInfo.Amount,
                            ZoneId = (int)reviveEvent.ZoneId,
                            WorldId = payload.WorldId,
                            Points = reviveEvent.Points,
                            EnemyPoints = reviveEvent.EnemyPoints,
                            EnemyActionType = reviveEvent.EnemyActionType,
                            LastKilledByCharacterId = (reviveEvent.LastKilledByPlayer?.Id != null) ? reviveEvent.LastKilledByPlayer.Id : null
                        };

                        using var factory = _dbContextHelper.GetFactory();
                        var dbContext = factory.GetDbContext();

                        dbContext.ScrimRevives.Add(dataModel);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }

            _messageService.BroadcastScrimReviveActionEventMessage(new ScrimReviveActionEventMessage(reviveEvent));
        }

        private ScrimActionType GetReviveScrimActionType(ScrimReviveActionEvent reviveEvent)
        {
            // Determine if this is involves a non-tracked player
            if ((reviveEvent.MedicPlayer == null && !string.IsNullOrWhiteSpace(reviveEvent.MedicCharacterId))
                    || (reviveEvent.RevivedPlayer == null && !string.IsNullOrWhiteSpace(reviveEvent.RevivedCharacterId))
                    || (reviveEvent.LastKilledByPlayer == null))
            {
                return ScrimActionType.OutsideInterference;
            }

            bool isRevivedMax = ProfileService.IsMaxLoadoutId(reviveEvent.RevivedPlayer.LoadoutId);

            return isRevivedMax
                        ? ScrimActionType.ReviveMax
                        : ScrimActionType.ReviveInfantry;
        }

        private async Task ProcessAssistPayload(ScrimExperienceGainActionEvent baseEvent, GainExperiencePayload payload)
        {
            var assistEvent = new ScrimAssistActionEvent(baseEvent);

            string attackerId = payload.CharacterId;
            string victimId = payload.OtherId;

            bool isValidattackerId = (attackerId != null && attackerId.Length > 18);
            bool isValidvictimId = (victimId != null && victimId.Length > 18);

            Player attackerPlayer;
            Player victimPlayer;

            bool involvesBenchedPlayer = false;

            if (isValidattackerId == true)
            {
                assistEvent.AttackerCharacterId = attackerId;

                attackerPlayer = _teamsManager.GetPlayerFromId(attackerId);
                assistEvent.AttackerPlayer = attackerPlayer;

                if (attackerPlayer != null)
                {
                    _teamsManager.SetPlayerLoadoutId(attackerId, assistEvent.LoadoutId);

                    involvesBenchedPlayer = involvesBenchedPlayer || attackerPlayer.IsBenched;
                }
            }

            if (isValidvictimId == true)
            {
                assistEvent.VictimCharacterId = victimId;

                victimPlayer = _teamsManager.GetPlayerFromId(victimId);
                assistEvent.VictimPlayer = victimPlayer;

                if (victimPlayer != null)
                {
                    involvesBenchedPlayer = involvesBenchedPlayer || victimPlayer.IsBenched;
                }
            }

            assistEvent.ActionType = GetAssistScrimActionType(assistEvent);

            if (assistEvent.ActionType != ScrimActionType.OutsideInterference)
            {
                if (_isScoringEnabled && !involvesBenchedPlayer)
                {
                    var scoringResult = await _scorer.ScoreAssistEvent(assistEvent);
                    assistEvent.Points = scoringResult.Points;
                    assistEvent.IsBanned = scoringResult.IsBanned;

                    var currentMatchId = _scrimMatchService.CurrentMatchId;
                    var currentRound = _scrimMatchService.CurrentMatchRound;

                    if (_isEventStoringEnabled && !string.IsNullOrWhiteSpace(currentMatchId))
                    {
                        switch (assistEvent.ActionType)
                        {
                            case ScrimActionType.DamageAssist:
                                await SaveScrimDamageAssistToDb(assistEvent, currentMatchId, currentRound, payload.WorldId);
                                break;

                            case ScrimActionType.GrenadeAssist:
                                await SaveScrimGrenadeAssistToDb(assistEvent, currentMatchId, currentRound, payload.WorldId);
                                break;

                            case ScrimActionType.SpotAssist:
                                await SaveScrimSpotAssistToDb(assistEvent, currentMatchId, currentRound, payload.WorldId);
                                break;
                        };
                    }
                }
            }

            _messageService.BroadcastScrimAssistActionEventMessage(new ScrimAssistActionEventMessage(assistEvent));
        }

        private ScrimActionType GetAssistScrimActionType(ScrimAssistActionEvent assistEvent)
        {
            // Determine if this is involves a non-tracked player
            if ((assistEvent.AttackerPlayer == null && !string.IsNullOrWhiteSpace(assistEvent.AttackerCharacterId))
                    || (assistEvent.VictimPlayer == null && !string.IsNullOrWhiteSpace(assistEvent.VictimCharacterId)))
            {
                return ScrimActionType.OutsideInterference;
            }

            var isTeamkillAssist = (assistEvent.AttackerPlayer != null
                                    && assistEvent.VictimPlayer != null
                                    && assistEvent.AttackerPlayer.TeamOrdinal == assistEvent.VictimPlayer.TeamOrdinal
                                    && assistEvent.AttackerPlayer != assistEvent.VictimPlayer);

            var isSuicideAssist = (assistEvent.AttackerPlayer != null
                                    && assistEvent.VictimPlayer != null
                                    && assistEvent.AttackerPlayer.TeamOrdinal == assistEvent.VictimPlayer.TeamOrdinal
                                    && assistEvent.AttackerPlayer == assistEvent.VictimPlayer);

            return assistEvent.ExperienceType switch
            {
                ExperienceType.DamageAssist => (!(isTeamkillAssist || isSuicideAssist) ? ScrimActionType.DamageAssist
                                                    : (isTeamkillAssist ? ScrimActionType.DamageTeamAssist
                                                            : ScrimActionType.DamageSelfAssist)),
                ExperienceType.GrenadeAssist => (!(isTeamkillAssist || isSuicideAssist) ? ScrimActionType.GrenadeAssist
                                                    : (isTeamkillAssist ? ScrimActionType.GrenadeTeamAssist
                                                            : ScrimActionType.GrenadeSelfAssist)),
                ExperienceType.HealSupportAssist => ScrimActionType.HealSupportAssist,
                ExperienceType.ProtectAlliesAssist => ScrimActionType.ProtectAlliesAssist,
                ExperienceType.SpotAssist => ScrimActionType.SpotAssist,
                _ => ScrimActionType.UtilityAssist
            };
        }

        private async Task SaveScrimDamageAssistToDb(ScrimAssistActionEvent assistEvent, string matchId, int matchRound, int worldId)
        {
            var dataModel = new ScrimDamageAssist
            {
                ScrimMatchId = matchId,
                Timestamp = assistEvent.Timestamp,
                AttackerCharacterId = assistEvent.AttackerPlayer.Id,
                VictimCharacterId = assistEvent.VictimPlayer.Id,
                ScrimMatchRound = matchRound,
                ActionType = assistEvent.ActionType,
                AttackerTeamOrdinal = assistEvent.AttackerPlayer.TeamOrdinal,
                VictimTeamOrdinal = assistEvent.VictimPlayer.TeamOrdinal,
                AttackerLoadoutId = assistEvent.AttackerPlayer?.LoadoutId != null ? assistEvent.AttackerPlayer.LoadoutId : null,
                VictimLoadoutId = assistEvent.VictimPlayer?.LoadoutId != null ? assistEvent.VictimPlayer.LoadoutId : null,
                ExperienceGainId = assistEvent.ExperienceGainInfo.Id,
                ExperienceGainAmount = assistEvent.ExperienceGainInfo.Amount,
                ZoneId = assistEvent.ZoneId != null ? assistEvent.ZoneId : null,
                WorldId = worldId,
                Points = assistEvent.Points,
            };

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            dbContext.ScrimDamageAssists.Add(dataModel);
            await dbContext.SaveChangesAsync();
        }

        private async Task SaveScrimGrenadeAssistToDb(ScrimAssistActionEvent assistEvent, string matchId, int matchRound, int worldId)
        {
            var dataModel = new ScrimGrenadeAssist
            {
                ScrimMatchId = matchId,
                Timestamp = assistEvent.Timestamp,
                AttackerCharacterId = assistEvent.AttackerPlayer.Id,
                VictimCharacterId = assistEvent.VictimPlayer.Id,
                ScrimMatchRound = matchRound,
                ActionType = assistEvent.ActionType,
                AttackerTeamOrdinal = assistEvent.AttackerPlayer.TeamOrdinal,
                VictimTeamOrdinal = assistEvent.VictimPlayer.TeamOrdinal,
                AttackerLoadoutId = assistEvent.AttackerPlayer?.LoadoutId != null ? assistEvent.AttackerPlayer.LoadoutId : null,
                VictimLoadoutId = assistEvent.VictimPlayer?.LoadoutId != null ? assistEvent.VictimPlayer.LoadoutId : null,
                ExperienceGainId = assistEvent.ExperienceGainInfo.Id,
                ExperienceGainAmount = assistEvent.ExperienceGainInfo.Amount,
                ZoneId = assistEvent.ZoneId != null ? assistEvent.ZoneId : null,
                WorldId = worldId,
                Points = assistEvent.Points,
            };

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            dbContext.ScrimGrenadeAssists.Add(dataModel);
            await dbContext.SaveChangesAsync();
        }

        private async Task SaveScrimSpotAssistToDb(ScrimAssistActionEvent assistEvent, string matchId, int matchRound, int worldId)
        {
            var dataModel = new ScrimSpotAssist
            {
                ScrimMatchId = matchId,
                Timestamp = assistEvent.Timestamp,
                SpotterCharacterId = assistEvent.AttackerPlayer.Id,
                VictimCharacterId = assistEvent.VictimPlayer.Id,
                ScrimMatchRound = matchRound,
                ActionType = assistEvent.ActionType,
                SpotterTeamOrdinal = assistEvent.AttackerPlayer.TeamOrdinal,
                VictimTeamOrdinal = assistEvent.VictimPlayer.TeamOrdinal,
                SpotterLoadoutId = assistEvent.AttackerPlayer?.LoadoutId != null ? assistEvent.AttackerPlayer.LoadoutId : null,
                VictimLoadoutId = assistEvent.VictimPlayer?.LoadoutId != null ? assistEvent.VictimPlayer.LoadoutId : null,
                ExperienceGainId = assistEvent.ExperienceGainInfo.Id,
                ExperienceGainAmount = assistEvent.ExperienceGainInfo.Amount,
                ZoneId = assistEvent.ZoneId != null ? assistEvent.ZoneId : null,
                WorldId = worldId,
                Points = assistEvent.Points,
            };

            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            dbContext.ScrimSpotAssists.Add(dataModel);
            await dbContext.SaveChangesAsync();
        }

        private async Task ProcessPointControlPayload(ScrimExperienceGainActionEvent baseEvent, GainExperiencePayload payload)
        {
            var controlEvent = new ScrimObjectiveTickActionEvent(baseEvent);

            string playerId = payload.CharacterId;

            bool isValidAttackerId = (playerId != null && playerId.Length > 18);

            if (!isValidAttackerId)
            {
                return;
            }

            controlEvent.PlayerCharacterId = playerId;

            var player = _teamsManager.GetPlayerFromId(playerId);
            controlEvent.Player = player;

            _teamsManager.SetPlayerLoadoutId(playerId, controlEvent.LoadoutId);

            controlEvent.ActionType = GetObjectiveTickScrimActionType(controlEvent);

            var involvesBenchedPlayer = player.IsBenched;

            if (controlEvent.ActionType != ScrimActionType.Unknown)
            {
                if (_isScoringEnabled && !involvesBenchedPlayer)
                {
                    var scoringResult = await _scorer.ScoreObjectiveTickEvent(controlEvent);
                    controlEvent.Points = scoringResult.Points;
                    controlEvent.IsBanned = scoringResult.IsBanned;
                }
            }

            _messageService.BroadcastScrimObjectiveTickActionEventMessage(new ScrimObjectiveTickActionEventMessage(controlEvent));
        }

        private ScrimActionType GetObjectiveTickScrimActionType(ScrimObjectiveTickActionEvent controlEvent)
        {
            var experienceId = controlEvent.ExperienceGainInfo.Id;

            return experienceId switch
            {
                15 => ScrimActionType.PointControl,             // Control Point - Defend (100xp)
                16 => ScrimActionType.PointDefend,              // Control Point - Attack (100xp)
                272 => ScrimActionType.ConvertCapturePoint,     // Convert Capture Point (25xp)
                556 => ScrimActionType.ObjectiveDefensePulse,   // Objective Pulse Defend (50xp)
                557 => ScrimActionType.ObjectiveCapturePulse,   // Objective Pulse Capture (100xp)
                _ => ScrimActionType.Unknown
            };
        }
        #endregion

        #region FacilityControl Payloads
        [CensusEventHandler("FacilityControl", typeof(FacilityControlPayload))]
        private async Task Process(FacilityControlPayload payload)
        {
            _logger.LogInformation($"Processing FacilityControl payload...");
            
            if (!await _facilityControlFilter.TryFilterNewPayload(payload, p => p.Timestamp.ToString("s")))
            {
                _logger.LogWarning("Duplicate Facility Control payload detected, excluded");
                return;
            }

            var oldFactionId = payload.OldFactionId;
            var newFactionId = payload.NewFactionId;

            var type = GetFacilityControlType(oldFactionId, newFactionId);

            if (type == FacilityControlType.Unknown)
            {
                _logger.LogInformation($"FacilityControl payload had Unknow FacilityControlType: worldId={payload.WorldId} facilityId={payload.FacilityId}");
                
                return;
            }

            var controllingTeamOrdinal = _teamsManager.GetFirstTeamWithFactionId(newFactionId);
            if (controllingTeamOrdinal == null)
            {
                _logger.LogInformation($"Could not resolve controlling team for {type} FacilityControl payload: worldId={payload.WorldId} facilityId={payload.FacilityId} newFactionId={newFactionId} oldFactionId={oldFactionId}");

                return;
            }

            var actionType = GetFacilityControlActionType(type);

            _logger.LogInformation($"FacilityControl payload has FacilityControlType of {type} & ScrimActionType of {actionType} ({(int)actionType}) for Team {controllingTeamOrdinal}: worldId={payload.WorldId} facilityId={payload.FacilityId} newFactionId={newFactionId} oldFactionId={oldFactionId}");

            // "Outside Influence" doesn't really apply to base captures
            if (actionType == ScrimActionType.None)
            {
                return;
            }

            var mapRegion = await _facilityService.GetScrimmableMapRegionFromFacilityIdAsync(payload.FacilityId);

            var controlEvent = new ScrimFacilityControlActionEvent
            {
                FacilityId = payload.FacilityId,
                FacilityName = mapRegion.FacilityName,
                NewFactionId = payload.NewFactionId,
                OldFactionId = payload.OldFactionId,
                DurationHeld = payload.DurationHeld,
                OutfitId = payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value,
                ControllingTeamOrdinal = (int)controllingTeamOrdinal,
                ControlType = type,
                ActionType = actionType
            };

            if (_isScoringEnabled)
            {
                var scoringResult = _scorer.ScoreFacilityControlEvent(controlEvent);
                controlEvent.Points = scoringResult.Points;
                controlEvent.IsBanned = scoringResult.IsBanned;

                var currentMatchId = _scrimMatchService.CurrentMatchId;
                var currentRound = _scrimMatchService.CurrentMatchRound;

                var controllingFaction = _teamsManager.GetTeam(controlEvent.ControllingTeamOrdinal)?.FactionId;

                if (_isEventStoringEnabled && !string.IsNullOrWhiteSpace(currentMatchId))
                {
                    var dataModel = new ScrimFacilityControl
                    {
                        ScrimMatchId = currentMatchId,
                        Timestamp = controlEvent.Timestamp,
                        ScrimMatchRound = currentRound,
                        ControllingTeamOrdinal = controlEvent.ControllingTeamOrdinal,
                        ActionType = controlEvent.ActionType,
                        ControlType = controlEvent.ControlType,
                        ControllingFactionId = (int)controllingFaction,
                        ZoneId = controlEvent.ZoneId != null ? controlEvent.ZoneId : (int?)null,
                        FacilityId = payload.FacilityId,
                        WorldId = payload.WorldId,
                        Points = controlEvent.Points,
                    };

                    using var factory = _dbContextHelper.GetFactory();
                    var dbContext = factory.GetDbContext();

                    dbContext.ScrimFacilityControls.Add(dataModel);
                    await dbContext.SaveChangesAsync();
                }
            }

            _messageService.BroadcastScrimFacilityControlActionEventMessage(new ScrimFacilityControlActionEventMessage(controlEvent));
        }

        private FacilityControlType GetFacilityControlType(int? oldFactionId, int? newFactionId)
        {
            if (newFactionId == null || newFactionId <= 0)
            {
                return FacilityControlType.Unknown;
            }
            else if (oldFactionId == null || oldFactionId <= 0)
            {
                return newFactionId == null
                        ? FacilityControlType.Unknown
                        : FacilityControlType.Capture;
            }
            else
            {
                return oldFactionId == newFactionId
                            ? FacilityControlType.Defense
                            : FacilityControlType.Capture;
            }
        }

        private ScrimActionType GetFacilityControlActionType(FacilityControlType type)
        {
            if (type == FacilityControlType.Unknown)
            {
                return ScrimActionType.None;
            }
            
            var matchRoundControlVictories = _teamsManager.GetCurrentMatchRoundBaseControlsCount();

            // Only the first defense in a round should ever count. After that, base always trades hands via captures
            if (type == FacilityControlType.Defense && matchRoundControlVictories != 0)
            {
                return ScrimActionType.None;
            }

            return (matchRoundControlVictories == 0)
                        ? ScrimActionType.FirstBaseCapture
                        : ScrimActionType.SubsequentBaseCapture;
        }
        #endregion

        #endregion Payload Handling

        public void Dispose()
        {
            return;
        }
    }
}
