using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Models.Planetside.Events;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System.Linq;
using System.Threading.Tasks;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimMatchScorer : IScrimMatchScorer
    {
        private readonly IScrimRulesetManager _rulesets;
        private readonly IScrimTeamsManager _teamsManager;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<ScrimMatchEngine> _logger;

        private Ruleset _activeRuleset;

        public ScrimMatchScorer(IScrimRulesetManager rulesets, IScrimTeamsManager teamsManager, IScrimMessageBroadcastService messageService, ILogger<ScrimMatchEngine> logger)
        {
            _rulesets = rulesets;
            _teamsManager = teamsManager;
            _messageService = messageService;
            _logger = logger;

            _messageService.RaiseActiveRulesetChangeEvent += OnActiveRulesetChangeEvent;
            _messageService.RaiseRulesetRuleChangeEvent += OnRulesetRuleChangeEvent;
        }

        private async void OnActiveRulesetChangeEvent(object sender, ActiveRulesetChangeEventArgs e)
        {
            await SetActiveRulesetAsync();
        }

        private async void OnRulesetRuleChangeEvent(object sender, RulesetRuleChangeEventArgs e)
        {
            if (_activeRuleset.Id == e.Message.Ruleset.Id)
            {
                await SetActiveRulesetAsync();
            }

            // TODO: specific methods for only updating Rule Type that changed (Action Rules or Item Category Rules)
        }

        public async Task SetActiveRulesetAsync()
        {
            _activeRuleset = await _rulesets.GetActiveRulesetAsync();
        }

        #region Death Events
        public async Task<int> ScoreDeathEvent(ScrimDeathActionEvent death)
        {
            switch (death.DeathType)
            {
                case DeathEventType.Kill:
                    return await ScoreKill(death);

                case DeathEventType.Suicide:
                    return await ScoreSuicide(death);

                case DeathEventType.Teamkill:
                    return await ScoreTeamkill(death);

                default:
                    return 0;
            }
        }

        private async Task<int> ScoreKill(ScrimDeathActionEvent death)
        {
            int points = 0;

            if (GetDeferToItemCategoryPoints(death.ActionType))
            {
                var categoryId = death.Weapon?.ItemCategoryId;

                if (categoryId != null)
                {
                    points = _activeRuleset.RulesetItemCategoryRules
                                                .Where(rule => rule.ItemCategoryId == categoryId)
                                                .Select(rule => rule.Points)
                                                .FirstOrDefault();
                }
            }
            else
            {
                var actionType = death.ActionType;
                points = GetActionRulePoints(actionType);
            }

            var isHeadshot = (death.IsHeadshot ? 1 : 0);

            var attackerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                Kills = 1,
                Headshots = isHeadshot
            };

            var victimUpdate = new ScrimEventAggregate()
            {
                NetScore = -points,
                Deaths = 1,
                HeadshotDeaths = isHeadshot
            };

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(death.AttackerPlayer.Id, attackerUpdate);
            await _teamsManager.UpdatePlayerStats(death.VictimPlayer.Id, victimUpdate);

            return points;
        }

        private async Task<int> ScoreSuicide(ScrimDeathActionEvent death)
        {
            var actionType = death.ActionType;
            var points = GetActionRulePoints(actionType);

            var victimUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                Deaths = 1,
                Suicides = 1
            };

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(death.VictimPlayer.Id, victimUpdate);

            return points;
        }

        private async Task<int> ScoreTeamkill(ScrimDeathActionEvent death)
        {
            var actionType = death.ActionType;
            var points = GetActionRulePoints(actionType);

            var attackerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                Teamkills = 1
            };

            var victimUpdate = new ScrimEventAggregate()
            {
                Deaths = 1,
                TeamkillDeaths = 1
            };

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(death.AttackerPlayer.Id, attackerUpdate);
            await _teamsManager.UpdatePlayerStats(death.VictimPlayer.Id, victimUpdate);

            return points;
        }
        #endregion Death Events

        #region Vehicle Destruction Events
        public async Task<int> ScoreVehicleDestructionEvent(ScrimVehicleDestructionActionEvent destruction)
        {
            return destruction.DeathType switch
            {
                DeathEventType.Kill => await ScoreVehicleDestruction(destruction),
                DeathEventType.Suicide => await ScoreVehicleTeamDestruction(destruction),
                DeathEventType.Teamkill => await ScoreVehicleSuicideDestruction(destruction),
                _ => 0,
            };
        }

        private async Task<int> ScoreVehicleDestruction(ScrimVehicleDestructionActionEvent destruction)
        {
            int points = 0;

            if (GetDeferToItemCategoryPoints(destruction.ActionType))
            {
                var categoryId = destruction.Weapon.ItemCategoryId;

                if (categoryId != null)
                {
                    points = _activeRuleset.RulesetItemCategoryRules
                                                .Where(rule => rule.ItemCategoryId == categoryId)
                                                .Select(rule => rule.Points)
                                                .FirstOrDefault();
                }
            }
            else
            {
                var actionType = destruction.ActionType;
                points = GetActionRulePoints(actionType);
            }

            var attackerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
            };

            attackerUpdate.Add(GetVehicleDestroyedEventAggregate(destruction.VictimVehicle.Type));

            var victimUpdate = new ScrimEventAggregate()
            {
                NetScore = -points,
            };

            victimUpdate.Add(GetVehicleLostEventAggregate(destruction.VictimVehicle.Type));

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(destruction.AttackerPlayer.Id, attackerUpdate);

            if (destruction.VictimPlayer != null)
            {
                await _teamsManager.UpdatePlayerStats(destruction.VictimPlayer.Id, victimUpdate);
            }

            return points;

        }

        private async Task<int> ScoreVehicleSuicideDestruction(ScrimVehicleDestructionActionEvent destruction)
        {
            int points;

            if (GetDeferToItemCategoryPoints(destruction.ActionType))
            {
                var categoryId = destruction.Weapon.ItemCategoryId;
                points = _activeRuleset.RulesetItemCategoryRules
                                            .Where(rule => rule.ItemCategoryId == categoryId)
                                            .Select(rule => rule.Points)
                                            .FirstOrDefault();
            }
            else
            {
                var actionType = destruction.ActionType;
                points = GetActionRulePoints(actionType);
            }

            var victimUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
            };

            victimUpdate.Add(GetVehicleLostEventAggregate(destruction.VictimVehicle.Type));

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(destruction.VictimPlayer.Id, victimUpdate);

            return points;
        }

        private async Task<int> ScoreVehicleTeamDestruction(ScrimVehicleDestructionActionEvent destruction)
        {
            int points;

            if (GetDeferToItemCategoryPoints(destruction.ActionType))
            {
                var categoryId = destruction.Weapon.ItemCategoryId;
                points = _activeRuleset.RulesetItemCategoryRules
                                            .Where(rule => rule.ItemCategoryId == categoryId)
                                            .Select(rule => rule.Points)
                                            .FirstOrDefault();
            }
            else
            {
                var actionType = destruction.ActionType;
                points = GetActionRulePoints(actionType);
            }

            var attackerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
            };

            var victimUpdate = GetVehicleLostEventAggregate(destruction.VictimVehicle.Type);

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(destruction.AttackerPlayer.Id, attackerUpdate);
            await _teamsManager.UpdatePlayerStats(destruction.VictimPlayer.Id, victimUpdate);

            return points;
        }

        private ScrimEventAggregate GetVehicleDestroyedEventAggregate(VehicleType vehicleType)
        {
            return new ScrimEventAggregate()
            {
                VehiclesDestroyed = 1,

                InterceptorsDestroyed = vehicleType == VehicleType.Interceptor ? 1 : 0,
                EsfsDestroyed = vehicleType == VehicleType.ESF ? 1 : 0,
                ValkyriesDestroyed = vehicleType == VehicleType.Valkyrie ? 1 : 0,
                LiberatorsDestroyed = vehicleType == VehicleType.Liberator ? 1 : 0,
                GalaxiesDestroyed = vehicleType == VehicleType.Galaxy ? 1 : 0,
                BastionsDestroyed = vehicleType == VehicleType.Bastion ? 1 : 0,

                FlashesDestroyed = vehicleType == VehicleType.Flash ? 1 : 0,
                HarassersDestroyed = vehicleType == VehicleType.Harasser ? 1 : 0,
                AntsDestroyed = vehicleType == VehicleType.ANT ? 1 : 0,
                SunderersDestroyed = vehicleType == VehicleType.Sunderer ? 1 : 0,
                LightningsDestroyed = vehicleType == VehicleType.Lightning ? 1 : 0,
                MbtsDestroyed = vehicleType == VehicleType.MBT ? 1 : 0
            };
        }

        private ScrimEventAggregate GetVehicleLostEventAggregate(VehicleType vehicleType)
        {
            return new ScrimEventAggregate()
            {
                VehiclesLost = 1,

                InterceptorsLost = vehicleType == VehicleType.Interceptor ? 1 : 0,
                EsfsLost = vehicleType == VehicleType.ESF ? 1 : 0,
                ValkyriesLost = vehicleType == VehicleType.Valkyrie ? 1 : 0,
                LiberatorsLost = vehicleType == VehicleType.Liberator ? 1 : 0,
                GalaxiesLost = vehicleType == VehicleType.Galaxy ? 1 : 0,
                BastionsLost = vehicleType == VehicleType.Bastion ? 1 : 0,

                FlashesLost = vehicleType == VehicleType.Flash ? 1 : 0,
                HarassersLost = vehicleType == VehicleType.Harasser ? 1 : 0,
                AntsLost = vehicleType == VehicleType.ANT ? 1 : 0,
                SunderersLost = vehicleType == VehicleType.Sunderer ? 1 : 0,
                LightningsLost = vehicleType == VehicleType.Lightning ? 1 : 0,
                MbtsLost = vehicleType == VehicleType.MBT ? 1 : 0
            };
        }
        #endregion Vehicle Destruction Events

        #region Experience Events
        public async Task<int> ScoreReviveEvent(ScrimReviveActionEvent revive)
        {
            var actionType = revive.ActionType;
            var points = GetActionRulePoints(actionType);

            var medicUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                RevivesGiven = 1
            };

            var revivedUpdate = new ScrimEventAggregate()
            {
                RevivesTaken = 1
            };

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(revive.MedicPlayer.Id, medicUpdate);
            await _teamsManager.UpdatePlayerStats(revive.RevivedPlayer.Id, revivedUpdate);

            return points;
        }

        public async Task<int> ScoreAssistEvent(ScrimAssistActionEvent assist)
        {
            var actionType = assist.ActionType;
            var points = GetActionRulePoints(actionType);

            var attackerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points
            };

            var victimUpdate = new ScrimEventAggregate();

            if (actionType == ScrimActionType.DamageAssist)
            {
                attackerUpdate.DamageAssists = 1;
                victimUpdate.DamageAssistedDeaths = 1;
            }
            else if (actionType == ScrimActionType.DamageTeamAssist)
            {
                attackerUpdate.DamageTeamAssists = 1;
                victimUpdate.DamageAssistedDeaths = 1;
                victimUpdate.DamageTeamAssistedDeaths = 1;
            }
            else if (actionType == ScrimActionType.DamageSelfAssist)
            {
                victimUpdate.DamageSelfAssists = 1;
                victimUpdate.DamageAssistedDeaths = 1;
                victimUpdate.DamageSelfAssistedDeaths = 1;
            }
            else if (actionType == ScrimActionType.GrenadeAssist)
            {
                attackerUpdate.GrenadeAssists = 1;
                victimUpdate.GrenadeAssistedDeaths = 1;
            }
            else if (actionType == ScrimActionType.GrenadeTeamAssist)
            {
                attackerUpdate.GrenadeTeamAssists = 1;
                victimUpdate.GrenadeAssistedDeaths = 1;
                victimUpdate.GrenadeTeamAssistedDeaths = 1;
            }
            else if (actionType == ScrimActionType.GrenadeSelfAssist)
            {
                victimUpdate.GrenadeSelfAssists = 1;
                victimUpdate.GrenadeAssistedDeaths = 1;
                victimUpdate.GrenadeSelfAssistedDeaths = 1;
            }
            else if (actionType == ScrimActionType.HealSupportAssist)
            {
                attackerUpdate.HealSupportAssists = 1;
            }
            else if (actionType == ScrimActionType.ProtectAlliesAssist)
            {
                attackerUpdate.ProtectAlliesAssists = 1;
                victimUpdate.ProtectAlliesAssistedDeaths = 1;
            }
            else if (actionType == ScrimActionType.SpotAssist)
            {
                attackerUpdate.SpotAssists = 1;
                victimUpdate.SpotAssistedDeaths = 1;
            }

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(assist.AttackerPlayer.Id, attackerUpdate);

            if (assist.VictimPlayer != null)
            {
                await _teamsManager.UpdatePlayerStats(assist.VictimPlayer.Id, victimUpdate);
            }

            return points;
        }

        public async Task<int> ScoreObjectiveTickEvent(ScrimObjectiveTickActionEvent objective)
        {
            var actionType = objective.ActionType;
            var points = _activeRuleset.RulesetActionRules
                                        .Where(rule => rule.ScrimActionType == actionType)
                                        .Select(rule => rule.Points)
                                        .FirstOrDefault();

            var playerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points
            };

            var isDefense = (actionType == ScrimActionType.PointDefend
                                || actionType == ScrimActionType.ObjectiveDefensePulse);

            if (isDefense)
            {
                playerUpdate.ObjectiveDefenseTicks = 1;
            }
            else
            {
                playerUpdate.ObjectiveCaptureTicks = 1;
            }

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(objective.Player.Id, playerUpdate);

            return points;
        }

        #endregion Experience Events

        #region Objective Events
        public int ScoreFacilityControlEvent(ScrimFacilityControlActionEvent control)
        {
            var teamOrdinal = control.ControllingTeamOrdinal;
            var type = control.ControlType;

            var actionType = control.ActionType;
            var points = GetActionRulePoints(actionType);

            var teamUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                BaseCaptures = (type == FacilityControlType.Capture ? 1 : 0),
                BaseDefenses = (type == FacilityControlType.Defense ? 1 : 0)
            };

            _teamsManager.UpdateTeamStats(teamOrdinal, teamUpdate);
            
            return points;
        }
        #endregion Objective Events

        #region Misc. Non-Scored Events
        public void HandlePlayerLogin(PlayerLogin login)
        {
            var characterId = login.CharacterId;
            _teamsManager.SetPlayerOnlineStatus(characterId, true);
        }

        public void HandlePlayerLogout(PlayerLogout login)
        {
            var characterId = login.CharacterId;
            _teamsManager.SetPlayerOnlineStatus(characterId, false);
        }
        #endregion Misc. Non-Scored Events
    
        private int GetActionRulePoints(ScrimActionType actionType)
        {
            return _activeRuleset.RulesetActionRules
                                    .Where(rule => rule.ScrimActionType == actionType)
                                    .Select(rule => rule.Points)
                                    .FirstOrDefault();
        }

        private bool GetDeferToItemCategoryPoints(ScrimActionType actionType)
        {
            return _activeRuleset.RulesetActionRules
                                    .Where(rule => rule.ScrimActionType == actionType)
                                    .Select(rule => rule.DeferToItemCategoryRules)
                                    .FirstOrDefault();
        }
    }
}
