/* All credit to Lampjaw (Voidwell.DaybreakGames.App) */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans
{
    public class KeyedSemaphoreSlim : IDisposable
    {
        private readonly object _lock = new object();

        private readonly Queue<SemaphoreWrapper> _wrapperQueue
            = new Queue<SemaphoreWrapper>();

        private readonly Dictionary<string, SemaphoreWrapper> _wrapperMap
            = new Dictionary<string, SemaphoreWrapper>();

        private bool _isDisposed;

        public Task<IDisposable> WaitAsync(
            string key,
            CancellationToken cancelToken = default(CancellationToken))
        {
            lock (_lock)
            {
                SemaphoreWrapper wrapper;

                if (_wrapperMap.ContainsKey(key))
                    wrapper = _wrapperMap[key];
                else
                {
                    wrapper = _wrapperMap[key] = _wrapperQueue.Count == 0
                        ? new SemaphoreWrapper(Release)
                        : _wrapperQueue.Dequeue();

                    wrapper.Key = key;
                }

                return wrapper.WaitAsync(cancelToken);
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            lock (_lock)
                foreach (var value in _wrapperMap.Values)
                    value.InternalDispose();

            _isDisposed = true;
        }

        private void Release(SemaphoreWrapper wrapper)
        {
            lock (_lock)
            {
                var isEmpty = wrapper.Release();
                if (!isEmpty)
                    return;

                _wrapperMap.Remove(wrapper.Key);
                _wrapperQueue.Enqueue(wrapper);
            }
        }

        private class SemaphoreWrapper : IDisposable
        {
            private readonly Action<SemaphoreWrapper> _parentRelease;
            private readonly SemaphoreSlim _semaphoreSlim;

            private int _useCount;

            public SemaphoreWrapper(Action<SemaphoreWrapper> parentRelease)
            {
                _parentRelease = parentRelease;
                _semaphoreSlim = new SemaphoreSlim(1, 1);
            }

            public string Key { get; set; }

            public async Task<IDisposable> WaitAsync(CancellationToken cancelToken)
            {
                _useCount++;
                await _semaphoreSlim.WaitAsync(cancelToken).ConfigureAwait(false);
                return this;
            }

            public bool Release()
            {
                _semaphoreSlim.Release();
                _useCount--;
                return _useCount == 0;
            }

            public void Dispose()
            {
                _parentRelease(this);
            }

            public void InternalDispose()
            {
                _semaphoreSlim.Dispose();
            }
        }
    }
}
