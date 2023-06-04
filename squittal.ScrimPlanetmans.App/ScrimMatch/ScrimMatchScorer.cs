using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Models.Planetside.Events;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System.Linq;
using System.Threading.Tasks;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimMatchScorer : IScrimMatchScorer
    {
        private readonly IScrimRulesetManager _rulesets;
        private readonly IScrimTeamsManager _teamsManager;
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<ScrimMatchEngine> _logger;

        private Ruleset _activeRuleset;

        private int? _periodicControlPointsValue;

        public ScrimMatchScorer(IScrimRulesetManager rulesets, IScrimTeamsManager teamsManager, IScrimMessageBroadcastService messageService, ILogger<ScrimMatchEngine> logger)
        {
            _rulesets = rulesets;
            _teamsManager = teamsManager;
            _messageService = messageService;
            _logger = logger;

            _messageService.RaiseActiveRulesetChangeEvent += OnActiveRulesetChangeEvent;
            _messageService.RaiseRulesetRuleChangeEvent += OnRulesetRuleChangeEvent;
            _messageService.RaiseMatchConfigurationUpdateEvent += OnMatchConfigurationUpdateEvent;
        }

        private async void OnActiveRulesetChangeEvent(object sender, ScrimMessageEventArgs<ActiveRulesetChangeMessage> e)
        {
            await SetActiveRulesetAsync();
        }

        private async void OnRulesetRuleChangeEvent(object sender, ScrimMessageEventArgs<RulesetRuleChangeMessage> e)
        {
            if (_activeRuleset.Id == e.Message.Ruleset.Id)
            {
                await SetActiveRulesetAsync();
            }

            // TODO: specific methods for only updating Rule Type that changed (Action Rules or Item Category Rules)
        }

        private void OnMatchConfigurationUpdateEvent(object sender, ScrimMessageEventArgs<MatchConfigurationUpdateMessage> e)
        {
            var message = e.Message;
            var matchConfiguration = message.MatchConfiguration;

            _periodicControlPointsValue = matchConfiguration.PeriodicFacilityControlPoints;
        }

        public async Task SetActiveRulesetAsync()
        {
            _activeRuleset = await _rulesets.GetActiveRulesetAsync();
        }

        #region Death Events
        public async Task<ScrimEventScoringResult> ScoreDeathEvent(ScrimDeathActionEvent death)
        {
            return death.DeathType switch
            {
                DeathEventType.Kill => await ScoreKill(death),
                DeathEventType.Suicide => await ScoreSuicide(death),
                DeathEventType.Teamkill => await ScoreTeamkill(death),
                _ => new ScrimEventScoringResult(ScrimEventScorePointsSource.Default, 0, false)
            };
        }

        private async Task<ScrimEventScoringResult> ScoreKill(ScrimDeathActionEvent death)
        {
            var scoringResult = GetDeathOrDestructionEventPoints(death.ActionType, death.Weapon?.ItemCategoryId, death.Weapon?.Id, death.AttackerLoadoutId);
            var points = scoringResult.Points;

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

            _teamsManager.TrySetPlayerLastKilledBy(death.VictimPlayer.Id, death.AttackerPlayer.Id);

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(death.AttackerPlayer.Id, attackerUpdate);
            await _teamsManager.UpdatePlayerStats(death.VictimPlayer.Id, victimUpdate);

            return scoringResult;
        }

        private async Task<ScrimEventScoringResult> ScoreSuicide(ScrimDeathActionEvent death)
        {
            var scoringResult = GetDeathOrDestructionEventPoints(death.ActionType, death.Weapon?.ItemCategoryId, death.Weapon?.Id, death.AttackerLoadoutId);
            var points = scoringResult.Points;

            var victimUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                Deaths = 1,
                Suicides = 1
            };

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(death.VictimPlayer.Id, victimUpdate);

            return scoringResult;
        }

        private async Task<ScrimEventScoringResult> ScoreTeamkill(ScrimDeathActionEvent death)
        {
            var scoringResult = GetDeathOrDestructionEventPoints(death.ActionType, death.Weapon?.ItemCategoryId, death.Weapon?.Id, death.AttackerLoadoutId);
            var points = scoringResult.Points;

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

            return scoringResult;
        }
        #endregion Death Events

        #region Vehicle Destruction Events
        public async Task<ScrimEventScoringResult> ScoreVehicleDestructionEvent(ScrimVehicleDestructionActionEvent destruction)
        {
            return destruction.DeathType switch
            {
                DeathEventType.Kill => await ScoreVehicleDestruction(destruction),
                DeathEventType.Suicide => await ScoreVehicleTeamDestruction(destruction),
                DeathEventType.Teamkill => await ScoreVehicleSuicideDestruction(destruction),
                _ => new ScrimEventScoringResult(ScrimEventScorePointsSource.Default, 0, false)
            };
        }

        private async Task<ScrimEventScoringResult> ScoreVehicleDestruction(ScrimVehicleDestructionActionEvent destruction)
        {
            var scoringResult = GetDeathOrDestructionEventPoints(destruction.ActionType, destruction.Weapon?.ItemCategoryId, destruction.Weapon?.Id, destruction.AttackerLoadoutId);
            var points = scoringResult.Points;

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

            return scoringResult;

        }

        private async Task<ScrimEventScoringResult> ScoreVehicleSuicideDestruction(ScrimVehicleDestructionActionEvent destruction)
        {
            var scoringResult = GetDeathOrDestructionEventPoints(destruction.ActionType, destruction.Weapon?.ItemCategoryId, destruction.Weapon?.Id, destruction.AttackerLoadoutId);
            var points = scoringResult.Points;

            var victimUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
            };

            victimUpdate.Add(GetVehicleLostEventAggregate(destruction.VictimVehicle.Type));

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(destruction.VictimPlayer.Id, victimUpdate);

            return scoringResult;
        }

        private async Task<ScrimEventScoringResult> ScoreVehicleTeamDestruction(ScrimVehicleDestructionActionEvent destruction)
        {
            var scoringResult = GetDeathOrDestructionEventPoints(destruction.ActionType, destruction.Weapon?.ItemCategoryId, destruction.Weapon?.Id, destruction.AttackerLoadoutId);
            var points = scoringResult.Points;

            var attackerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
            };

            var victimUpdate = GetVehicleLostEventAggregate(destruction.VictimVehicle.Type);

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(destruction.AttackerPlayer.Id, attackerUpdate);
            await _teamsManager.UpdatePlayerStats(destruction.VictimPlayer.Id, victimUpdate);

            return scoringResult;
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
        public async Task<ScrimEventScoringReviveResult> ScoreReviveEvent(ScrimReviveActionEvent revive)
        {
            var medicTeamOrdinal = revive.MedicPlayer.TeamOrdinal;
            
            var actionType = revive.ActionType;
            var scoringResult = GetActionRulePoints(actionType);
            var points = scoringResult.Points;

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

            // Additional same-team check is becasue players could technically be on different
            // factions but the same scrim team
            var lastKilledByPlayer = revive.LastKilledByPlayer; // _teamsManager.GetLastKilledByPlayer(revive.RevivedPlayer.Id);
            var lastDeathWasToEnemy = !_teamsManager.DoPlayersShareTeam(revive.RevivedPlayer, lastKilledByPlayer);

            var enemyActionType = revive.ActionType == ScrimActionType.ReviveMax
                            ? ScrimActionType.EnemyRevivedMax
                            : ScrimActionType.EnemyRevivedInfantry;
            
            // if there was a revive experience event, but lastKilledByPlayer is null, then the player was killed via Outside Interference
            if (lastKilledByPlayer == null)
            {
                enemyActionType = ScrimActionType.OutsideInterference;
            }

            var enemyScoringResult = GetActionRulePoints(enemyActionType);
            var enemyPoints = enemyScoringResult.Points;

            //// Additional same-team check is becasue players could technically be on different
            //// factions but the same scrim team
            //var lastKilledByPlayer = revive.LastKilledByPlayer; // _teamsManager.GetLastKilledByPlayer(revive.RevivedPlayer.Id);
            //var lastDeathWasToEnemy = !_teamsManager.DoPlayersShareTeam(revive.RevivedPlayer, lastKilledByPlayer);

            var enemyReviveUpdate = new ScrimEventAggregate()
            {
                Points = enemyPoints,
                NetScore = enemyPoints,
                EnemyRevivesAllowed = (lastKilledByPlayer != null && lastDeathWasToEnemy) ? 1 : 0, //1,
                KillsUndoneByRevive = (lastKilledByPlayer != null && lastDeathWasToEnemy) ? 1 : 0
            };

            revivedUpdate.NetScore = -enemyPoints;

            if (lastKilledByPlayer != null)
            {
                await _teamsManager.UpdatePlayerStats(lastKilledByPlayer.Id, enemyReviveUpdate);
            }
            else
            {
                var enemyTeamOrdinal = _teamsManager.GetEnemyTeamOrdinal(medicTeamOrdinal);
                _teamsManager.UpdateTeamStats(enemyTeamOrdinal, enemyReviveUpdate);
            }

            //var enemyTeamOrdinal = _teamsManager.GetEnemyTeamOrdinal(medicTeamOrdinal);
            //_teamsManager.UpdateTeamStats(enemyTeamOrdinal, enemyReviveUpdate);

            // Player Stats update automatically updates the appropriate team's stats
            await _teamsManager.UpdatePlayerStats(revive.MedicPlayer.Id, medicUpdate);
            await _teamsManager.UpdatePlayerStats(revive.RevivedPlayer.Id, revivedUpdate);

            return new ScrimEventScoringReviveResult(scoringResult, enemyScoringResult, enemyActionType, lastKilledByPlayer);

            //return scoringResult;
        }

        public async Task<ScrimEventScoringResult> ScoreAssistEvent(ScrimAssistActionEvent assist)
        {
            var actionType = assist.ActionType;
            var scoringResult = GetActionRulePoints(actionType);
            var points = scoringResult.Points;

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

            return scoringResult;
        }

        public async Task<ScrimEventScoringResult> ScoreObjectiveTickEvent(ScrimObjectiveTickActionEvent objective)
        {
            var actionType = objective.ActionType;
            var scoringResult = GetActionRulePoints(actionType);
            var points = scoringResult.Points;

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

            return scoringResult;
        }

        #endregion Experience Events

        #region Objective Events
        public ScrimEventScoringResult ScoreFacilityControlEvent(ScrimFacilityControlActionEvent control)
        {
            var teamOrdinal = control.ControllingTeamOrdinal;
            var type = control.ControlType;

            var actionType = control.ActionType;
            var scoringResult = GetActionRulePoints(actionType);
            var points = scoringResult.Points;

            var teamUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                BaseCaptures = (type == FacilityControlType.Capture ? 1 : 0),
                BaseDefenses = (type == FacilityControlType.Defense ? 1 : 0)
            };

            if (actionType == ScrimActionType.FirstBaseCapture)
            {
                teamUpdate.FirstCaptures = 1;
                teamUpdate.FirstCapturePoints = points;
            }
            else if (actionType == ScrimActionType.SubsequentBaseCapture)
            {
                teamUpdate.SubsequentCaptures = 1;
                teamUpdate.SubsequentCapturePoints = points;
            }

            _teamsManager.UpdateTeamStats(teamOrdinal, teamUpdate);
            
            return scoringResult;
        }

        public int? ScorePeriodicFacilityControlTick(int controllingTeamOrdinal)
        {
            var points = GetPeriodicControlPoints();
            if (!points.HasValue)
            {
                return null;
            }

            var teamUpdate = new ScrimEventAggregate()
            {
                Points = points.Value,
                PeriodicCaptureTicks = 1
            };

            var oldTeamPoints = _teamsManager.GetTeam(controllingTeamOrdinal)?.EventAggregateTracker.RoundStats.Points;

            _logger.LogInformation($"Adding {teamUpdate.Points} to team {controllingTeamOrdinal}");

            _teamsManager.UpdateTeamStats(controllingTeamOrdinal, teamUpdate);

            var newTeamPoints = _teamsManager.GetTeam(controllingTeamOrdinal)?.EventAggregateTracker.RoundStats.Points;

            _logger.LogInformation($"Team {controllingTeamOrdinal} points updated from {oldTeamPoints} => {newTeamPoints}");

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

        #region Rule Handling
        private ScrimEventScoringResult GetDeathOrDestructionEventPoints(ScrimActionType actionType, int? itemCategoryId, int? itemId, int? attackerLoadoutId)
        {
            /* Action Rules */
            var actionRule = GetActionRule(actionType);

            if (actionRule == null)
            {
                return new ScrimEventScoringResult(ScrimEventScorePointsSource.Default, 0, false);
            }
            
            if (!actionRule.DeferToItemCategoryRules || itemCategoryId == null)
            {
                return new ScrimEventScoringResult(ScrimEventScorePointsSource.ActionTypeRule, actionRule.Points, false);
            }

            /* Item Category Rules */
            var itemCategoryRule = GetItemCategoryRule((int)itemCategoryId);

            if (itemCategoryRule == null)
            {
                return new ScrimEventScoringResult(ScrimEventScorePointsSource.ActionTypeRule, actionRule.Points, false);
            }

            if (itemCategoryRule.IsBanned)
            {
                return new ScrimEventScoringResult(ScrimEventScorePointsSource.ItemCategoryRule, itemCategoryRule.Points, itemCategoryRule.IsBanned);
            }

            if (itemCategoryRule.DeferToPlanetsideClassSettings)
            {
                if (attackerLoadoutId == null)
                {
                    return new ScrimEventScoringResult(ScrimEventScorePointsSource.ItemCategoryRule, itemCategoryRule.Points, itemCategoryRule.IsBanned); // Placeholder. TO-DO: determine better backup
                }
                else
                {
                    return GetPlanetsideClassSettingPoints((int)attackerLoadoutId, itemCategoryRule.GetPlanetsideClassRuleSettings(), ScrimEventScorePointsSource.ItemCategoryRulePlanetsideClassSetting);
                }
            }

            if (!itemCategoryRule.DeferToItemRules || itemId == null)
            {
                return new ScrimEventScoringResult(ScrimEventScorePointsSource.ItemCategoryRule, itemCategoryRule.Points, itemCategoryRule.IsBanned);
            }

            /* Item Rules */
            var itemRule = GetItemRule((int)itemId);

            if (itemRule == null)
            {
                return new ScrimEventScoringResult(ScrimEventScorePointsSource.ItemCategoryRule, itemCategoryRule.Points, itemCategoryRule.IsBanned);
            }

            if (itemRule.DeferToPlanetsideClassSettings)
            {
                if (attackerLoadoutId == null)
                {
                    return new ScrimEventScoringResult(ScrimEventScorePointsSource.ItemRule, itemRule.Points, itemRule.IsBanned); // Placeholder. TO-DO: determine better backup
                }
                else
                {
                    return GetPlanetsideClassSettingPoints((int)attackerLoadoutId, itemRule.GetPlanetsideClassRuleSettings(), ScrimEventScorePointsSource.ItemRulePlanetsideClassSetting);
                }
            }

            return new ScrimEventScoringResult(ScrimEventScorePointsSource.ItemRule, itemRule.Points, itemRule.IsBanned);
        }

        private RulesetActionRule GetActionRule(ScrimActionType actionType)
        {
            return _activeRuleset.RulesetActionRules
                                    .Where(rule => rule.ScrimActionType == actionType)
                                    .FirstOrDefault();
        }

        private RulesetItemCategoryRule GetItemCategoryRule(int itemCategoryId)
        {
            return _activeRuleset.RulesetItemCategoryRules
                                    .Where(rule => rule.ItemCategoryId == itemCategoryId)
                                    .FirstOrDefault();
        }

        private RulesetItemRule GetItemRule(int itemId)
        {
            return _activeRuleset.RulesetItemRules
                                    .Where(rule => rule.ItemId == itemId)
                                    .FirstOrDefault();
        }

        private int? GetPeriodicControlPoints()
        {
            if (_periodicControlPointsValue.HasValue)
            {
                return _periodicControlPointsValue;
            }
            
            return _activeRuleset.PeriodicFacilityControlPoints;
        }

        private ScrimEventScoringResult GetActionRulePoints(ScrimActionType actionType)
        {
            var actionRule = _activeRuleset.RulesetActionRules
                                                .Where(rule => rule.ScrimActionType == actionType)
                                                .FirstOrDefault();

            if (actionRule == null)
            {
                return new ScrimEventScoringResult(ScrimEventScorePointsSource.Default, 0, false);

            }

            return new ScrimEventScoringResult(ScrimEventScorePointsSource.ActionTypeRule, actionRule.Points, false);
        }

        private ScrimEventScoringResult GetPlanetsideClassSettingPoints(int attackerLoadoutId, PlanetsideClassRuleSettings classSettings, ScrimEventScorePointsSource scoreSource)
        {
            var planetsideClass = PlanetsideClassLoadoutTranslator.GetPlanetsideClass(attackerLoadoutId);

            var isBanned = classSettings.GetClassIsBanned(planetsideClass);
            var points = classSettings.GetClassPoints(planetsideClass);

            return new ScrimEventScoringResult(scoreSource, points, isBanned);
        }
        #endregion Rule Handling
    }
}
