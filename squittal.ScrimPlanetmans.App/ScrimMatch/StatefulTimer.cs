using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Timers;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class StatefulTimer : IStatefulTimer
    {
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<StatefulTimer> _logger;

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        private Timer _timer;
        public TimeTracker Tracker { get; private set; } = new TimeTracker();
        public TimerState State { get; private set; }

        public bool IsRunning { get; private set; } = false;

        // For handling pauses
        private DateTime _prevTickTime;
        private int _resumeDelayMs;


        public StatefulTimer(IScrimMessageBroadcastService messageService, ILogger<StatefulTimer> logger)
        {
            _messageService = messageService;
            _logger = logger;

            State = TimerState.Initialized;
        }

        public void Configure(TimeSpan? timeSpan)
        {
            _logger.LogInformation($"Configuring Timer");

            _autoEvent.WaitOne();

            if (!CanConfigureTimer())
            {
                _logger.LogInformation($"Failed to configure timer: {Enum.GetName(typeof(TimerState), State)}");

                _autoEvent.Set();
                return;
            }

            State = TimerState.Configuring;

            if (timeSpan.HasValue)
            {
                var secondsMax = (int)Math.Round((decimal)timeSpan.Value.TotalSeconds, 0);

                Tracker.Configure(secondsMax);
            }
            else
            {
                Tracker.Configure(null);
            }

            _timer?.Dispose(); // TODO: is this Dispose necessary?
            _timer = new Timer(HandleTick, _autoEvent, Timeout.Infinite, 1000);

            State = TimerState.Configured;
            
            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Start()
        {
            _logger.LogInformation($"Starting Timer");
            
            // Ensure timer can only be started once
            _autoEvent.WaitOne();

            if (IsRunning || State != TimerState.Configured)
            {
                _logger.LogInformation($"Failed to start timer: {IsRunning} | {Enum.GetName(typeof(TimerState), State)}");

                _autoEvent.Set();
                return;
            }

            State = TimerState.Starting;

            // Immediately start the clock
            _timer.Change(0, 1000);

            IsRunning = true;
            State = TimerState.Running;

            _logger.LogInformation($"Timer Started");

            // Signal the waiting thread
            _autoEvent.Set();

        }

        public void Stop()
        {
            _logger.LogInformation($"Stopping Timer");
            _autoEvent.WaitOne();

            if (State != TimerState.Stopping)
            {
                _logger.LogInformation($"Failed to stop timer: timer not running");
                _autoEvent.Set();
                return;
            }

            //_timer.Change(Timeout.Infinite, 1000);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            IsRunning = false;
            State = TimerState.Stopped;

            var message = new MatchTimerTickMessage(this);
            _messageService.BroadcastMatchTimerTickMessage(message);

            _logger.LogInformation($"Timer Stopped");

            _autoEvent.Set();
        }

        public void Pause()
        {
            _logger.LogInformation($"Pausing Timer");

            _autoEvent.WaitOne();
            if (State != TimerState.Running)
            {
                _logger.LogInformation($"Failed to pause timer: timer not running");
                _autoEvent.Set();
                return;
            }

            IsRunning = false;
            State = TimerState.Pausing;

            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            var now = DateTime.UtcNow;
            _resumeDelayMs = 1000 - (int)(now - _prevTickTime).TotalMilliseconds;

            if (_resumeDelayMs < 0)
            {
                _resumeDelayMs = 0;
            }

            State = TimerState.Paused;

            var message = new MatchTimerTickMessage(this);
            _messageService.BroadcastMatchTimerTickMessage(message);

            _logger.LogInformation($"Timer Paused");

            _autoEvent.Set();
        }

        public void Resume()
        {
            _logger.LogInformation($"Resuming Timer");
            _autoEvent.WaitOne();

            if (State != TimerState.Paused)
            {
                _logger.LogInformation($"Failed to resume timer: timer not paused");
                _autoEvent.Set();
                return;
            }

            State = TimerState.Resuming;

            _timer.Change(_resumeDelayMs, 1000);

            IsRunning = true;
            State = TimerState.Running;

            var message = new MatchTimerTickMessage(this);
            _messageService.BroadcastMatchTimerTickMessage(message);

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Reset()
        {
            _logger.LogInformation($"Reseting timer");

            if (!CanResetTimer())
            {
                _logger.LogInformation($"Failed to reset timer: {IsRunning.ToString()} | {Enum.GetName(typeof(TimerState), State)}");
                return;
            }

            if (Tracker.SecondsMax.HasValue)
            {
                Configure(TimeSpan.FromSeconds(Tracker.SecondsMax.Value));
            }
            else
            {
                Configure(null);
            }

            _logger.LogInformation($"Timer reset");
        }

        public void Halt()
        {
            _logger.LogInformation($"Halting Timer");

            //_logger.LogInformation($"Halting timer current state: {Enum.GetName(typeof(TimerState), State)}");

            _autoEvent.WaitOne();

            //_logger.LogInformation($"Finished waiting for timer autoEvent");

            if (!CanHaltTimer())
            {
                _logger.LogInformation($"Failed to halt timer: {Enum.GetName(typeof(TimerState), State)}");
                _autoEvent.Set();
                return;
            }

            State = TimerState.Halting;

            _timer.Change(Timeout.Infinite, 1000);

            IsRunning = false;

            if (Tracker.HasTimeLimit)
            {
                Tracker.ForceZeroRemaining();
            }

            State = TimerState.Stopped;

            var message = new MatchTimerTickMessage(this);
            _messageService.BroadcastMatchTimerTickMessage(message);

            _logger.LogInformation($"Timer Halted");

            _autoEvent.Set();
        }

        private void HandleTick(object stateInfo)
        {
            //_logger.LogDebug($"Handling timer tick");
            //_logger.LogInformation($"Handling timer tick");

            _autoEvent.WaitOne();

            if (ShouldProcessTick())
            {
                _prevTickTime = DateTime.UtcNow;

                Tracker.StepForward();

                var message = new MatchTimerTickMessage(this);

                /*
                if (Tracker.SecondsRemaining.HasValue && Tracker.SecondsRemaining.Value == 0)
                {
                    State = TimerState.Stopping;

                    // Signal the waiting thread. Only a thread waiting in Stop() should be able to do anything due to _status.State
                    _autoEvent.Set();
                    Stop();

                    return;
                }
                */
                
                // Signal the waiting thread
                _autoEvent.Set();

                _messageService.BroadcastMatchTimerTickMessage(message);
            }

            // Signal the waiting thread
            _autoEvent.Set();
        }

        private bool ShouldProcessTick()
        {
            return State switch
            {
                TimerState.Running => true,
                TimerState.Starting => false,
                TimerState.Paused => false,
                TimerState.Stopping => false,
                TimerState.Stopped => false,
                TimerState.Halting => false,
                TimerState.Initialized => false,
                TimerState.Resuming => false,
                TimerState.Configuring => false,
                TimerState.Configured => false,
                TimerState.Pausing => false,
                _ => false,
            };
        }

        private bool CanConfigureTimer()
        {
            if (IsRunning)
            {
                return false;
            }
            
            return State switch
            {
                TimerState.Stopped => true,
                TimerState.Initialized => true,
                TimerState.Configured => true,
                TimerState.Running => false,
                TimerState.Starting => false,
                TimerState.Paused => false,
                TimerState.Stopping => false,
                TimerState.Halting => false,
                TimerState.Resuming => false,
                TimerState.Configuring => false,
                TimerState.Pausing => false,
                _ => false,
            };
        }

        private bool CanResetTimer()
        {
            if (IsRunning)
            {
                return false;
            }

            return State switch
            {
                TimerState.Stopped => true,
                TimerState.Initialized => false,
                TimerState.Configured => false,
                TimerState.Running => false,
                TimerState.Starting => false,
                TimerState.Paused => false,
                TimerState.Stopping => false,
                TimerState.Halting => false,
                TimerState.Resuming => false,
                TimerState.Configuring => false,
                TimerState.Pausing => false,
                _ => false,
            };
        }

        private bool CanHaltTimer()
        {
            return State switch
            {
                TimerState.Stopped => true,
                TimerState.Running => true,
                TimerState.Paused => true,
                TimerState.Initialized => false,
                TimerState.Configured => false,
                TimerState.Starting => false,
                TimerState.Stopping => false,
                TimerState.Halting => false,
                TimerState.Resuming => false,
                TimerState.Configuring => false,
                TimerState.Pausing => false,
                _ => false,
            };
        }
    }
}
