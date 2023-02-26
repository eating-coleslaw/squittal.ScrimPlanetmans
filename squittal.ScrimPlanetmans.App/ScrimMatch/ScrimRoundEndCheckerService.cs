using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Models.ScrimEngine;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    // Service only says whether an event would cause the match to end under 
    // the current ruleset, not whether the match *should* end. I.e. this service
    // does not track the matches state (whether it's paused, running, etc.)
    public class ScrimRoundEndCheckerService : IScrimRoundEndCheckerService
    {
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<ScrimRoundEndCheckerService> _logger;

        //private Ruleset ActiveRuleset { get; set; }
        private MatchConfiguration MatchConfiguration { get; set; }

        private bool IsEnabled { get; set; } = false;

        public ScrimRoundEndCheckerService(IScrimMessageBroadcastService messageService, ILogger<ScrimRoundEndCheckerService> logger)
        {
            _messageService = messageService;
            _logger = logger;

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

        public void Enable()
        {
            IsEnabled = true;
            _logger.LogInformation($"ScrimRoundEndChecker Enabled");
        }

        public void Disable()
        {
            IsEnabled = false;
            _logger.LogInformation($"ScrimRoundEndChecker Disabled");
        }

        private void ReceiveMatchConfigurationUpdateEvent(object sender, ScrimMessageEventArgs<MatchConfigurationUpdateMessage> e)
        {
            MatchConfiguration = e.Message.MatchConfiguration;

            //_logger.LogInformation("Received MatchConfigurationUpdateEvent");
        }

        private void ReceiveTeamStatUpdateEvent(object sender, ScrimMessageEventArgs<TeamStatUpdateMessage> e)
        {
            if (!IsEnabled)
            {
                return;
            }
            
            if (!MatchConfiguration.EndRoundOnPointValueReached)
            {
                return;
            }    
            
            var message = e.Message;

            var teamOrdinal = message.Team.TeamOrdinal;
            var newPoints = message.Team.RoundEventAggregate.Points;

            //_logger.LogInformation($"Team {teamOrdinal} now has {newPoints} points");

            var countingUp = (MatchConfiguration.InitialPoints < MatchConfiguration.TargetPointValue);

            if (countingUp && newPoints >= MatchConfiguration.TargetPointValue)
            {
                BroadcastEndRoundMessage(EndRoundReason.PointTargetReached, teamOrdinal);
                return;
            }
            else if (!countingUp && newPoints <= MatchConfiguration.TargetPointValue)
            {
                BroadcastEndRoundMessage(EndRoundReason.PointTargetReached, teamOrdinal);
                return;
            }
        }

        private void OnMatchTimerTick(object sender, ScrimMessageEventArgs<MatchTimerTickMessage> e)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!MatchConfiguration.EnableRoundTimeLimit)
            {
                return;
            }

            var message = e.Message;

            var state = message.State;

            //_logger.LogInformation($"")

            if (MatchConfiguration.RoundSecondsTotal == message.SecondsElapsed)
            {
                BroadcastEndRoundMessage(EndRoundReason.TimeLimit, null);
                return;
            }

            if (state == TimerState.Stopped)
            {
                BroadcastEndRoundMessage(EndRoundReason.TimeLimit, null);
                return;
            }
        }

        private void OnFacilityControlEvent(object sender, ScrimMessageEventArgs<ScrimFacilityControlActionEventMessage> e)
        {
            if (!IsEnabled)
            {
                return;
            }

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
