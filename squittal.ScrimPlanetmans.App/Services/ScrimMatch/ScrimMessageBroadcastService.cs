using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Events;
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

        public event EventHandler<PlayerStatUpdateEventArgs> RaisePlayerStatUpdateEvent;
        public delegate void PlayerStatUpdateMessageEventHandler(object sender, PlayerStatUpdateEventArgs e);

        public event EventHandler<ScrimDeathActionEventEventArgs> RaisePlayerScrimDeathEvent;
        public delegate void PlayerScrimDeathEventMessageEventHandler(object sender, ScrimDeathActionEventEventArgs e);

        public event EventHandler<PlayerLoginEventArgs> RaisePlayerLoginEvent;
        public delegate void PlayerLoginEventHandler(object sender, PlayerLoginEventArgs e);

        public event EventHandler<PlayerLogoutEventArgs> RaisePlayerLogoutEvent;
        public delegate void PlayerLogoutEventHandler(object sender, PlayerLogoutEventArgs e);

        public event EventHandler<MatchStateUpdateEventArgs> RaiseMatchStateUpdateEvent;

        public event EventHandler<MatchTimerTickEventArgs> RaiseMatchTimerTickEvent;

        public delegate void MatchTimerTickEventHandler(object sender, MatchTimerTickEventArgs e);

        public ScrimMessageBroadcastService(ILogger<ScrimMessageBroadcastService> logger)
        {
            _logger = logger;
        }

        /***********************
         * Match State Change
         ***********************/
        public void BroadcastMatchStateUpdateMessage(string message)
        {
            OnRaiseMatchStateUpdateEvent(new MatchStateUpdateEventArgs(message));
        }
        protected virtual void OnRaiseMatchStateUpdateEvent(MatchStateUpdateEventArgs e)
        {
            RaiseMatchStateUpdateEvent?.Invoke(this, e);
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

        public void BroadcastPlayerScrimDeathEventMessage(ScrimDeathActionEventMessage message)
        {
            OnRaisePlayerScrimDeathEvent(new ScrimDeathActionEventEventArgs(message));
        }
        protected virtual void OnRaisePlayerScrimDeathEvent(ScrimDeathActionEventEventArgs e)
        {
            RaisePlayerScrimDeathEvent?.Invoke(this, e);
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
    }
}
