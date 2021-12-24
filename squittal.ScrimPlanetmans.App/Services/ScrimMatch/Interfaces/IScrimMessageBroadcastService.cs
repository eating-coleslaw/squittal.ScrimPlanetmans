using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
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

        event EventHandler<ScrimMessageEventArgs<TeamPlayerChangeMessage>> RaiseTeamPlayerChangeEvent;
        event EventHandler<ScrimMessageEventArgs<TeamOutfitChangeMessage>> RaiseTeamOutfitChangeEvent;
        event EventHandler<ScrimMessageEventArgs<TeamConstructedTeamChangeMessage>> RaiseTeamConstructedTeamChangeEvent;
        event EventHandler<ScrimMessageEventArgs<TeamAliasChangeMessage>> RaiseTeamAliasChangeEvent;
        event EventHandler<ScrimMessageEventArgs<TeamFactionChangeMessage>> RaiseTeamFactionChangeEvent;
        event EventHandler<ScrimMessageEventArgs<TeamLockStatusChangeMessage>> RaiseTeamLockStatusChangeEvent;

        event EventHandler<ScrimMessageEventArgs<PlayerNameDisplayChangeMessage>> RaisePlayerNameDisplayChangeEvent;

        event EventHandler<ScrimMessageEventArgs<PlayerStatUpdateMessage>> RaisePlayerStatUpdateEvent;
        event EventHandler<ScrimMessageEventArgs<TeamStatUpdateMessage>> RaiseTeamStatUpdateEvent;

        event EventHandler<ScrimMessageEventArgs<ScrimKillfeedEventMessage>> RaiseScrimKillfeedEvent;
        
        event EventHandler<ScrimMessageEventArgs<ScrimDeathActionEventMessage>> RaiseScrimDeathActionEvent;
        event EventHandler<ScrimMessageEventArgs<ScrimVehicleDestructionActionEventMessage>> RaiseScrimVehicleDestructionActionEvent;
        event EventHandler<ScrimMessageEventArgs<ScrimReviveActionEventMessage>> RaiseScrimReviveActionEvent;
        event EventHandler<ScrimMessageEventArgs<ScrimAssistActionEventMessage>> RaiseScrimAssistActionEvent;
        event EventHandler<ScrimMessageEventArgs<ScrimObjectiveTickActionEventMessage>> RaiseScrimObjectiveTickActionEvent;
        event EventHandler<ScrimMessageEventArgs<ScrimFacilityControlActionEventMessage>> RaiseScrimFacilityControlActionEvent;

        event EventHandler<ScrimMessageEventArgs<PlayerLoginMessage>> RaisePlayerLoginEvent;
        event EventHandler<ScrimMessageEventArgs<PlayerLogoutMessage>> RaisePlayerLogoutEvent;

        event EventHandler<ScrimMessageEventArgs<MatchStateUpdateMessage>> RaiseMatchStateUpdateEvent;
        event EventHandler<ScrimMessageEventArgs<MatchConfigurationUpdateMessage>> RaiseMatchConfigurationUpdateEvent;

        event EventHandler<ScrimMessageEventArgs<MatchTimerTickMessage>> RaiseMatchTimerTickEvent;
        event EventHandler<ScrimMessageEventArgs<PeriodicPointsTimerStateMessage>> RaisePeriodicPointsTimerTickEvent;

        event EventHandler<ScrimMessageEventArgs<ConstructedTeamMemberChangeMessage>> RaiseConstructedTeamMemberChangeEvent;
        event EventHandler<ScrimMessageEventArgs<ConstructedTeamInfoChangeMessage>> RaiseConstructedTeamInfoChangeEvent;

        event EventHandler<ScrimMessageEventArgs<ActiveRulesetChangeMessage>> RaiseActiveRulesetChangeEvent;
        event EventHandler<ScrimMessageEventArgs<RulesetSettingChangeMessage>> RaiseRulesetSettingChangeEvent;
        event EventHandler<ScrimMessageEventArgs<RulesetOverlayConfigurationChangeMessage>> RaiseRulesetOverlayConfigurationChangeEvent;
        event EventHandler<ScrimMessageEventArgs<RulesetRuleChangeMessage>> RaiseRulesetRuleChangeEvent;
        
        event EventHandler<ScrimMessageEventArgs<EndRoundCheckerMessage>> RaiseEndRoundCheckerMessage;


        void BroadcastSimpleMessage(string message);

        void BroadcastTeamPlayerChangeMessage(TeamPlayerChangeMessage message);
        void BroadcastTeamOutfitChangeMessage(TeamOutfitChangeMessage message);
        void BroadcastTeamConstructedTeamChangeMessage(TeamConstructedTeamChangeMessage message);
        void BroadcastTeamAliasChangeMessage(TeamAliasChangeMessage message);
        void BroadcastTeamFactionChangeMessage(TeamFactionChangeMessage message);
        void BroadcastTeamLockStatusChangeMessage(TeamLockStatusChangeMessage message);
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
        void BroadcastPeriodicPointsTimerTickMessage(PeriodicPointsTimerStateMessage message);

        void BroadcastConstructedTeamMemberChangeMessage(ConstructedTeamMemberChangeMessage message);
        void BroadcastConstructedTeamInfoChangeMessage(ConstructedTeamInfoChangeMessage message);

        void BroadcastActiveRulesetChangeMessage(ActiveRulesetChangeMessage message);
        void BroadcastRulesetSettingChangeMessage(RulesetSettingChangeMessage message);
        void BroadcastRulesetOverlayConfigurationChangeMessage(RulesetOverlayConfigurationChangeMessage message);
        void BroadcastRulesetRuleChangeMessage(RulesetRuleChangeMessage message);
        
        void BroadcastEndRoundCheckerMessage(EndRoundCheckerMessage message);

    }
}
