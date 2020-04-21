using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using System;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class ScrimMessageBroadcastService : IScrimMessageBroadcastService
    {
        private readonly ILogger<ScrimMessageBroadcastService> _logger;
        
        public event EventHandler<SimpleMessageEventArgs> RaiseSimpleMessageEvent;
        public delegate void SimpleMessageEventHandler(object sender, SimpleMessageEventArgs e);

        public event EventHandler<TeamPlayerChangeEventArgs> RaiseTeamPlayerChangeEvent;
        public delegate void TeamPlayerChangeEventHandler(object sender, TeamPlayerChangeEventArgs e);
        
        public event EventHandler<TeamOutfitChangeEventArgs> RaiseTeamOutfitChangeEvent;
        public delegate void TeamOutfitChangeEventHandler(object sender, TeamOutfitChangeEventArgs e);

        public event EventHandler<PlayerStatUpdateEventArgs> RaisePlayerStatUpdateEvent;
        public delegate void PlayerStatUpdateMessageEventHandler(object sender, PlayerStatUpdateEventArgs e);

        public event EventHandler<TeamStatUpdateEventArgs> RaiseTeamStatUpdateEvent;
        public delegate void TeamStatUpdateMessageEventHandler(object sender, TeamStatUpdateEventArgs e);

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

        public ScrimMessageBroadcastService(ILogger<ScrimMessageBroadcastService> logger)
        {
            _logger = logger;
        }

        /***********************
         * Match State Change
         ***********************/
        public void BroadcastMatchStateUpdateMessage(MatchStateUpdateMessage message)
        {
            OnRaiseMatchStateUpdateEvent(new MatchStateUpdateEventArgs(message));
        }
        protected virtual void OnRaiseMatchStateUpdateEvent(MatchStateUpdateEventArgs e)
        {
            RaiseMatchStateUpdateEvent?.Invoke(this, e);
        }

        public void BroadcastMatchConfigurationUpdateMessage(MatchConfigurationUpdateMessage message)
        {
            OnRaiseMatchConfigurationUpdateEvent(new MatchConfigurationUpdateEventArgs(message));
        }
        protected virtual void OnRaiseMatchConfigurationUpdateEvent(MatchConfigurationUpdateEventArgs e)
        {
            RaiseMatchConfigurationUpdateEvent?.Invoke(this, e);
        }

        /**********************
         *  Match Timer Tick
         **********************/
        public void BroadcastMatchTimerTickMessage(MatchTimerTickMessage message)
        {
            OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));
        }
        protected virtual void OnRaiseMatchTimerTickEvent(MatchTimerTickEventArgs e)
        {
            RaiseMatchTimerTickEvent?.Invoke(this, e);
        }

        /*************************
         * Player Login / Logout
         *************************/
        public void BroadcastPlayerLoginMessage(PlayerLoginMessage message)
        {
            OnRaisePlayerLoginEvent(new PlayerLoginEventArgs(message));
        }
        protected virtual void OnRaisePlayerLoginEvent(PlayerLoginEventArgs e)
        {
            RaisePlayerLoginEvent?.Invoke(this, e);
        }

        public void BroadcastPlayerLogoutMessage(PlayerLogoutMessage message)
        {
            OnRaisePlayerLogoutEvent(new PlayerLogoutEventArgs(message));
        }
        protected virtual void OnRaisePlayerLogoutEvent(PlayerLogoutEventArgs e)
        {
            RaisePlayerLogoutEvent?.Invoke(this, e);
        }

        /*************************************
         * Player Stat Update & Scrim Events
         *************************************/
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

        public void BroadcastScrimDeathActionEventMessage(ScrimDeathActionEventMessage message)
        {
            OnRaiseScrimDeathActionEvent(new ScrimDeathActionEventEventArgs(message));
        }
        protected virtual void OnRaiseScrimDeathActionEvent(ScrimDeathActionEventEventArgs e)
        {
            RaiseScrimDeathActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimVehicleDestructionActionEventMessage(ScrimVehicleDestructionActionEventMessage message)
        {
            OnRaiseScrimVehicleDestructionActionEvent(new ScrimVehicleDestructionActionEventEventArgs(message));
        }
        protected virtual void OnRaiseScrimVehicleDestructionActionEvent(ScrimVehicleDestructionActionEventEventArgs e)
        {
            RaiseScrimVehicleDestructionActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimReviveActionEventMessage(ScrimReviveActionEventMessage message)
        {
            OnRaiseScrimReviveActionEvent(new ScrimReviveActionEventEventArgs(message));
        }
        protected virtual void OnRaiseScrimReviveActionEvent(ScrimReviveActionEventEventArgs e)
        {
            RaiseScrimReviveActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimAssistActionEventMessage(ScrimAssistActionEventMessage message)
        {
            OnRaiseScrimAssistActionEvent(new ScrimAssistActionEventEventArgs(message));
        }
        protected virtual void OnRaiseScrimAssistActionEvent(ScrimAssistActionEventEventArgs e)
        {
            RaiseScrimAssistActionEvent?.Invoke(this, e);
        }

        public void BroadcastScrimObjectiveTickActionEventMessage(ScrimObjectiveTickActionEventMessage message)
        {
            OnRaiseScrimObjectiveTickActionEvent(new ScrimObjectiveTickActionEventEventArgs(message));
        }
        protected virtual void OnRaiseScrimObjectiveTickActionEvent(ScrimObjectiveTickActionEventEventArgs e)
        {
            RaiseScrimObjectiveTickActionEvent?.Invoke(this, e);
        }
        
        public void BroadcastScrimFacilityControlActionEventMessage(ScrimFacilityControlActionEventMessage message)
        {
            OnRaiseScrimFacilityControlActionEvent(new ScrimFacilityControlActionEventEventArgs(message));
        }
        protected virtual void OnRaiseScrimFacilityControlActionEvent(ScrimFacilityControlActionEventEventArgs e)
        {
            RaiseScrimFacilityControlActionEvent?.Invoke(this, e);
        }

        /***************************
         * Simple (string) Message
         ***************************/
        public void BroadcastSimpleMessage(string message)
        {
            OnRaiseSimpleMessageEvent(new SimpleMessageEventArgs(message));
        }
        protected virtual void OnRaiseSimpleMessageEvent(SimpleMessageEventArgs e)
        {
            RaiseSimpleMessageEvent?.Invoke(this, e);
        }

        /**********************
         * Team Player Change
         **********************/
        public void BroadcastTeamPlayerChangeMessage(TeamPlayerChangeMessage message)
        {
            OnRaiseTeamPlayerChangeEvent(new TeamPlayerChangeEventArgs(message));
        }
        protected virtual void OnRaiseTeamPlayerChangeEvent(TeamPlayerChangeEventArgs e)
        {
            RaiseTeamPlayerChangeEvent?.Invoke(this, e);
        }

        public void BroadcastTeamOutfitChangeMessage(TeamOutfitChangeMessage message)
        {
            OnRaiseTeamOutfitChangeEvent(new TeamOutfitChangeEventArgs(message));
        }
        protected virtual void OnRaiseTeamOutfitChangeEvent(TeamOutfitChangeEventArgs e)
        {
            RaiseTeamOutfitChangeEvent?.Invoke(this, e);
        }
    }
}
