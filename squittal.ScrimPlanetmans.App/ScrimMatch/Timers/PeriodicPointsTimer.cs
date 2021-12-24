using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.ScrimMatch.Messages;
using squittal.ScrimPlanetmans.Services.ScrimMatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch.Timers
{
    public class PeriodicPointsTimer : IPeriodicPointsTimer //, IDisposable
    {
        private readonly IScrimMessageBroadcastService _messageService;
        private readonly ILogger<PeriodicPointsTimer> _logger;

        public string TimerName { get; set; }

        private readonly Timer _timer;
        //public TimeTracker Tracker { get; private set; }
        public TimerState State { get; private set; }
        public bool IsRunning { get; private set; }

        public int PeriodSeconds { get; private set; } = -1;

        // For handling pauses
        private DateTime _prevTickTime;
        private int _resumeDelayMs;

        private bool disposedValue;

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        //public event EventHandler<PeriodicPointsTimerStateMessage> RaiseMatchTimerTickEvent;
        //public delegate void PeriodPointsTimerTickEventHandler(object sender, PeriodicPointsTimerStateMessage e);

        //public event EventHandler<ScrimMessageEventArgs<PeriodicPointsTimerStateMessage>> RaisePeriodPointsTimerTickEvent;
        //public delegate void PeriodPointsTimerTickEventHandler(object sender, ScrimMessageEventArgs<PeriodicPointsTimerStateMessage> e);



        public PeriodicPointsTimer(string timerName, int periodSeconds = -1)
        {
            TimerName = timerName;

            //Tracker = new TimeTracker(periodSeconds);

            PeriodSeconds = periodSeconds;

            _timer = new Timer(HandleTick, _autoEvent, Timeout.Infinite, periodSeconds);

            IsRunning = false;

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

                //Tracker.Configure(periodSeconds);
                PeriodSeconds = periodSeconds;
            }
            else
            {
                //Tracker.Configure(null);
                PeriodSeconds = Timeout.Infinite;
            }

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
            //var message = new PeriodicPointsTimerStateMessage(this);
            //_messageService.BroadcastPeriodicPointsTimerTickMessage(message);

            //OnRaisePeriodicPointsTimerTickEvent(new PeriodicPointsTimerStateMessage(this)); // TODO: replace EventArgs

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
            //var message = new PeriodicPointsTimerStateMessage(this);
            //_messageService.BroadcastPeriodicPointsTimerTickMessage(message);

            //OnRaisePeriodicPointsTimerTickEvent(new PeriodicPointsTimerStateMessage(this)); // TODO: replace EventArgs

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
            //var message = new PeriodicPointsTimerStateMessage(this);
            //_messageService.BroadcastPeriodicPointsTimerTickMessage(message);

            //OnRaisePeriodicPointsTimerTickEvent(new PeriodicPointsTimerStateMessage(this)); // TODO: replace EventArgs

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
            //var message = new PeriodicPointsTimerStateMessage(this);
            //_messageService.BroadcastPeriodicPointsTimerTickMessage(message);

            //OnRaisePeriodicPointsTimerTickEvent(new PeriodicPointsTimerStateMessage(this)); // TODO: replace EventArgs

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
            
            //if (Tracker.HasTimeLimit)
            //{
            //    Configure(TimeSpan.FromSeconds(Tracker.SecondsMax.Value));
            //}
            //else
            //{
            //    Configure(null);
            //}
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
            //var message = new PeriodicPointsTimerStateMessage(this);
            //_messageService.BroadcastPeriodicPointsTimerTickMessage(message);

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

            //if (Tracker.HasTimeLimit)
            //{
            //    Tracker.ForceZeroRemaining();
            //}

            State = TimerState.Stopped;

            BroadcastEvent(false);
            //var message = new PeriodicPointsTimerStateMessage(this);
            //_messageService.BroadcastPeriodicPointsTimerTickMessage(message);

            //OnRaisePeriodicPointsTimerTickEvent(new PeriodicPointsTimerStateMessage(this)); // TODO: replace EventArgs

            _autoEvent.Set();
        }

        private void HandleTick(object stateInfo)
        {
            _autoEvent.WaitOne();

            if (ShouldProcessTick())
            {
                _prevTickTime = DateTime.UtcNow;

                //Tracker.StepForward();

                BroadcastEvent(true);
                //var message = new PeriodicPointsTimerStateMessage(this, true);
                //_messageService.BroadcastPeriodicPointsTimerTickMessage(message);

                //OnRaisePeriodicPointsTimerTickEvent(new PeriodicPointsTimerStateMessage(this)); // TODO: replace EventArgs
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

        //protected virtual void OnRaisePeriodicPointsTimerTickEvent(ScrimMessageEventArgs<PeriodicPointsTimerStateMessage> e)
        //{
        //    RaisePeriodPointsTimerTickEvent?.Invoke(this, e);
        //}

        /*
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _timer.Dispose();
                    //Tracker = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StatefulTimer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        */
    }
}
