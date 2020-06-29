using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using System;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public interface IScrimMessageBroadcastService
    {
        string LogFileName { get; set; }
        bool IsLoggingEnabled { get; set; }
        void DisableLogging();
        void EnableLogging();
        void SetLogFileName(string fileName);

        event EventHandler<SimpleMessageEventArgs> RaiseSimpleMessageEvent;

        event EventHandler<TeamPlayerChangeEventArgs> RaiseTeamPlayerChangeEvent;
        event EventHandler<TeamOutfitChangeEventArgs> RaiseTeamOutfitChangeEvent;
        event EventHandler<TeamConstructedTeamChangeEventArgs> RaiseTeamConstructedTeamChangeEvent;
        event EventHandler<TeamAliasChangeEventArgs> RaiseTeamAliasChangeEvent;
        event EventHandler<TeamFactionChangeEventArgs> RaiseTeamFactionChangeEvent;

        event EventHandler<PlayerNameDisplayChangeEventArgs> RaisePlayerNameDisplayChangeEvent;

        event EventHandler<PlayerStatUpdateEventArgs> RaisePlayerStatUpdateEvent;
        event EventHandler<TeamStatUpdateEventArgs> RaiseTeamStatUpdateEvent;

        event EventHandler<ScrimKillfeedEventEventArgs> RaiseScrimKillfeedEvent;
        
        event EventHandler<ScrimDeathActionEventEventArgs> RaiseScrimDeathActionEvent;
        event EventHandler<ScrimVehicleDestructionActionEventEventArgs> RaiseScrimVehicleDestructionActionEvent;
        event EventHandler<ScrimReviveActionEventEventArgs> RaiseScrimReviveActionEvent;
        event EventHandler<ScrimAssistActionEventEventArgs> RaiseScrimAssistActionEvent;
        event EventHandler<ScrimObjectiveTickActionEventEventArgs> RaiseScrimObjectiveTickActionEvent;
        event EventHandler<ScrimFacilityControlActionEventEventArgs> RaiseScrimFacilityControlActionEvent;

        event EventHandler<PlayerLoginEventArgs> RaisePlayerLoginEvent;
        event EventHandler<PlayerLogoutEventArgs> RaisePlayerLogoutEvent;

        event EventHandler<MatchStateUpdateEventArgs> RaiseMatchStateUpdateEvent;
        event EventHandler<MatchConfigurationUpdateEventArgs> RaiseMatchConfigurationUpdateEvent;

        event EventHandler<MatchTimerTickEventArgs> RaiseMatchTimerTickEvent;
        
        event EventHandler<ConstructedTeamMemberChangeEventArgs> RaiseConstructedTeamMemberChangeEvent;
        event EventHandler<ConstructedTeamInfoChangeEventArgs> RaiseConstructedTeamInfoChangeEvent;

        void BroadcastSimpleMessage(string message);

        void BroadcastTeamPlayerChangeMessage(TeamPlayerChangeMessage message);
        void BroadcastTeamOutfitChangeMessage(TeamOutfitChangeMessage message);
        void BroadcastTeamConstructedTeamChangeMessage(TeamConstructedTeamChangeMessage message);
        void BroadcastTeamAliasChangeMessage(TeamAliasChangeMessage message);
        void BroadcastTeamFactionChangeMessage(TeamFactionChangeMessage message);
        void BroadcastPlayerStatUpdateMessage(PlayerStatUpdateMessage message);
        void BroadcastTeamStatUpdateMessage(TeamStatUpdateMessage message);

        void BroadcastPlayerNameDisplayChangeMessage(PlayerNameDisplayChangeMessage message);

        void BroadcastScrimKillfeedEventMessage(ScrimKillfeedEventMessage message);

        void BroadcastScrimDeathActionEventMessage(ScrimDeathActionEventMessage message);
        void BroadcastScrimVehicleDestructionActionEventMessage(ScrimVehicleDestructionActionEventMessage message);
        void BroadcastScrimReviveActionEventMessage(ScrimReviveActionEventMessage message);
        void BroadcastScrimAssistActionEventMessage(ScrimAssistActionEventMessage message);
        void BroadcastScrimObjectiveTickActionEventMessage(ScrimObjectiveTickActionEventMessage message);
        void BroadcastScrimFacilityControlActionEventMessage(ScrimFacilityControlActionEventMessage message);

        void BroadcastPlayerLoginMessage(PlayerLoginMessage message);
        void BroadcastPlayerLogoutMessage(PlayerLogoutMessage message);

        void BroadcastMatchStateUpdateMessage(MatchStateUpdateMessage message);
        void BroadcastMatchConfigurationUpdateMessage(MatchConfigurationUpdateMessage message);

        void BroadcastMatchTimerTickMessage(MatchTimerTickMessage message);

        void BroadcastConstructedTeamMemberChangeMessage(ConstructedTeamMemberChangeMessage message);
        void BroadcastConstructedTeamInfoChangeMessage(ConstructedTeamInfoChangeMessage message);
    }
}
