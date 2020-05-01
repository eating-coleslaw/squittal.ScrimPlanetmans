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
using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.Models.Planetside.Events;
using squittal.ScrimPlanetmans.ScrimMatch;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
//using squittal.ScrimPlanetmans.Shared.Models;
//using squittal.ScrimPlanetmans.Shared.Models.Planetside;
//using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;

//using Microsoft.EntityFrameworkCore;
//using squittal.ScrimPlanetmans.Data;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketEventHandler : IWebsocketEventHandler
    {
        //private readonly IDbContextHelper _dbContextHelper;
        private readonly IItemService _itemService;
        private readonly ICharacterService _characterService;
        private readonly IFacilityService _facilityService;
        private readonly IVehicleService _vehicleService;
        private readonly IScrimTeamsManager _teamsManager;
        private readonly IScrimMatchScorer _scorer;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<WebsocketEventHandler> _logger;

        private readonly IDbContextHelper _dbContextHelper;
        private readonly IScrimMatchDataService _scrimMatchService;

        private readonly Dictionary<string, MethodInfo> _processMethods;

        private bool _isScoringEnabled = false;
        private bool _isEventStoringEnabled = false;


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

        public WebsocketEventHandler(IScrimTeamsManager teamsManager, ICharacterService characterService, IScrimMatchScorer scorer,
            IItemService itemService, IFacilityService facilityService, IVehicleService vehicleService, IScrimMessageBroadcastService messageService,
            IScrimMatchDataService scrimMatchService, IDbContextHelper dbContextHelper, ILogger<WebsocketEventHandler> logger)
        {
            _teamsManager = teamsManager;
            _itemService = itemService;
            _messageService = messageService;
            //_dbContextHelper = dbContextHelper;
            _characterService = characterService;
            _facilityService = facilityService;
            _vehicleService = vehicleService;
            _scorer = scorer;
            _logger = logger;

            _dbContextHelper = dbContextHelper;
            _scrimMatchService = scrimMatchService;

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
                        await Task.Run(()=>
                        {
                            Process(loginParam);
                        });
                        break;

                    case "PlayerLogout":
                        var logoutParam = jPayload.ToObject<PlayerLogoutPayload>(_payloadDeserializer);
                        await Task.Run(() =>
                        {
                            Process(logoutParam);
                        });
                        break;

                    case "GainExperience":
                        var experienceParam = jPayload.ToObject<GainExperiencePayload>(_payloadDeserializer);
                        await Task.Run(() =>
                        {
                            Process(experienceParam);
                        });
                        break;

                    case "FacilityControl":
                        var controlParam = jPayload.ToObject<FacilityControlPayload>(_payloadDeserializer);
                        await Task.Run(() =>
                        {
                            Process(controlParam);
                        });
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
            string attackerId = payload.AttackerCharacterId;
            string victimId = payload.CharacterId;

            bool isValidAttackerId = (attackerId != null && attackerId.Length > 18);
            bool isValidVictimId = (victimId != null && victimId.Length > 18);

            Player attackerPlayer;
            Player victimPlayer;

            ScrimDeathActionEvent deathEvent = new ScrimDeathActionEvent
            {
                Timestamp = payload.Timestamp,
                ZoneId = payload.ZoneId,
                IsHeadshot = payload.IsHeadshot
            };

            var weaponItem = await _itemService.GetItem((int)payload.AttackerWeaponId);
            if (weaponItem != null)
            {
                deathEvent.Weapon = new ScrimActionWeaponInfo()
                {
                    Id = weaponItem.Id,
                    ItemCategoryId = (int)weaponItem.ItemCategoryId,
                    Name = weaponItem.Name,
                    IsVehicleWeapon = weaponItem.IsVehicleWeapon
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

                    if (_isScoringEnabled)
                    {
                        //_scorer.ScoreDeathEvent(dataModel);
                        var points = _scorer.ScoreDeathEvent(deathEvent);
                        deathEvent.Points = points;

                        var currentMatchId = _scrimMatchService.CurrentMatchId;

                        if (_isEventStoringEnabled && !string.IsNullOrWhiteSpace(currentMatchId))
                        {
                            var dataModel = new Data.Models.ScrimDeath
                            {
                                ScrimMatchId = currentMatchId,
                                Timestamp = deathEvent.Timestamp,
                                AttackerCharacterId = deathEvent.AttackerPlayer.Id,
                                VictimCharacterId = deathEvent.VictimPlayer.Id,
                                ActionType = deathEvent.ActionType,
                                DeathType = deathEvent.DeathType,
                                ZoneId = deathEvent.ZoneId,
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
                                AttackerResultingPoints = deathEvent.AttackerPlayer.EventAggregate.Points,
                                AttackerResultingNetScore = deathEvent.AttackerPlayer.EventAggregate.NetScore,
                                VictimResultingPoints = deathEvent.VictimPlayer.EventAggregate.Points,
                                VictimResultingNetScore = deathEvent.VictimPlayer.EventAggregate.NetScore
                            };

                            using var factory = _dbContextHelper.GetFactory();
                            var dbContext = factory.GetDbContext();

                            dbContext.ScrimDeaths.Add(dataModel);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }

                //var dataModel = new Death
                //{
                //    AttackerCharacterId = attackerId,
                //    AttackerFireModeId = payload.AttackerFireModeId,
                //    AttackerLoadoutId = payload.AttackerLoadoutId,
                //    AttackerVehicleId = payload.AttackerVehicleId,
                //    AttackerWeaponId = payload.AttackerWeaponId,
                //    //AttackerOutfitId = attackerOutfitTask?.Result?.OutfitId,
                //    //AttackerTeamOrdinal = attackerTeamOrdinal,
                //    AttackerFactionId = attackerFactionId,
                //    CharacterId = victimId,
                //    CharacterLoadoutId = payload.CharacterLoadoutId,
                //    //CharacterOutfitId = victimOutfitTask?.Result?.OutfitId,
                //    //CharacterTeamOrdinal = victimTeamOrdinal,
                //    CharacterFactionId = victimFactionId,
                //    IsHeadshot = payload.IsHeadshot,
                //    DeathEventType = deathEventType,
                //    Timestamp = payload.Timestamp,
                //    WorldId = payload.WorldId,
                //    ZoneId = payload.ZoneId.Value
                //};

                //if (_isScoringEnabled)
                //{
                //    //_scorer.ScoreDeathEvent(dataModel);
                //    var points = _scorer.ScoreDeathEvent(deathEvent);
                //    deathEvent.Points = points;
                //}

                _messageService.BroadcastScrimDeathActionEventMessage(new ScrimDeathActionEventMessage(deathEvent));

                //return dataModel;
                return deathEvent;

                //dbContext.Deaths.Add(dataModel);
                //await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Ignore
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

            var attackerIsVehicle = death.Weapon.IsVehicleWeapon;

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
            string attackerId = payload.AttackerCharacterId;
            string victimId = payload.CharacterId;

            bool isValidAttackerId = (attackerId != null && attackerId.Length > 18);
            bool isValidVictimId = (victimId != null && victimId.Length > 18);

            Player attackerPlayer;
            Player victimPlayer;

            ScrimVehicleDestructionActionEvent destructionEvent = new ScrimVehicleDestructionActionEvent
            {
                Timestamp = payload.Timestamp,
                ZoneId = payload.ZoneId,
            };

            var weaponItem = await _itemService.GetItem((int)payload.AttackerWeaponId);
            if (weaponItem != null)
            {
                destructionEvent.Weapon = new ScrimActionWeaponInfo
                {
                    Id = weaponItem.Id,
                    ItemCategoryId = (int)weaponItem.ItemCategoryId,
                    Name = weaponItem.Name,
                    IsVehicleWeapon = weaponItem.IsVehicleWeapon
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

                    }
                }

                if (isValidVictimId == true)
                {
                    destructionEvent.VictimCharacterId = victimId;

                    victimPlayer = _teamsManager.GetPlayerFromId(victimId);
                    destructionEvent.VictimPlayer = victimPlayer;
                }

                destructionEvent.DeathType = GetVehicleDestructionDeathType(destructionEvent);

                destructionEvent.ActionType = GetVehicleDestructionScrimActionType(destructionEvent);


                if (destructionEvent.ActionType != ScrimActionType.OutsideInterference)
                {
                    if (destructionEvent.DeathType == DeathEventType.Suicide)
                    {
                        destructionEvent.AttackerPlayer = destructionEvent.VictimPlayer;
                        destructionEvent.AttackerCharacterId = destructionEvent.VictimCharacterId;
                    }

                    if (_isScoringEnabled)
                    {
                        var points = _scorer.ScoreVehicleDestructionEvent(destructionEvent);
                        destructionEvent.Points = points;
                    }
                }

                _messageService.BroadcastScrimVehicleDestructionActionEventMessage(new ScrimVehicleDestructionActionEventMessage(destructionEvent));

                return destructionEvent;
            }
            catch (Exception)
            {
                //Ignore
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

            var attackerIsVehicle = (destruction.Weapon.IsVehicleWeapon || (destruction.AttackerVehicle != null && destruction.AttackerVehicle.Type != VehicleType.Unknown));

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
        //private Task<PlayerLogin> Process(PlayerLoginPayload payload)
        private PlayerLogin Process(PlayerLoginPayload payload)
        {
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
        //private Task<PlayerLogout> Process(PlayerLogoutPayload payload)
        private PlayerLogout Process(PlayerLogoutPayload payload)
        {
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
        //private Task<GainExperience> Process(GainExperiencePayload payload)
        [CensusEventHandler("GainExperience", typeof(GainExperiencePayload))]
        private void Process(GainExperiencePayload payload)
        {
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

            switch (experienceType)
            {
                case ExperienceType.Revive:
                    ProcessRevivePayload(baseEvent, payload);
                    return;

                case ExperienceType.DamageAssist:
                    ProcessAssistPayload(baseEvent, payload);
                    return;

                case ExperienceType.UtilityAssist:
                    ProcessAssistPayload(baseEvent, payload);
                    return;

                case ExperienceType.PointControl:
                    ProcessPointControlPayload(baseEvent, payload);
                    return;

                default:
                    return;
            }

            //var characterId = payload.CharacterId;

            //var player = _teamsManager.GetPlayerFromId(characterId);


            //var dataModel = new GainExperience
            //{
            //    Id = Guid.NewGuid(),
            //    ExperienceId = payload.ExperienceId,
            //    CharacterId = payload.CharacterId,
            //    Amount = payload.Amount,
            //    LoadoutId = payload.LoadoutId,
            //    OtherId = payload.OtherId,
            //    Timestamp = payload.Timestamp,
            //    WorldId = payload.WorldId,
            //    ZoneId = payload.ZoneId.Value
            //};

            //return Task.FromResult(dataModel);
            //return dataModel;
        }

        private void ProcessRevivePayload(ScrimExperienceGainActionEvent baseEvent, GainExperiencePayload payload)
        {
            var reviveEvent = new ScrimReviveActionEvent(baseEvent);

            string medicId = payload.CharacterId;
            string revivedId = payload.OtherId;

            bool isValidMedicId = (medicId != null && medicId.Length > 18);
            bool isValidRevivedId = (revivedId != null && revivedId.Length > 18);

            Player medicPlayer;
            Player revivedPlayer;

            if (isValidMedicId == true)
            {
                reviveEvent.MedicCharacterId = medicId;

                medicPlayer = _teamsManager.GetPlayerFromId(medicId);
                reviveEvent.MedicPlayer = medicPlayer;

                _teamsManager.SetPlayerLoadoutId(medicId, reviveEvent.LoadoutId);
            }

            if (isValidRevivedId == true)
            {
                reviveEvent.RevivedCharacterId = revivedId;

                revivedPlayer = _teamsManager.GetPlayerFromId(revivedId);
                reviveEvent.RevivedPlayer = revivedPlayer;
            }

            reviveEvent.ActionType = GetReviveScrimActionType(reviveEvent);

            if (reviveEvent.ActionType != ScrimActionType.OutsideInterference)
            {
                if (_isScoringEnabled)
                {
                    var points = _scorer.ScoreReviveEvent(reviveEvent);
                    reviveEvent.Points = points;
                }
            }

            _messageService.BroadcastScrimReviveActionEventMessage(new ScrimReviveActionEventMessage(reviveEvent));
        }

        private ScrimActionType GetReviveScrimActionType(ScrimReviveActionEvent reviveEvent)
        {
            // Determine if this is involves a non-tracked player
            if ((reviveEvent.MedicPlayer == null && !string.IsNullOrWhiteSpace(reviveEvent.MedicCharacterId))
                    || (reviveEvent.RevivedPlayer == null && !string.IsNullOrWhiteSpace(reviveEvent.RevivedCharacterId)))
            {
                return ScrimActionType.OutsideInterference;
            }

            bool isRevivedMax = ProfileService.IsMaxLoadoutId(reviveEvent.RevivedPlayer.LoadoutId);

            return isRevivedMax
                        ? ScrimActionType.ReviveMax
                        : ScrimActionType.ReviveInfantry;
        }

        private void ProcessAssistPayload(ScrimExperienceGainActionEvent baseEvent, GainExperiencePayload payload)
        {
            var assistEvent = new ScrimAssistActionEvent(baseEvent);

            string attackerId = payload.CharacterId;
            string victimId = payload.OtherId;

            bool isValidattackerId = (attackerId != null && attackerId.Length > 18);
            bool isValidvictimId = (victimId != null && victimId.Length > 18);

            Player attackerPlayer;
            Player victimPlayer;

            if (isValidattackerId == true)
            {
                assistEvent.AttackerCharacterId = attackerId;

                attackerPlayer = _teamsManager.GetPlayerFromId(attackerId);
                assistEvent.AttackerPlayer = attackerPlayer;

                _teamsManager.SetPlayerLoadoutId(attackerId, assistEvent.LoadoutId);
            }

            if (isValidvictimId == true)
            {
                assistEvent.VictimCharacterId = victimId;

                victimPlayer = _teamsManager.GetPlayerFromId(victimId);
                assistEvent.VictimPlayer = victimPlayer;
            }

            assistEvent.ActionType = GetAssistScrimActionType(assistEvent);

            if (assistEvent.ActionType != ScrimActionType.OutsideInterference)
            {
                if (_isScoringEnabled)
                {
                    var points = _scorer.ScoreAssistEvent(assistEvent);
                    assistEvent.Points = points;
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

            return assistEvent.ExperienceType == ExperienceType.DamageAssist
                        ? ScrimActionType.DamageAssist
                        : ScrimActionType.UtilityAssist;
        }

        private void ProcessPointControlPayload(ScrimExperienceGainActionEvent baseEvent, GainExperiencePayload payload)
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

            if (controlEvent.ActionType != ScrimActionType.Unknown)
            {
                if (_isScoringEnabled)
                {
                    var points = _scorer.ScoreObjectiveTickEvent(controlEvent);
                    controlEvent.Points = points;
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
        private void Process(FacilityControlPayload payload)
        {
            var oldFactionId = payload.OldFactionId;
            var newFactionId = payload.NewFactionId;

            var type = GetFacilityControlType(oldFactionId, newFactionId);

            if (type == FacilityControlType.Unknown)
            {
                return;
            }

            var controllingTeamOrdinal = _teamsManager.GetFirstTeamWithFactionId(newFactionId);
            if (controllingTeamOrdinal == null)
            {
                return;
            }

            var actionType = GetFacilityControlActionType(type, (int)controllingTeamOrdinal);

            // "Outside Influence" doesn't really apply to base captures
            if (actionType == ScrimActionType.None)
            {
                return;
            }

            var mapRegion = _facilityService.GetScrimmableMapRegionFromFacilityId(payload.FacilityId);

            //var controlModel = new FacilityControl
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

            //_scorer.ScoreFacilityControlEvent(controlModel, out bool controlCounts);

            if (_isScoringEnabled)
            {
                var points = _scorer.ScoreFacilityControlEvent(controlEvent);
                controlEvent.Points = points;
            }

            // TODO: broadcast Facility Control message
            _messageService.BroadcastScrimFacilityControlActionEventMessage(new ScrimFacilityControlActionEventMessage(controlEvent));

            //return Task.FromResult(dataModel);
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

        private ScrimActionType GetFacilityControlActionType(FacilityControlType type, int teamOrdinal)
        {
            var team = _teamsManager.GetTeam(teamOrdinal);

            var roundControlVictories = team.EventAggregateTracker.RoundStats.BaseControlVictories;

            //if (roundControlVictories == 0)
            //{
            //    return true;
            //}

            var previousScoredControlType = team.EventAggregateTracker.RoundStats.PreviousScoredBaseControlType;

            if (type != previousScoredControlType && roundControlVictories != 0)
            {
                return ScrimActionType.None;
            }

            return (roundControlVictories == 0)
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
