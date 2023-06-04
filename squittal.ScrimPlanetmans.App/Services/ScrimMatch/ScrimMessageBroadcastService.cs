using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using System;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ScrimMessageBroadcastService : IScrimMessageBroadcastService
    {
        private readonly ILogger<ScrimMessageBroadcastService> _logger;

        public string LogFileName { get; set; }
        public bool IsLoggingEnabled { get; set; } = false;

        #region Handler Events & Delegates
        public event EventHandler<SimpleMessageEventArgs> RaiseSimpleMessageEvent;
        public delegate void SimpleMessageEventHandler(object sender, SimpleMessageEventArgs e);

        public event EventHandler<ScrimMessageEventArgs<TeamPlayerChangeMessage>> RaiseTeamPlayerChangeEvent;
        public delegate void TeamPlayerChangeEventHandler(object sender, ScrimMessageEventArgs<TeamPlayerChangeMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<TeamOutfitChangeMessage>> RaiseTeamOutfitChangeEvent;
        public delegate void TeamOutfitChangeEventHandler(object sender, ScrimMessageEventArgs<TeamOutfitChangeMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<TeamConstructedTeamChangeMessage>> RaiseTeamConstructedTeamChangeEvent;
        public delegate void TeamConstructedTeamChangeEventHandler(object sender, ScrimMessageEventArgs<TeamConstructedTeamChangeMessage> e);

        public event EventHandler<ScrimMessageEventArgs<TeamAliasChangeMessage>> RaiseTeamAliasChangeEvent;
        public delegate void TeamAliasChangeEventHandler(object sender, ScrimMessageEventArgs<TeamAliasChangeMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<TeamFactionChangeMessage>> RaiseTeamFactionChangeEvent;
        public delegate void TeamFactionChangeEventHandler(object sender, ScrimMessageEventArgs<TeamFactionChangeMessage> e);

        public event EventHandler<ScrimMessageEventArgs<TeamLockStatusChangeMessage>> RaiseTeamLockStatusChangeEvent;
        public delegate void TeamLockStatusChangeEventHandler(object sender, ScrimMessageEventArgs<TeamLockStatusChangeMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<PlayerNameDisplayChangeMessage>> RaisePlayerNameDisplayChangeEvent;
        public delegate void PlayerNameDisplayChangeEventHandler(object sender, ScrimMessageEventArgs<PlayerNameDisplayChangeMessage> e);


        public event EventHandler<ScrimMessageEventArgs<PlayerStatUpdateMessage>> RaisePlayerStatUpdateEvent;
        public delegate void PlayerStatUpdateMessageEventHandler(object sender, ScrimMessageEventArgs<PlayerStatUpdateMessage> e);

        public event EventHandler<ScrimMessageEventArgs<TeamStatUpdateMessage>> RaiseTeamStatUpdateEvent;
        public delegate void TeamStatUpdateMessageEventHandler(object sender, ScrimMessageEventArgs<TeamStatUpdateMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<ScrimKillfeedEventMessage>> RaiseScrimKillfeedEvent;
        public delegate void ScrimKillfeedEventMessageEventHandler(object sender, ScrimMessageEventArgs<ScrimKillfeedEventMessage> e);


        public event EventHandler<ScrimMessageEventArgs<ScrimDeathActionEventMessage>> RaiseScrimDeathActionEvent;
        public delegate void ScrimDeathActionEventMessageEventHandler(object sender, ScrimMessageEventArgs<ScrimDeathActionEventMessage> e);

        public event EventHandler<ScrimMessageEventArgs<ScrimVehicleDestructionActionEventMessage>> RaiseScrimVehicleDestructionActionEvent;
        public delegate void ScrimVehicleDestructionActionEventMessageEventHandler(object sender, ScrimMessageEventArgs<ScrimVehicleDestructionActionEventMessage> e);

        public event EventHandler<ScrimMessageEventArgs<ScrimReviveActionEventMessage>> RaiseScrimReviveActionEvent;
        public delegate void ScrimReviveActionEventMessageEventHandler(object sender, ScrimMessageEventArgs<ScrimReviveActionEventMessage> e);

        public event EventHandler<ScrimMessageEventArgs<ScrimAssistActionEventMessage>> RaiseScrimAssistActionEvent;
        public delegate void ScrimAssistActionEventMessageEventHandler(object sender, ScrimMessageEventArgs<ScrimAssistActionEventMessage> e);

        public event EventHandler<ScrimMessageEventArgs<ScrimObjectiveTickActionEventMessage>> RaiseScrimObjectiveTickActionEvent;
        public delegate void ScrimObjectiveTickActionEventMessageEventHandler(object sender, ScrimMessageEventArgs<ScrimObjectiveTickActionEventMessage> e);

        public event EventHandler<ScrimMessageEventArgs<ScrimFacilityControlActionEventMessage>> RaiseScrimFacilityControlActionEvent;
        public delegate void ScrimFacilityControlActionEventMessageEventHandler(object sender, ScrimMessageEventArgs<ScrimFacilityControlActionEventMessage> e);

        public event EventHandler<ScrimMessageEventArgs<PlayerLoginMessage>> RaisePlayerLoginEvent;
        public delegate void PlayerLoginEventHandler(object sender, ScrimMessageEventArgs<PlayerLoginMessage> e);

        public event EventHandler<ScrimMessageEventArgs<PlayerLogoutMessage>> RaisePlayerLogoutEvent;
        public delegate void PlayerLogoutEventHandler(object sender, ScrimMessageEventArgs<PlayerLogoutMessage> e);


        public event EventHandler<ScrimMessageEventArgs<MatchStateUpdateMessage>> RaiseMatchStateUpdateEvent;
        public delegate void MatchStateUpdateEventHandler(object sender, ScrimMessageEventArgs<MatchStateUpdateMessage> e);

        public event EventHandler<ScrimMessageEventArgs<MatchConfigurationUpdateMessage>> RaiseMatchConfigurationUpdateEvent;
        public delegate void MatchConfigurationUpdateEventHandler(object sender, ScrimMessageEventArgs<MatchConfigurationUpdateMessage> e);


        public event EventHandler<ScrimMessageEventArgs<ConstructedTeamMemberChangeMessage>> RaiseConstructedTeamMemberChangeEvent;
        public delegate void ConstructedTeamMemberChangeEventHandler(object sender, ScrimMessageEventArgs<ConstructedTeamMemberChangeMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<ConstructedTeamInfoChangeMessage>> RaiseConstructedTeamInfoChangeEvent;
        public delegate void ConstructedTeamInfoChangeEventHandler(object sender, ScrimMessageEventArgs<ConstructedTeamInfoChangeMessage> e);

        
        public event EventHandler<ScrimMessageEventArgs<ActiveRulesetChangeMessage>> RaiseActiveRulesetChangeEvent;
        public delegate void ActiveRulesetChangeMessageEventHandler(object sender, ScrimMessageEventArgs<ActiveRulesetChangeMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<RulesetSettingChangeMessage>> RaiseRulesetSettingChangeEvent;
        public delegate void RulesetSettingChangeMessageEventHandler(object sender, ScrimMessageEventArgs<RulesetSettingChangeMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<RulesetRuleChangeMessage>> RaiseRulesetRuleChangeEvent;
        public delegate void ActiveRulesetRuleChangeEventHandler(object sender, ScrimMessageEventArgs<RulesetRuleChangeMessage> e);
        
        public event EventHandler<ScrimMessageEventArgs<RulesetOverlayConfigurationChangeMessage>> RaiseRulesetOverlayConfigurationChangeEvent;

        public event EventHandler<ScrimMessageEventArgs<EndRoundCheckerMessage>> RaiseEndRoundCheckerMessage;
        public delegate void EndRoundCheckerMessageEventHandler(object sender, ScrimMessageEventArgs<EndRoundCheckerMessage> e);

        public delegate void RulesetOverlayConfigurationChangeEventHandler(object sender, ScrimMessageEventArgs<RulesetOverlayConfigurationChangeMessage> e);

        #endregion Handler Events & Delegates

        public ScrimMessageBroadcastService(ILogger<ScrimMessageBroadcastService> logger)
        {
            _logger = logger;
        }

        #region Logging
        public void DisableLogging()
        {
            IsLoggingEnabled = false;
        }
        public void EnableLogging()
        {
            IsLoggingEnabled = true;
        }

        public void SetLogFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            LogFileName = fileName;
        }


        private void TrySaveToLogFile(string message)
        {
            var timestamp = DateTime.Now.ToLongTimeString();

            if (IsLoggingEnabled && !string.IsNullOrWhiteSpace(LogFileName))
            {
                Task.Run( () =>
                {
                    LogFileWriter.WriteToLogFile(LogFileName, $"{timestamp}: {message}");
                });
            }
        }
        #endregion Loggin

        #region Match State Change
        public void BroadcastMatchStateUpdateMessage(MatchStateUpdateMessage message)
        {
            OnRaiseMatchStateUpdateEvent(new ScrimMessageEventArgs<MatchStateUpdateMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseMatchStateUpdateEvent(ScrimMessageEventArgs<MatchStateUpdateMessage> e)
        {
            RaiseMatchStateUpdateEvent?.Invoke(this, e);
        }

        public void BroadcastMatchConfigurationUpdateMessage(MatchConfigurationUpdateMessage message)
        {
            OnRaiseMatchConfigurationUpdateEvent(new ScrimMessageEventArgs<MatchConfigurationUpdateMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseMatchConfigurationUpdateEvent(ScrimMessageEventArgs<MatchConfigurationUpdateMessage> e)
        {
            RaiseMatchConfigurationUpdateEvent?.Invoke(this, e);
        }
        #endregion Match State Change

        #region Match Timer Tick
        public event EventHandler<ScrimMessageEventArgs<MatchTimerTickMessage>> RaiseMatchTimerTickEvent;
        public delegate void MatchTimerTickEventHandler(object sender, ScrimMessageEventArgs<MatchTimerTickMessage> e);

        public void BroadcastMatchTimerTickMessage(MatchTimerTickMessage message)
        {
            OnRaiseMatchTimerTickEvent(new ScrimMessageEventArgs<MatchTimerTickMessage>(message));
        }
        protected virtual void OnRaiseMatchTimerTickEvent(ScrimMessageEventArgs<MatchTimerTickMessage> e)
        {
            RaiseMatchTimerTickEvent?.Invoke(this, e);
        }
        #endregion Match Timer Tick

        #region Period Points Timer Tick
        public event EventHandler<ScrimMessageEventArgs<PeriodicPointsTimerStateMessage>> RaisePeriodicPointsTimerTickEvent;
        public delegate void PeriodicPointsTimerTickEventHandler(object sender, ScrimMessageEventArgs<PeriodicPointsTimerStateMessage> e);

        public void BroadcastPeriodicPointsTimerTickMessage(PeriodicPointsTimerStateMessage message)
        {
            OnRaisePeriodicPointsTimerTickEvent(new ScrimMessageEventArgs<PeriodicPointsTimerStateMessage>(message));
        }
        protected virtual void OnRaisePeriodicPointsTimerTickEvent(ScrimMessageEventArgs<PeriodicPointsTimerStateMessage> e)
        {
            RaisePeriodicPointsTimerTickEvent?.Invoke(this, e);
        }
        #endregion Period Points Timer Tick

        #region Player Login / Logout
        public void BroadcastPlayerLoginMessage(PlayerLoginMessage message)
        {
            OnRaisePlayerLoginEvent(new ScrimMessageEventArgs<PlayerLoginMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaisePlayerLoginEvent(ScrimMessageEventArgs<PlayerLoginMessage> e)
        {
            RaisePlayerLoginEvent?.Invoke(this, e);
        }

        public void BroadcastPlayerLogoutMessage(PlayerLogoutMessage message)
        {
            OnRaisePlayerLogoutEvent(new ScrimMessageEventArgs<PlayerLogoutMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaisePlayerLogoutEvent(ScrimMessageEventArgs<PlayerLogoutMessage> e)
        {
            RaisePlayerLogoutEvent?.Invoke(this, e);
        }
        #endregion Player Login / Logout

        #region Player Stat Update & Scrim Events
        public void BroadcastPlayerStatUpdateMessage(PlayerStatUpdateMessage message)
        {
            OnRaisePlayerStatUpdateEvent(new ScrimMessageEventArgs<PlayerStatUpdateMessage>(message));
        }
        protected virtual void OnRaisePlayerStatUpdateEvent(ScrimMessageEventArgs<PlayerStatUpdateMessage> e)
        {
            RaisePlayerStatUpdateEvent?.Invoke(this, e);
        }

        public void BroadcastTeamStatUpdateMessage(TeamStatUpdateMessage message)
        {
            OnRaiseTeamStatUpdateEvent(new ScrimMessageEventArgs<TeamStatUpdateMessage>(message));
        }
        protected virtual void OnRaiseTeamStatUpdateEvent(ScrimMessageEventArgs<TeamStatUpdateMessage> e)
        {
            RaiseTeamStatUpdateEvent?.Invoke(this, e);
        }
        
        public void BroadcastScrimKillfeedEventMessage(ScrimKillfeedEventMessage message)
        {
            OnRaiseScrimKillfeedEventEvent(new ScrimMessageEventArgs<ScrimKillfeedEventMessage>(message));
        }
        protected virtual void OnRaiseScrimKillfeedEventEvent(ScrimMessageEventArgs<ScrimKillfeedEventMessage> e)
        {
            RaiseScrimKillfeedEvent?.Invoke(this, e);
        }


        public void BroadcastScrimDeathActionEventMessage(ScrimDeathActionEventMessage message)
        {
            OnRaiseScrimDeathActionEvent(new ScrimMessageEventArgs<ScrimDeathActionEventMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimDeathActionEvent(ScrimMessageEventArgs<ScrimDeathActionEventMessage> e)
        {
            RaiseScrimDeathActionEvent?.Invoke(this, e);
        }
        
        public void BroadcastScrimVehicleDestructionActionEventMessage(ScrimVehicleDestructionActionEventMessage message)
        {
            OnRaiseScrimVehicleDestructionActionEvent(new ScrimMessageEventArgs<ScrimVehicleDestructionActionEventMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimVehicleDestructionActionEvent(ScrimMessageEventArgs<ScrimVehicleDestructionActionEventMessage> e)
        {
            RaiseScrimVehicleDestructionActionEvent?.Invoke(this, e);
        }
        
        public void BroadcastScrimReviveActionEventMessage(ScrimReviveActionEventMessage message)
        {
            OnRaiseScrimReviveActionEvent(new ScrimMessageEventArgs<ScrimReviveActionEventMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimReviveActionEvent(ScrimMessageEventArgs<ScrimReviveActionEventMessage> e)
        {
            RaiseScrimReviveActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimAssistActionEventMessage(ScrimAssistActionEventMessage message)
        {
            OnRaiseScrimAssistActionEvent(new ScrimMessageEventArgs<ScrimAssistActionEventMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimAssistActionEvent(ScrimMessageEventArgs<ScrimAssistActionEventMessage> e)
        {
            RaiseScrimAssistActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimObjectiveTickActionEventMessage(ScrimObjectiveTickActionEventMessage message)
        {
            OnRaiseScrimObjectiveTickActionEvent(new ScrimMessageEventArgs<ScrimObjectiveTickActionEventMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimObjectiveTickActionEvent(ScrimMessageEventArgs<ScrimObjectiveTickActionEventMessage> e)
        {
            RaiseScrimObjectiveTickActionEvent?.Invoke(this, e);
        }
        
        public void BroadcastScrimFacilityControlActionEventMessage(ScrimFacilityControlActionEventMessage message)
        {
            OnRaiseScrimFacilityControlActionEvent(new ScrimMessageEventArgs<ScrimFacilityControlActionEventMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimFacilityControlActionEvent(ScrimMessageEventArgs<ScrimFacilityControlActionEventMessage> e)
        {
            RaiseScrimFacilityControlActionEvent?.Invoke(this, e);
        }
        #endregion Player Stat Update & Scrim Events

        #region Simple (string) Message
        public void BroadcastSimpleMessage(string message)
        {
            OnRaiseSimpleMessageEvent(new SimpleMessageEventArgs(message));

            TrySaveToLogFile(message);
        }
        protected virtual void OnRaiseSimpleMessageEvent(SimpleMessageEventArgs e)
        {
            RaiseSimpleMessageEvent?.Invoke(this, e);
        }
        #endregion Simple (string) Message


        #region Team Player/Outfit/Constructed Team Changes
        public void BroadcastTeamPlayerChangeMessage(TeamPlayerChangeMessage message)
        {
            OnRaiseTeamPlayerChangeEvent(new ScrimMessageEventArgs<TeamPlayerChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamPlayerChangeEvent(ScrimMessageEventArgs<TeamPlayerChangeMessage> e)
        {
            RaiseTeamPlayerChangeEvent?.Invoke(this, e);
        }

        public void BroadcastTeamOutfitChangeMessage(TeamOutfitChangeMessage message)
        {
            OnRaiseTeamOutfitChangeEvent(new ScrimMessageEventArgs<TeamOutfitChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamOutfitChangeEvent(ScrimMessageEventArgs<TeamOutfitChangeMessage> e)
        {
            RaiseTeamOutfitChangeEvent?.Invoke(this, e);
        }
        
        public void BroadcastTeamConstructedTeamChangeMessage(TeamConstructedTeamChangeMessage message)
        {
            OnRaiseTeamConstructedTeamChangeEvent(new ScrimMessageEventArgs<TeamConstructedTeamChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamConstructedTeamChangeEvent(ScrimMessageEventArgs<TeamConstructedTeamChangeMessage> e)
        {
            RaiseTeamConstructedTeamChangeEvent?.Invoke(this, e);
        }
        #endregion Team Player/Outfit/Constructed Team Changes

        public void BroadcastTeamAliasChangeMessage(TeamAliasChangeMessage message)
        {
            OnRaiseTeamAliasChangeEvent(new ScrimMessageEventArgs<TeamAliasChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamAliasChangeEvent(ScrimMessageEventArgs<TeamAliasChangeMessage> e)
        {
            RaiseTeamAliasChangeEvent?.Invoke(this, e);
        }

        public void BroadcastTeamFactionChangeMessage(TeamFactionChangeMessage message)
        {
            OnRaiseTeamFactionChangeEvent(new ScrimMessageEventArgs<TeamFactionChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamFactionChangeEvent(ScrimMessageEventArgs<TeamFactionChangeMessage> e)
        {
            RaiseTeamFactionChangeEvent?.Invoke(this, e);
        }
        
        public void BroadcastTeamLockStatusChangeMessage(TeamLockStatusChangeMessage message)
        {
            OnRaiseTeamLockStatusChangeEvent(new ScrimMessageEventArgs<TeamLockStatusChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamLockStatusChangeEvent(ScrimMessageEventArgs<TeamLockStatusChangeMessage> e)
        {
            RaiseTeamLockStatusChangeEvent?.Invoke(this, e);
        }
        
        public void BroadcastPlayerNameDisplayChangeMessage(PlayerNameDisplayChangeMessage message)
        {
            OnRaisePlayerNameDisplayChangeEvent(new ScrimMessageEventArgs<PlayerNameDisplayChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaisePlayerNameDisplayChangeEvent(ScrimMessageEventArgs<PlayerNameDisplayChangeMessage> e)
        {
            RaisePlayerNameDisplayChangeEvent?.Invoke(this, e);
        }


        #region Constructed Team Messages
        public void BroadcastConstructedTeamMemberChangeMessage(ConstructedTeamMemberChangeMessage message)
        {
            OnRaiseConstructedTeamMemberChangeEvent(new ScrimMessageEventArgs<ConstructedTeamMemberChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseConstructedTeamMemberChangeEvent(ScrimMessageEventArgs<ConstructedTeamMemberChangeMessage> e)
        {
            RaiseConstructedTeamMemberChangeEvent?.Invoke(this, e);
        }
        
        public void BroadcastConstructedTeamInfoChangeMessage(ConstructedTeamInfoChangeMessage message)
        {
            OnRaiseConstructedTeamInfoChangeEvent(new ScrimMessageEventArgs<ConstructedTeamInfoChangeMessage>(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseConstructedTeamInfoChangeEvent(ScrimMessageEventArgs<ConstructedTeamInfoChangeMessage> e)
        {
            RaiseConstructedTeamInfoChangeEvent?.Invoke(this, e);
        }

        #endregion Constructed Team Messages

        #region Ruleset Messages
        public void BroadcastActiveRulesetChangeMessage(ActiveRulesetChangeMessage message)
        {
            OnRaiseActiveRulesetChangeEvent(new ScrimMessageEventArgs<ActiveRulesetChangeMessage>(message));
        }
        protected virtual void OnRaiseActiveRulesetChangeEvent(ScrimMessageEventArgs<ActiveRulesetChangeMessage> e)
        {
            RaiseActiveRulesetChangeEvent?.Invoke(this, e);
        }

        public void BroadcastRulesetSettingChangeMessage(RulesetSettingChangeMessage message)
        {
            OnRaiseRulesetSettingChangeEvent(new ScrimMessageEventArgs<RulesetSettingChangeMessage>(message));
        }
        protected virtual void OnRaiseRulesetSettingChangeEvent(ScrimMessageEventArgs<RulesetSettingChangeMessage> e)
        {
            RaiseRulesetSettingChangeEvent?.Invoke(this, e);
        }
        
        public void BroadcastRulesetOverlayConfigurationChangeMessage(RulesetOverlayConfigurationChangeMessage message)
        {
            OnRaiseRulesetOverlayConfigurationChangeEvent(new ScrimMessageEventArgs<RulesetOverlayConfigurationChangeMessage>(message));
        }
        protected virtual void OnRaiseRulesetOverlayConfigurationChangeEvent(ScrimMessageEventArgs<RulesetOverlayConfigurationChangeMessage> e)
        {
            RaiseRulesetOverlayConfigurationChangeEvent?.Invoke(this, e);
        }

        public void BroadcastRulesetRuleChangeMessage(RulesetRuleChangeMessage message)
        {
            OnRaiseRulesetRuleChangeEvent(new ScrimMessageEventArgs<RulesetRuleChangeMessage>(message));
        }
        protected virtual void OnRaiseRulesetRuleChangeEvent(ScrimMessageEventArgs<RulesetRuleChangeMessage> e)
        {
            RaiseRulesetRuleChangeEvent?.Invoke(this, e);
        }

        public void BroadcastEndRoundCheckerMessage(EndRoundCheckerMessage message)
        {
            OnRaiseEndRoundCheckerEvent(new ScrimMessageEventArgs<EndRoundCheckerMessage>(message));
        }
        protected virtual void OnRaiseEndRoundCheckerEvent(ScrimMessageEventArgs<EndRoundCheckerMessage> e)
        {
            RaiseEndRoundCheckerMessage?.Invoke(this, e);
        }
        #endregion Ruleset Messages
    }
}
