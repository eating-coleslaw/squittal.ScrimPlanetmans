using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using squittal.ScrimPlanetmans.Services.ScrimMatch;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    // Service only says whether an event would cause the match to end under 
    // the current ruleset, not whether the match *should* end. I.e. this service
    // does not track the matches state (whether it's paused, running, etc.)
    public class ScrimRoundEndCheckerService : IScrimRoundEndCheckerService
    {
        private readonly IScrimMessageBroadcastService _messageService;

        private Ruleset ActiveRuleset { get; set; }
        private MatchConfiguration MatchConfiguration { get; set; }

        public ScrimRoundEndCheckerService(IScrimMessageBroadcastService messageService)
        {
            _messageService = messageService;
            
            /* Messages to subscribe to:
             *  x Timer Ticks
             *  x Facility Capture Events
             *  x Team Points Update
             *  - Match Ruleset Change
             *  x MatchConfigurationUpdateMessage
             *  - (?) Match State Change
             */

            _messageService.RaiseMatchTimerTickEvent += OnMatchTimerTick;

            _messageService.RaiseScrimFacilityControlActionEvent += OnFacilityControlEvent;

            _messageService.RaiseMatchConfigurationUpdateEvent += ReceiveMatchConfigurationUpdateEvent;

            _messageService.RaiseTeamStatUpdateEvent += ReceiveTeamStatUpdateEvent;
        }

        private void ReceiveMatchConfigurationUpdateEvent(object sender, ScrimMessageEventArgs<MatchConfigurationUpdateMessage> e)
        {
            MatchConfiguration = e.Message.MatchConfiguration;
        }

        private void ReceiveTeamStatUpdateEvent(object sender, ScrimMessageEventArgs<TeamStatUpdateMessage> e)
        {
            if (!MatchConfiguration.EndRoundOnPointValueReached)
            {
                return;
            }    
            
            var message = e.Message;

            var teamOrdinal = message.Team.TeamOrdinal;
            var newPoints = message.Team.EventAggregate.Points;

            if (newPoints == MatchConfiguration.TargetPointValue)
            {
                BroadcastEndRoundMessage(EndRoundReason.PointTargetReached, teamOrdinal);
                return;
            }
        }

        private void OnMatchTimerTick(object sender, ScrimMessageEventArgs<MatchTimerTickMessage> e)
        {
            if (!MatchConfiguration.EnableRoundTimeLimit)
            {
                return;
            }

            var message = e.Message;

            //var status = e.Message.MatchTimerStatus;

            //var state = status.State;
            var state = message.State;

            //if (MatchConfiguration.RoundSecondsTotal == status.GetSecondsElapsed())
            if (MatchConfiguration.RoundSecondsTotal == message.SecondsElapsed)
            {
                BroadcastEndRoundMessage(EndRoundReason.TimeLimit, null);
                return;
            }

            //if (state == MatchTimerState.Stopped)
            if (state == TimerState.Stopped)
            {
                BroadcastEndRoundMessage(EndRoundReason.TimeLimit, null);
                return;
            }
        }

        private void OnFacilityControlEvent(object sender, ScrimMessageEventArgs<ScrimFacilityControlActionEventMessage> e)
        {
            if (!MatchConfiguration.EndRoundOnFacilityCapture)
            {
                return;
            }

            var message = e.Message;
            var controlEvent = message.FacilityControl;

            if (controlEvent.FacilityId == MatchConfiguration.FacilityId
                && controlEvent.WorldId == MatchConfiguration.WorldId)
            {
                BroadcastEndRoundMessage(EndRoundReason.FacilityCapture, controlEvent.ControllingTeamOrdinal);
            }
        }

        private void BroadcastEndRoundMessage(EndRoundReason reason, int? teamOrdinal)
        {
            var message = new EndRoundCheckerMessage()
            {
                EndRoundReason = reason,
                TeamOrdinal = teamOrdinal
            };

            _messageService.BroadcastEndRoundCheckerMessage(message);
        }
    }
}
