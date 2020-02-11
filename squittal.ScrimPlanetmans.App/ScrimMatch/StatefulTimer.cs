using squittal.ScrimPlanetmans.ScrimMatch.Events;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class StatefulTimer : IStatefulTimer
    {
        public MatchTimerStatus Status { get => _status; }
        
        private MatchTimerStatus _status;

        private int _secondsMax = 900;
        private int _secondsRemaining;
        private int _secondsElapsed = 0;

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        private Timer _timer;

        private bool _isRunning = false;

        // For handling pauses
        private DateTime _prevTickTime;
        private int _resumeDelayMs;

        public event EventHandler<MatchTimerTickEventArgs> RaiseMatchTimerTickEvent;
        public delegate void MatchTimerTickEventHandler(object sender, MatchTimerTickEventArgs e);


        //public StatefulTimer(TimeSpan timeSpan)
        //{
        //    _secondsMax = (int)Math.Round((decimal)timeSpan.TotalSeconds, 0);

        //    _status.SecondsMax = _secondsMax;

        //    _status.State = MatchTimerState.Initialized;

        //    // TODO: move Timer instantiation to the Start() method?
        //    _timer = new Timer(HandleTick, _autoEvent, Timeout.Infinite, 1000);
        //}

        public void Configure(TimeSpan timeSpan)
        {
            _autoEvent.WaitOne();
            if (!CanConfigureTimer())
            {
                _autoEvent.Set();
                return;
            }

            _status.State = MatchTimerState.Configuring;
            
            _secondsMax = (int)Math.Round((decimal)timeSpan.TotalSeconds, 0);

            _status.SecondsMax = _secondsMax;

            // TODO: move Timer instantiation to the Start() method?
            _timer?.Dispose(); // TODO: is this Dispose necessary?
            _timer = new Timer(HandleTick, _autoEvent, Timeout.Infinite, 1000);

            _status.State = MatchTimerState.Configured;
        }

        public void Start()
        {
            // Ensure timer can only be started once
            _autoEvent.WaitOne();
            if (_isRunning || _status.State != MatchTimerState.Configured)
            {
                _autoEvent.Set();
                return;
            }

            _status.State = MatchTimerState.Starting;

            // Don't think this is necessary; WaitOne() should already do a Reset  Reset to block other threads, just in case "Start Match" is double-clicked or something
            //_autoEvent.Reset();

            // Immediately start the clock
            // TODO: should we create a new Timer instance instead of having just starting up the single instance?
            _timer.Change(0, 1000);

            _isRunning = true;
            _status.State = MatchTimerState.Running;
            
            // TODO: broadcast a "MatchStateChange" of event of type "Started" here

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Stop()
        {
            _autoEvent.WaitOne();

            if (_status.State != MatchTimerState.Stopping)
            {
                _autoEvent.Set();
                return;
            }

            // TODO: should we Dispose of the timer instead of putting it on hold indefinitely?
            _timer.Change(Timeout.Infinite, 1000);

            _isRunning = false;
            _status.State = MatchTimerState.Stopped;

            var message = new MatchTimerTickMessage(_status);
            OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));

            // TODO: broadcast a "MatchStateChange" of event of type "Round Ended" here

            _autoEvent.Set();
        }

        public void Pause()
        {
            _autoEvent.WaitOne();
            if (_status.State != MatchTimerState.Running)
            {
                _autoEvent.Set();
                return;
            }

            var now = DateTime.UtcNow;
            _resumeDelayMs = (int)(now - _prevTickTime).TotalMilliseconds;

            _timer.Change(Timeout.Infinite, 1000);

            _isRunning = false;
            _status.State = MatchTimerState.Paused;

            var message = new MatchTimerTickMessage(_status);
            OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));

            // TODO: broadcast a "MatchStateChange" of event of type "Round Ended" here

            _autoEvent.Set();
        }

        public void Resume()
        {
            // Ensure timer can only be started once
            _autoEvent.WaitOne();
            if (_status.State != MatchTimerState.Paused)
            {
                _autoEvent.Set();
                return;
            }

            _status.State = MatchTimerState.Resuming;

            _timer.Change(_resumeDelayMs, 1000);

            _isRunning = true;
            _status.State = MatchTimerState.Running;

            var message = new MatchTimerTickMessage(_status);
            OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));

            // TODO: broadcast a "MatchStateChange" of event of type "Resumed" here

            // Signal the waiting thread
            _autoEvent.Set();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Halt()
        {
            throw new NotImplementedException();
        }

        private void HandleTick(object stateInfo)
        {
            _autoEvent.WaitOne();

            if (ShouldProcessTick())
            {
                _prevTickTime = DateTime.UtcNow;
                
                Interlocked.Decrement(ref _secondsRemaining);
                _status.SecondsRemaining = _secondsRemaining;

                Interlocked.Increment(ref _secondsElapsed);
                _status.SecondsElapsed = _secondsElapsed;

                var message = new MatchTimerTickMessage(_status);

                if (_secondsRemaining == 0)
                {
                    _status.State = MatchTimerState.Stopping;

                    // Signal the waiting thread. Only a thread waiting in Stop() should be able to do anything due to _status.State
                    _autoEvent.Set();
                    Stop();

                    return;
                }

                OnRaiseMatchTimerTickEvent(new MatchTimerTickEventArgs(message));

                // Signal the waiting thread
                _autoEvent.Set();
            }

        }

        private bool ShouldProcessTick()
        {
            return _status.State switch
            {
                MatchTimerState.Running => true,
                MatchTimerState.Starting => false,
                MatchTimerState.Paused => false,
                MatchTimerState.Stopping => false,
                MatchTimerState.Stopped => false,
                MatchTimerState.Halting => false,
                MatchTimerState.Initialized => false,
                MatchTimerState.Resuming => false,
                MatchTimerState.Configuring => false,
                MatchTimerState.Configured => false,
                _ => false,
            };
        }

        private bool CanConfigureTimer()
        {
            return _status.State switch
            {
                MatchTimerState.Stopped => true,
                MatchTimerState.Initialized => true,
                MatchTimerState.Configured => true,
                MatchTimerState.Running => false,
                MatchTimerState.Starting => false,
                MatchTimerState.Paused => false,
                MatchTimerState.Stopping => false,
                MatchTimerState.Halting => false,
                MatchTimerState.Resuming => false,
                MatchTimerState.Configuring => false,
                _ => false,
            };
        }

        protected virtual void OnRaiseMatchTimerTickEvent(MatchTimerTickEventArgs e)
        {
            RaiseMatchTimerTickEvent?.Invoke(this, e);
        }


    }
}
