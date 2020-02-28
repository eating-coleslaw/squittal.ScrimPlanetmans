using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using System;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IScrimMessageBroadcastService
    {
        event EventHandler<SimpleMessageEventArgs> RaiseSimpleMessageEvent;

        event EventHandler<TeamPlayerChangeEventArgs> RaiseTeamPlayerChangeEvent;
        event EventHandler<PlayerStatUpdateEventArgs> RaisePlayerStatUpdateEvent;
        
        event EventHandler<ScrimDeathActionEventEventArgs> RaiseScrimDeathActionEvent;
        event EventHandler<ScrimReviveActionEventEventArgs> RaiseScrimReviveActionEvent;
        event EventHandler<ScrimAssistActionEventEventArgs> RaiseScrimAssistActionEvent;
        event EventHandler<ScrimObjectiveTickActionEventEventArgs> RaiseScrimObjectiveTickActionEvent;

        event EventHandler<PlayerLoginEventArgs> RaisePlayerLoginEvent;
        event EventHandler<PlayerLogoutEventArgs> RaisePlayerLogoutEvent;

        event EventHandler<MatchStateUpdateEventArgs> RaiseMatchStateUpdateEvent;

        event EventHandler<MatchTimerTickEventArgs> RaiseMatchTimerTickEvent;

        void BroadcastSimpleMessage(string message);

        void BroadcastTeamPlayerChangeMessage(TeamPlayerChangeMessage message);
        void BroadcastPlayerStatUpdateMessage(PlayerStatUpdateMessage message);

        void BroadcastScrimDeathActionEventMessage(ScrimDeathActionEventMessage message);
        void BroadcastScrimReviveActionEventMessage(ScrimReviveActionEventMessage message);
        void BroadcastScrimAssistActionEventMessage(ScrimAssistActionEventMessage message);
        void BroadcastScrimObjectiveTickActionEventMessage(ScrimObjectiveTickActionEventMessage message);

        void BroadcastPlayerLoginMessage(PlayerLoginMessage message);
        void BroadcastPlayerLogoutMessage(PlayerLogoutMessage message);

        void BroadcastMatchStateUpdateMessage(MatchStateUpdateMessage message); // TODO: implement MatchStateUpdateMessage class
        void BroadcastMatchTimerTickMessage(MatchTimerTickMessage message);


    }
}
