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

        //private int _secondsMax = 900;
        //private int _secondsRemaining;
        //private int _secondsElapsed = 0;

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        private Timer _timer;
        public TimeTracker Tracker { get; private set; } = new TimeTracker();
        public TimerState State { get; private set; }
        //private MatchTimerStatus Status { get; set; } = new MatchTimerStatus();

        public bool IsRunning { get; private set; } = false;

        // For handling pauses
        private DateTime _prevTickTime;
        private int _resumeDelayMs;

        //public event EventHandler<ScrimMessageEventArgs<MatchTimerTickMessage>> RaiseMatchTimerTickEvent;
        //public delegate void MatchTimerTickEventHandler(object sender, ScrimMessageEventArgs<MatchTimerTickMessage> e);



        public StatefulTimer(IScrimMessageBroadcastService messageService, ILogger<StatefulTimer> logger)
        {
            _messageService = messageService;
            _logger = logger;

            State = TimerState.Initialized;
            //Status.State = MatchTimerState.Initialized;
        }

        public void Configure(TimeSpan? timeSpan)
        {
            _logger.LogInformation($"Configuring Timer");

            _autoEvent.WaitOne();
            if (!CanConfigureTimer())
            {
                //_logger.LogInformation($"Failed to configure timer: {Enum.GetName(typeof(MatchTimerState), Status.State)}");
                _logger.LogInformation($"Failed to configure timer: {Enum.GetName(typeof(TimerState), State)}");

                _autoEvent.Set();
                return;
            }

            State = TimerState.Configuring;
            //Status.State = MatchTimerState.Configuring;

            if (timeSpan.HasValue)
            {
                //_secondsMax = (int)Math.Round((decimal)timeSpan.Value.TotalSeconds, 0);
                var secondsMax = (int)Math.Round((decimal)timeSpan.Value.TotalSeconds, 0);

                //Status.ConfigureTimer(_secondsMax);
                Tracker.Configure(secondsMax);
            }
            else
            {
                Tracker.Configure(null);
            }


            // TODO: move Timer instantiation to the Start() method?
            _timer?.Dispose(); // TODO: is this Dispose necessary?
            _timer = new Timer(HandleTick, _autoEvent, Timeout.Infinite, 1000);

            State = TimerState.Configured;
            //Status.State = MatchTimerState.Configured;

            //_logger.LogInformation($"Timer Configured: {_secondsMax} seconds | {Status.GetSecondsRemaining()} remaining | {Status.GetSecondsElapsed()} elapsed");
            
            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Start()
        {
            _logger.LogInformation($"Starting Timer");
            // Ensure timer can only be started once
            _autoEvent.WaitOne();
            //if (IsRunning || Status.State != MatchTimerState.Configured)
            if (IsRunning || State != TimerState.Configured)
            {
                //_logger.LogInformation($"Failed to start timer: {IsRunning.ToString()} | {Enum.GetName(typeof(MatchTimerState), Status.State)}");
                _logger.LogInformation($"Failed to start timer: {IsRunning} | {Enum.GetName(typeof(TimerState), State)}");

                _autoEvent.Set();
                return;
            }

            State = TimerState.Starting;
            //Status.State = MatchTimerState.Starting;

            // Don't think this is necessary; WaitOne() should already do a Reset  Reset to block other threads, just in case "Start Match" is double-clicked or something
            //_autoEvent.Reset();

            // Immediately start the clock
            // TODO: should we create a new Timer instance instead of having just starting up the single instance?
            _timer.Change(0, 1000);

            IsRunning = true;
            State = TimerState.Running;
            //Status.State = MatchTimerState.Running;

            // TODO: broadcast a "MatchStateChange" of event of type "Started" here

            _logger.LogInformation($"Timer Started");

            // Signal the waiting thread
            _autoEvent.Set();

        }

        public void Stop()
        {
            _logger.LogInformation($"Stopping Timer");
            _autoEvent.WaitOne();

            //if (Status.State != MatchTimerState.Stopping)
            if (State != TimerState.Stopping)
            {
                _logger.LogInformation($"Failed to stop timer: timer not running");
                _autoEvent.Set();
                return;
            }

            // TODO: should we Dispose of the timer instead of putting it on hold indefinitely?
            _timer.Change(Timeout.Infinite, 1000);

            IsRunning = false;
            State = TimerState.Stopped;
            //Status.State = MatchTimerState.Stopped;

            //var message = new MatchTimerTickMessage(Status);
            var message = new MatchTimerTickMessage(this);

            // TODO: use both to make MatchTimer razor component self-contained?
            //OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));
            _messageService.BroadcastMatchTimerTickMessage(message);

            // TODO: broadcast a "MatchStateChange" of event of type "Round Ended" here

            _logger.LogInformation($"Timer Stopped");

            _autoEvent.Set();
        }

        public void Pause()
        {
            _logger.LogInformation($"Pausing Timer");

            _autoEvent.WaitOne();
            //if (Status.State != MatchTimerState.Running)
            if (State != TimerState.Running)
            {
                _logger.LogInformation($"Failed to pause timer: timer not running");
                _autoEvent.Set();
                return;
            }

            var now = DateTime.UtcNow;
            _resumeDelayMs = (int)(now - _prevTickTime).TotalMilliseconds;

            _timer.Change(Timeout.Infinite, 1000);

            IsRunning = false;
            State = TimerState.Paused;
            //Status.State = MatchTimerState.Paused;

            //var message = new MatchTimerTickMessage(Status);
            var message = new MatchTimerTickMessage(this);

            // TODO: use both to make MatchTimer razor component self-contained?
            //OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));
            _messageService.BroadcastMatchTimerTickMessage(message);

            // TODO: broadcast a "MatchStateChange" of event of type "Round Ended" here

            _logger.LogInformation($"Timer Paused");

            _autoEvent.Set();
        }

        public void Resume()
        {
            _logger.LogInformation($"Resuming Timer");

            _autoEvent.WaitOne();
            //if (Status.State != MatchTimerState.Paused)
            if (State != TimerState.Paused)
            {
                _logger.LogInformation($"Failed to resume timer: timer not paused");
                _autoEvent.Set();
                return;
            }

            State = TimerState.Resuming;
            //Status.State = MatchTimerState.Resuming;

            _timer.Change(_resumeDelayMs, 1000);

            IsRunning = true;
            State = TimerState.Running;
            //Status.State = MatchTimerState.Running;

            //var message = new MatchTimerTickMessage(Status);
            var message = new MatchTimerTickMessage(this);

            // TODO: use both to make MatchTimer razor component self-contained?
            //OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));
            _messageService.BroadcastMatchTimerTickMessage(message);

            // TODO: broadcast a "MatchStateChange" of event of type "Resumed" here

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Reset()
        {
            _logger.LogInformation($"Reseting timer");

            if (!CanResetTimer())
            {
                //_logger.LogInformation($"Failed to reset timer: {IsRunning.ToString()} | {Enum.GetName(typeof(MatchTimerState), Status.State)}");
                _logger.LogInformation($"Failed to reset timer: {IsRunning.ToString()} | {Enum.GetName(typeof(TimerState), State)}");
                return;
            }

            //Configure(TimeSpan.FromSeconds(_secondsMax));
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
            _autoEvent.WaitOne();

            if (!CanHaltTimer())
            {
                //_logger.LogInformation($"Failed to halt timer: {Enum.GetName(typeof(MatchTimerState), Status.State)}");
                _logger.LogInformation($"Failed to halt timer: {Enum.GetName(typeof(TimerState), State)}");
                _autoEvent.Set();
                return;
            }
            State = TimerState.Halting;
            //Status.State = MatchTimerState.Halting;

            _timer.Change(Timeout.Infinite, 1000);

            IsRunning = false;

            //_secondsRemaining = Status.ForceZeroRemaining();
            //_secondsElapsed = Status.ForceMaxElapsed();
            if (Tracker.HasTimeLimit)
            {
                Tracker.ForceZeroRemaining();
            }

            State = TimerState.Stopped;
            //Status.State = MatchTimerState.Stopped;

            //var message = new MatchTimerTickMessage(Status);
            var message = new MatchTimerTickMessage(this);

            // TODO: use both to make MatchTimer razor component self-contained?
            //OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));
            _messageService.BroadcastMatchTimerTickMessage(message);

            // TODO: broadcast a "MatchStateChange" of event of type "Round Ended" here

            _logger.LogInformation($"Timer Halted");

            _autoEvent.Set();
        }

        private void HandleTick(object stateInfo)
        {
            _logger.LogDebug($"Handling timer tick");

            _autoEvent.WaitOne();

            if (ShouldProcessTick())
            {
                _prevTickTime = DateTime.UtcNow;

                //_secondsRemaining = Status.DecrementRemaining();
                //_secondsElapsed = Status.IncrementElapsed();
                Tracker.StepForward();

                //Interlocked.Decrement(ref _secondsRemaining);
                //Status.SecondsRemaining = _secondsRemaining;

                //Interlocked.Increment(ref _secondsElapsed);
                //Status.SecondsElapsed = _secondsElapsed;

                //var message = new MatchTimerTickMessage(Status);
                var message = new MatchTimerTickMessage(this);

                //if (_secondsRemaining == 0)
                if (Tracker.SecondsRemaining.HasValue && Tracker.SecondsRemaining.Value == 0)
                {
                    State = TimerState.Stopping;
                    //Status.State = MatchTimerState.Stopping;

                    // Signal the waiting thread. Only a thread waiting in Stop() should be able to do anything due to _status.State
                    _autoEvent.Set();
                    Stop();

                    return;
                }

                // TODO: use both to make MatchTimer razor component self-contained?
                //OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));
                _messageService.BroadcastMatchTimerTickMessage(message);
            }

            // Signal the waiting thread
            _autoEvent.Set();
        }

        private bool ShouldProcessTick()
        {
            //return Status.State switch
            //{
            //    MatchTimerState.Running => true,
            //    MatchTimerState.Starting => false,
            //    MatchTimerState.Paused => false,
            //    MatchTimerState.Stopping => false,
            //    MatchTimerState.Stopped => false,
            //    MatchTimerState.Halting => false,
            //    MatchTimerState.Initialized => false,
            //    MatchTimerState.Resuming => false,
            //    MatchTimerState.Configuring => false,
            //    MatchTimerState.Configured => false,
            //    _ => false,
            //};
            
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
                _ => false,
            };
        }

        private bool CanConfigureTimer()
        {
            if (IsRunning)
            {
                return false;
            }
            
            //return Status.State switch
            //{
            //    MatchTimerState.Stopped => true,
            //    MatchTimerState.Initialized => true,
            //    MatchTimerState.Configured => true,
            //    MatchTimerState.Running => false,
            //    MatchTimerState.Starting => false,
            //    MatchTimerState.Paused => false,
            //    MatchTimerState.Stopping => false,
            //    MatchTimerState.Halting => false,
            //    MatchTimerState.Resuming => false,
            //    MatchTimerState.Configuring => false,
            //    _ => false,
            //};
            
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
                _ => false,
            };
        }

        private bool CanResetTimer()
        {
            if (IsRunning)
            {
                return false;
            }

            //return Status.State switch
            //{
            //    MatchTimerState.Stopped => true,
            //    MatchTimerState.Initialized => false,
            //    MatchTimerState.Configured => false,
            //    MatchTimerState.Running => false,
            //    MatchTimerState.Starting => false,
            //    MatchTimerState.Paused => false,
            //    MatchTimerState.Stopping => false,
            //    MatchTimerState.Halting => false,
            //    MatchTimerState.Resuming => false,
            //    MatchTimerState.Configuring => false,
            //    _ => false,
            //};
            
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
                _ => false,
            };
        }

        private bool CanHaltTimer()
        {
            //return Status.State switch
            //{
            //    MatchTimerState.Stopped => true,
            //    MatchTimerState.Running => true,
            //    MatchTimerState.Paused => true,
            //    MatchTimerState.Initialized => false,
            //    MatchTimerState.Configured => false,
            //    MatchTimerState.Starting => false,
            //    MatchTimerState.Stopping => false,
            //    MatchTimerState.Halting => false,
            //    MatchTimerState.Resuming => false,
            //    MatchTimerState.Configuring => false,
            //    _ => false,
            //};
            
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
                _ => false,
            };
        }

        //protected virtual void OnRaiseMatchTimerTickEvent(ScrimMessageEventArgs<MatchTimerTickMessage> e)
        //{
        //    RaiseMatchTimerTickEvent?.Invoke(this, e);
        //}


    }

    //public enum TimerDirection
    //{
    //    Down = 0,
    //    Up = 1
    //}
}
