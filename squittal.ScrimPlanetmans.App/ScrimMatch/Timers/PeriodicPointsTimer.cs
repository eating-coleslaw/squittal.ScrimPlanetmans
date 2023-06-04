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


        public int PeriodSeconds { get; private set; } = Timeout.Infinite; //- 1;
        private int PeriodMilliseconds => PeriodSeconds == Timeout.Infinite ? Timeout.Infinite : PeriodSeconds * 1000;

        public DateTime? LastElapsedTime { get; private set; } = null;
        public DateTime? LastPausedTime { get; private set; } = null;
        public DateTime? LastResumedTime { get; private set; } = null;

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
            _logger.LogInformation($"Configuring Periodic Timer");

            _autoEvent.WaitOne();

            if (!CanConfigure())
            {
                _logger.LogInformation($"Failed to configure Periodic Timer: {Enum.GetName(typeof(TimerState), State)}");

                _autoEvent.Set();
                return;
            }

            State = TimerState.Configuring;

            LastElapsedTime = null;
            LastPausedTime = null;
            LastResumedTime = null;

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
            _timer = new Timer(HandleTick, _autoEvent, Timeout.Infinite, PeriodMilliseconds);

            State = TimerState.Configured;

            _logger.LogInformation($"Periodic Timer Configured");

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Start()
        {
            _logger.LogInformation($"Starting Periodic Timer");

            // Ensure timer can only be started once
            _autoEvent.WaitOne();
            if (IsRunning || State != TimerState.Configured)
            {
                _logger.LogInformation($"Failed to start Periodic Timer: {IsRunning} | {Enum.GetName(typeof(TimerState), State)}");

                _autoEvent.Set();
                return;
            }

            State = TimerState.Starting;

            LastElapsedTime = DateTime.UtcNow;
            LastPausedTime = null;
            LastResumedTime = null;

            // Immediately start the clock
            //_timer.Change(0, PeriodMilliseconds);
            _timer.Change(PeriodMilliseconds, PeriodMilliseconds);

            IsRunning = true;

            State = TimerState.Running;

            BroadcastEvent(false);

            _logger.LogInformation($"Periodic Timer Started");

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Stop()
        {
            _logger.LogInformation($"Stopping Periodic Timer");

            _autoEvent.WaitOne();

            if (State != TimerState.Stopping)
            {
                _logger.LogInformation($"Failed to stop Periodic Timer: timer not running");

                _autoEvent.Set();
                return;
            }

            LastElapsedTime = null;
            LastPausedTime = null;
            LastResumedTime = null;

            //_timer.Change(Timeout.Infinite, PeriodMilliseconds);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            IsRunning = false;

            State = TimerState.Stopped;

            BroadcastEvent(false);

            _logger.LogInformation($"Periodic Timer Stopped");

            _autoEvent.Set();
        }

        public void Pause()
        {
            _logger.LogInformation($"Pausing Periodic Timer");

            _autoEvent.WaitOne();
            if (State != TimerState.Running)
            {
                _logger.LogInformation($"Failed to pause Periodic Timer: timer not running");

                _autoEvent.Set();
                return;
            }

            IsRunning = false;
            State = TimerState.Pausing;

            //_timer.Change(Timeout.Infinite, PeriodMilliseconds);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            var now = DateTime.UtcNow;
            //_resumeDelayMs = PeriodMilliseconds - (int)(now - _prevTickTime).TotalMilliseconds;

            //_logger.LogInformation($"now: {now}");
            //_logger.LogInformation($"LastElapsedTime: {LastElapsedTime.Value}");
            //_logger.LogInformation($"PeriodMilliseconds: {PeriodMilliseconds}");

            _resumeDelayMs = PeriodMilliseconds - (int)(now - LastElapsedTime.Value).TotalMilliseconds;

            if (_resumeDelayMs < 0)
            {
                _resumeDelayMs = 0;
            }

            LastPausedTime = now;

            //IsRunning = false;
            State = TimerState.Paused;

            BroadcastEvent(false);

            _logger.LogInformation($"Periodic Timer Paused");

            _autoEvent.Set();
        }

        public void Resume()
        {
            _logger.LogInformation($"Resuming Periodic Timer");

            _autoEvent.WaitOne();
            if (State != TimerState.Paused)
            {
                _logger.LogInformation($"Failed to resume Periodic Timer: timer not paused");
                _autoEvent.Set();
                return;
            }

            State = TimerState.Resuming;

            LastResumedTime = DateTime.UtcNow;

            ////*******************************************
            //var currentTime = DateTime.UtcNow;

            //// Step 1: Current Time - Last Tick Time
            //var millisecondsFromLastElapsed = (int)(currentTime - LastElapsedTime.Value).TotalMilliseconds;

            //// Step 2: Get pause delay, if there is one
            //int pauseDelayMilliseconds = 0;

            //if (LastPausedTime.HasValue && LastResumedTime.HasValue)
            //{
            //    pauseDelayMilliseconds = (int)(LastResumedTime.Value - LastPausedTime.Value).TotalMilliseconds;
            //}

            //var updateDelayMilliseconds = PeriodMilliseconds - (millisecondsFromLastElapsed - pauseDelayMilliseconds);

            ////if (updateDelayMilliseconds < Timeout.Infinite)
            ////{
            ////    updateDelayMilliseconds = 0;
            ////}
            ////*******************************************

            //_logger.LogInformation($"_resumeDelayMs: {_resumeDelayMs}");

            _timer.Change(_resumeDelayMs, PeriodMilliseconds);
            //_timer.Change(updateDelayMilliseconds, PeriodMilliseconds);

            IsRunning = false;

            State = TimerState.Running;

            BroadcastEvent(false);

            _logger.LogInformation($"Periodic Timer Resumed");

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Reset()
        {
            _logger.LogInformation($"Reseting Periodic Timer");

            if (!CanReset())
            {
                _logger.LogInformation($"Failed to reset Periodic Timer: {IsRunning} | {Enum.GetName(typeof(TimerState), State)}");
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

            LastElapsedTime = null;
            LastPausedTime = null;
            LastResumedTime = null;

            _logger.LogInformation($"Periodic Timer Reset");
        }

        public void Restart()
        {
            _logger.LogInformation("Restarting Periodic Timer");
            
            _autoEvent.WaitOne();
            if (!CanRestart())
            {
                _logger.LogInformation($"Failed to restart Periodic Timer: {IsRunning} | {Enum.GetName(typeof(TimerState), State)}");

                _autoEvent.Set();
                return;
            }

            State = TimerState.Restarting;

            LastElapsedTime = DateTime.UtcNow;
            LastPausedTime = null;
            LastResumedTime = null;

            // Immediately start the clock
            //_timer.Change(0, PeriodMilliseconds);
            _timer.Change(PeriodMilliseconds, PeriodMilliseconds);

            IsRunning = true;

            State = TimerState.Running;

            BroadcastEvent(false);

            _logger.LogInformation($"Periodic Timer Restarted");

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Halt()
        {
            _logger.LogInformation($"Halting Periodic Timer");

            _autoEvent.WaitOne();

            if (!CanHalt())
            {
                _logger.LogInformation($"Failed to halt Periodic Timer: {Enum.GetName(typeof(TimerState), State)}");

                _autoEvent.Set();
                return;
            }

            State = TimerState.Halting;

            //_timer.Change(Timeout.Infinite, PeriodMilliseconds);
            _timer.Change(Timeout.Infinite,  Timeout.Infinite);

            LastElapsedTime = null;
            LastPausedTime = null;
            LastResumedTime = null;

            IsRunning = false;

            State = TimerState.Stopped;

            BroadcastEvent(false);

            _logger.LogInformation($"Periodic Timer Halted");

            _autoEvent.Set();
        }

        private void HandleTick(object stateInfo)
        {
            _autoEvent.WaitOne();

            if (ShouldProcessTick())
            {
                _prevTickTime = DateTime.UtcNow;

                LastElapsedTime = _prevTickTime;
                LastPausedTime = null;
                LastResumedTime = null;

                _autoEvent.Set();

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
                TimerState.Pausing => false,
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
                TimerState.Pausing => false,
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
                TimerState.Pausing => false,
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
                TimerState.Pausing => false,
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
                TimerState.Pausing => false,
                _ => false,
            };
        }
    }
}
