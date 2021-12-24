using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Threading;

namespace squittal.ScrimPlanetmans.ScrimMatch.Timers
{
    public class PeriodicPointsTimer : IPeriodicPointsTimer
    {
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<PeriodicPointsTimer> _logger;

        public string TimerName { get; set; }

        private Timer _timer;
        public TimerState State { get; private set; }
        public bool IsRunning { get; private set; } = false;

        public int PeriodSeconds { get; private set; } = -1;

        // For handling pauses
        private DateTime _prevTickTime;
        private int _resumeDelayMs;

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        public PeriodicPointsTimer(IScrimMessageBroadcastService messageService, ILogger<PeriodicPointsTimer> logger)
        {
            _messageService = messageService;
            _logger = logger;

            State = TimerState.Initialized;
        }

        public void Configure(TimeSpan? timeSpan)
        {
            _autoEvent.WaitOne();
            if (!CanConfigure())
            {
                _autoEvent.Set();
                return;
            }

            State = TimerState.Configuring;

            if (timeSpan.HasValue)
            {
                var periodSeconds = (int)Math.Round((decimal)timeSpan.Value.TotalSeconds, 0);

                PeriodSeconds = periodSeconds;
            }
            else
            {
                PeriodSeconds = Timeout.Infinite;
            }

            _timer?.Dispose(); // TODO: is this Dispose necessary?
            _timer = new Timer(HandleTick, _autoEvent, Timeout.Infinite, PeriodSeconds);

            State = TimerState.Configured;

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Start()
        {
            // Ensure timer can only be started once
            _autoEvent.WaitOne();
            if (IsRunning || State != TimerState.Configured)
            {
                _autoEvent.Set();
                return;
            }

            State = TimerState.Starting;

            // Immediately start the clock
            _timer.Change(0, PeriodSeconds);

            IsRunning = true;

            State = TimerState.Running;

            BroadcastEvent(false);

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Stop()
        {
            _autoEvent.WaitOne();

            if (State != TimerState.Stopping)
            {
                _autoEvent.Set();
                return;
            }

            _timer.Change(Timeout.Infinite, PeriodSeconds);

            IsRunning = false;

            State = TimerState.Stopped;

            BroadcastEvent(false);

            _autoEvent.Set();
        }

        public void Pause()
        {
            _autoEvent.WaitOne();
            if (State != TimerState.Running)
            {
                _autoEvent.Set();
                return;
            }

            var now = DateTime.UtcNow;
            _resumeDelayMs = (int)(now - _prevTickTime).TotalMilliseconds;

            _timer.Change(Timeout.Infinite, PeriodSeconds);

            IsRunning = false;

            State = TimerState.Paused;

            BroadcastEvent(false);

            _autoEvent.Set();
        }

        public void Resume()
        {
            _autoEvent.WaitOne();
            if (State != TimerState.Paused)
            {
                _autoEvent.Set();
                return;
            }

            State = TimerState.Resuming;

            _timer.Change(_resumeDelayMs, PeriodSeconds);

            IsRunning = false;

            State = TimerState.Running;

            BroadcastEvent(false);

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Reset()
        {
            if (!CanReset())
            {
                return;
            }

            if (PeriodSeconds != Timeout.Infinite)
            {
                Configure(TimeSpan.FromSeconds(PeriodSeconds));
            }
            else
            {
                Configure(Timeout.InfiniteTimeSpan);
            }
        }

        public void Restart()
        {
            _autoEvent.WaitOne();
            if (!CanRestart())
            {
                _autoEvent.Set();
                return;
            }

            State = TimerState.Restarting;

            // Immediately start the clock
            _timer.Change(0, PeriodSeconds);

            IsRunning = true;

            State = TimerState.Running;

            BroadcastEvent(false);

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Halt()
        {
            _autoEvent.WaitOne();

            if (!CanHalt())
            {
                _autoEvent.Set();
                return;
            }

            State = TimerState.Halting;

            _timer.Change(Timeout.Infinite, PeriodSeconds);

            IsRunning = false;

            State = TimerState.Stopped;

            BroadcastEvent(false);

            _autoEvent.Set();
        }

        private void HandleTick(object stateInfo)
        {
            _autoEvent.WaitOne();

            if (ShouldProcessTick())
            {
                _prevTickTime = DateTime.UtcNow;

                BroadcastEvent(true);
            }

            // Signal the waiting thread
            _autoEvent.Set();
        }

        private void BroadcastEvent(bool periodElapsed = false)
        {
            var message = new PeriodicPointsTimerStateMessage(this, periodElapsed);
            _messageService.BroadcastPeriodicPointsTimerTickMessage(message);
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
                TimerState.Restarting => false,
                _ => false,
            };
        }

        public bool CanStart()
        {
            if (IsRunning || State != TimerState.Configured)
            {
                return false;
            }

            return true;
        }
        private bool CanConfigure()
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
                TimerState.Restarting => false,
                _ => false,
            };
        }

        private bool CanReset()
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
                TimerState.Restarting => false,
                _ => false,
            };
        }

        private bool CanHalt()
        {
            return State switch
            {
                TimerState.Stopped => true,
                TimerState.Running => true,
                TimerState.Paused => true,
                TimerState.Restarting => true,

                TimerState.Initialized => false,
                TimerState.Configured => false,
                TimerState.Starting => false,
                TimerState.Stopping => false,
                TimerState.Halting => false,
                TimerState.Resuming => false,
                TimerState.Configuring => false,
                _ => false,
            };
        }

        private bool CanRestart()
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
                TimerState.Restarting => false,
                _ => false,
            };
        }
    }
}
