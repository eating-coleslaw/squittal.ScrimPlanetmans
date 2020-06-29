using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
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

        public event EventHandler<TeamPlayerChangeEventArgs> RaiseTeamPlayerChangeEvent;
        public delegate void TeamPlayerChangeEventHandler(object sender, TeamPlayerChangeEventArgs e);
        
        public event EventHandler<TeamOutfitChangeEventArgs> RaiseTeamOutfitChangeEvent;
        public delegate void TeamOutfitChangeEventHandler(object sender, TeamOutfitChangeEventArgs e);
        
        public event EventHandler<TeamConstructedTeamChangeEventArgs> RaiseTeamConstructedTeamChangeEvent;
        public delegate void TeamConstructedTeamChangeEventHandler(object sender, TeamConstructedTeamChangeEventArgs e);

        public event EventHandler<TeamAliasChangeEventArgs> RaiseTeamAliasChangeEvent;
        public delegate void TeamAliasChangeEventHandler(object sender, TeamAliasChangeEventArgs e);
        
        public event EventHandler<PlayerNameDisplayChangeEventArgs> RaisePlayerNameDisplayChangeEvent;
        public delegate void PlayerNameDisplayChangeEventHandler(object sender, PlayerNameDisplayChangeEventArgs e);

        public event EventHandler<TeamFactionChangeEventArgs> RaiseTeamFactionChangeEvent;
        public delegate void TeamFactionChangeEventHandler(object sender, TeamFactionChangeEventArgs e);

        public event EventHandler<PlayerStatUpdateEventArgs> RaisePlayerStatUpdateEvent;
        public delegate void PlayerStatUpdateMessageEventHandler(object sender, PlayerStatUpdateEventArgs e);

        public event EventHandler<TeamStatUpdateEventArgs> RaiseTeamStatUpdateEvent;
        public delegate void TeamStatUpdateMessageEventHandler(object sender, TeamStatUpdateEventArgs e);
        
        public event EventHandler<ScrimKillfeedEventEventArgs> RaiseScrimKillfeedEvent;
        public delegate void ScrimKillfeedEventMessageEventHandler(object sender, ScrimKillfeedEventEventArgs e);

        public event EventHandler<ScrimDeathActionEventEventArgs> RaiseScrimDeathActionEvent;
        public delegate void ScrimDeathActionEventMessageEventHandler(object sender, ScrimDeathActionEventEventArgs e);

        public event EventHandler<ScrimVehicleDestructionActionEventEventArgs> RaiseScrimVehicleDestructionActionEvent;
        public delegate void ScrimVehicleDestructionActionEventMessageEventHandler(object sender, ScrimVehicleDestructionActionEventEventArgs e);

        public event EventHandler<ScrimReviveActionEventEventArgs> RaiseScrimReviveActionEvent;
        public delegate void ScrimReviveActionEventMessageEventHandler(object sender, ScrimReviveActionEventEventArgs e);

        public event EventHandler<ScrimAssistActionEventEventArgs> RaiseScrimAssistActionEvent;
        public delegate void ScrimAssistActionEventMessageEventHandler(object sender, ScrimAssistActionEventEventArgs e);

        public event EventHandler<ScrimObjectiveTickActionEventEventArgs> RaiseScrimObjectiveTickActionEvent;
        public delegate void ScrimObjectiveTickActionEventMessageEventHandler(object sender, ScrimObjectiveTickActionEventEventArgs e);

        public event EventHandler<ScrimFacilityControlActionEventEventArgs> RaiseScrimFacilityControlActionEvent;
        public delegate void ScrimFacilityControlActionEventMessageEventHandler(object sender, ScrimFacilityControlActionEventEventArgs e);

        public event EventHandler<PlayerLoginEventArgs> RaisePlayerLoginEvent;
        public delegate void PlayerLoginEventHandler(object sender, PlayerLoginEventArgs e);

        public event EventHandler<PlayerLogoutEventArgs> RaisePlayerLogoutEvent;
        public delegate void PlayerLogoutEventHandler(object sender, PlayerLogoutEventArgs e);


        public event EventHandler<MatchStateUpdateEventArgs> RaiseMatchStateUpdateEvent;
        public delegate void MatchStateUpdateEventHandler(object sender, MatchStateUpdateEventArgs e);

        public event EventHandler<MatchConfigurationUpdateEventArgs> RaiseMatchConfigurationUpdateEvent;
        public delegate void MatchConfigurationUpdateEventHandler(object sender, MatchConfigurationUpdateEventArgs e);

        public event EventHandler<MatchTimerTickEventArgs> RaiseMatchTimerTickEvent;
        public delegate void MatchTimerTickEventHandler(object sender, MatchTimerTickEventArgs e);
        
        public event EventHandler<ConstructedTeamMemberChangeEventArgs> RaiseConstructedTeamMemberChangeEvent;
        public delegate void ConstructedTeamMemberChangeEventHandler(object sender, ConstructedTeamMemberChangeEventArgs e);
        
        public event EventHandler<ConstructedTeamInfoChangeEventArgs> RaiseConstructedTeamInfoChangeEvent;
        public delegate void ConstructedTeamInfoChangeEventHandler(object sender, ConstructedTeamInfoChangeEventArgs e);

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
            OnRaiseMatchStateUpdateEvent(new MatchStateUpdateEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseMatchStateUpdateEvent(MatchStateUpdateEventArgs e)
        {
            RaiseMatchStateUpdateEvent?.Invoke(this, e);
        }

        public void BroadcastMatchConfigurationUpdateMessage(MatchConfigurationUpdateMessage message)
        {
            OnRaiseMatchConfigurationUpdateEvent(new MatchConfigurationUpdateEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseMatchConfigurationUpdateEvent(MatchConfigurationUpdateEventArgs e)
        {
            RaiseMatchConfigurationUpdateEvent?.Invoke(this, e);
        }
        #endregion Match State Change

        #region Match Timer Tick
        public void BroadcastMatchTimerTickMessage(MatchTimerTickMessage message)
        {
            OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));
        }
        protected virtual void OnRaiseMatchTimerTickEvent(MatchTimerTickEventArgs e)
        {
            RaiseMatchTimerTickEvent?.Invoke(this, e);
        }
        #endregion Match Timer Tick

        #region Player Login / Logout
        public void BroadcastPlayerLoginMessage(PlayerLoginMessage message)
        {
            OnRaisePlayerLoginEvent(new PlayerLoginEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaisePlayerLoginEvent(PlayerLoginEventArgs e)
        {
            RaisePlayerLoginEvent?.Invoke(this, e);
        }

        public void BroadcastPlayerLogoutMessage(PlayerLogoutMessage message)
        {
            OnRaisePlayerLogoutEvent(new PlayerLogoutEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaisePlayerLogoutEvent(PlayerLogoutEventArgs e)
        {
            RaisePlayerLogoutEvent?.Invoke(this, e);
        }
        #endregion Player Login / Logout

        #region Player Stat Update & Scrim Events
        public void BroadcastPlayerStatUpdateMessage(PlayerStatUpdateMessage message)
        {
            OnRaisePlayerStatUpdateEvent(new PlayerStatUpdateEventArgs(message));
        }
        protected virtual void OnRaisePlayerStatUpdateEvent(PlayerStatUpdateEventArgs e)
        {
            RaisePlayerStatUpdateEvent?.Invoke(this, e);
        }

        public void BroadcastTeamStatUpdateMessage(TeamStatUpdateMessage message)
        {
            OnRaiseTeamStatUpdateEvent(new TeamStatUpdateEventArgs(message));
        }
        protected virtual void OnRaiseTeamStatUpdateEvent(TeamStatUpdateEventArgs e)
        {
            RaiseTeamStatUpdateEvent?.Invoke(this, e);
        }
        
        public void BroadcastScrimKillfeedEventMessage(ScrimKillfeedEventMessage message)
        {
            OnRaiseScrimKillfeedEventEvent(new ScrimKillfeedEventEventArgs(message));
        }
        protected virtual void OnRaiseScrimKillfeedEventEvent(ScrimKillfeedEventEventArgs e)
        {
            RaiseScrimKillfeedEvent?.Invoke(this, e);
        }

        public void BroadcastScrimDeathActionEventMessage(ScrimDeathActionEventMessage message)
        {
            OnRaiseScrimDeathActionEvent(new ScrimDeathActionEventEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimDeathActionEvent(ScrimDeathActionEventEventArgs e)
        {
            RaiseScrimDeathActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimVehicleDestructionActionEventMessage(ScrimVehicleDestructionActionEventMessage message)
        {
            OnRaiseScrimVehicleDestructionActionEvent(new ScrimVehicleDestructionActionEventEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimVehicleDestructionActionEvent(ScrimVehicleDestructionActionEventEventArgs e)
        {
            RaiseScrimVehicleDestructionActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimReviveActionEventMessage(ScrimReviveActionEventMessage message)
        {
            OnRaiseScrimReviveActionEvent(new ScrimReviveActionEventEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimReviveActionEvent(ScrimReviveActionEventEventArgs e)
        {
            RaiseScrimReviveActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimAssistActionEventMessage(ScrimAssistActionEventMessage message)
        {
            OnRaiseScrimAssistActionEvent(new ScrimAssistActionEventEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimAssistActionEvent(ScrimAssistActionEventEventArgs e)
        {
            RaiseScrimAssistActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimObjectiveTickActionEventMessage(ScrimObjectiveTickActionEventMessage message)
        {
            OnRaiseScrimObjectiveTickActionEvent(new ScrimObjectiveTickActionEventEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimObjectiveTickActionEvent(ScrimObjectiveTickActionEventEventArgs e)
        {
            RaiseScrimObjectiveTickActionEvent?.Invoke(this, e);
        }
        
        public void BroadcastScrimFacilityControlActionEventMessage(ScrimFacilityControlActionEventMessage message)
        {
            OnRaiseScrimFacilityControlActionEvent(new ScrimFacilityControlActionEventEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseScrimFacilityControlActionEvent(ScrimFacilityControlActionEventEventArgs e)
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
            OnRaiseTeamPlayerChangeEvent(new TeamPlayerChangeEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamPlayerChangeEvent(TeamPlayerChangeEventArgs e)
        {
            RaiseTeamPlayerChangeEvent?.Invoke(this, e);
        }

        public void BroadcastTeamOutfitChangeMessage(TeamOutfitChangeMessage message)
        {
            OnRaiseTeamOutfitChangeEvent(new TeamOutfitChangeEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamOutfitChangeEvent(TeamOutfitChangeEventArgs e)
        {
            RaiseTeamOutfitChangeEvent?.Invoke(this, e);
        }
        
        public void BroadcastTeamConstructedTeamChangeMessage(TeamConstructedTeamChangeMessage message)
        {
            OnRaiseTeamConstructedTeamChangeEvent(new TeamConstructedTeamChangeEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamConstructedTeamChangeEvent(TeamConstructedTeamChangeEventArgs e)
        {
            RaiseTeamConstructedTeamChangeEvent?.Invoke(this, e);
        }
        #endregion Team Player/Outfit/Constructed Team Changes

        public void BroadcastTeamAliasChangeMessage(TeamAliasChangeMessage message)
        {
            OnRaiseTeamAliasChangeEvent(new TeamAliasChangeEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamAliasChangeEvent(TeamAliasChangeEventArgs e)
        {
            RaiseTeamAliasChangeEvent?.Invoke(this, e);
        }
        
        public void BroadcastPlayerNameDisplayChangeMessage(PlayerNameDisplayChangeMessage message)
        {
            OnRaisePlayerNameDisplayChangeEvent(new PlayerNameDisplayChangeEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaisePlayerNameDisplayChangeEvent(PlayerNameDisplayChangeEventArgs e)
        {
            RaisePlayerNameDisplayChangeEvent?.Invoke(this, e);
        }

        public void BroadcastTeamFactionChangeMessage(TeamFactionChangeMessage message)
        {
            OnRaiseTeamFactionChangeEvent(new TeamFactionChangeEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseTeamFactionChangeEvent(TeamFactionChangeEventArgs e)
        {
            RaiseTeamFactionChangeEvent?.Invoke(this, e);
        }

        #region Constructed Team Messages
        public void BroadcastConstructedTeamMemberChangeMessage(ConstructedTeamMemberChangeMessage message)
        {
            OnRaiseConstructedTeamMemberChangeEvent(new ConstructedTeamMemberChangeEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseConstructedTeamMemberChangeEvent(ConstructedTeamMemberChangeEventArgs e)
        {
            RaiseConstructedTeamMemberChangeEvent?.Invoke(this, e);
        }
        
        public void BroadcastConstructedTeamInfoChangeMessage(ConstructedTeamInfoChangeMessage message)
        {
            OnRaiseConstructedTeamInfoChangeEvent(new ConstructedTeamInfoChangeEventArgs(message));

            TrySaveToLogFile(message.Info);
        }
        protected virtual void OnRaiseConstructedTeamInfoChangeEvent(ConstructedTeamInfoChangeEventArgs e)
        {
            RaiseConstructedTeamInfoChangeEvent?.Invoke(this, e);
        }

        #endregion Constructed Team Messages
    }
}
