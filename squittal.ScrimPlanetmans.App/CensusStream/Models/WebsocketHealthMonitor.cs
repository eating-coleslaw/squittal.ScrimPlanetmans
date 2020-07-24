// Credit to Lampjaw @ Voidwell.DaybreakGames
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace squittal.ScrimPlanetmans.CensusStream
{
    public class WebsocketHealthMonitor : IWebsocketHealthMonitor
    {
        private readonly ILogger<WebsocketHealthMonitor> _logger;

        private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, DateTime>> _worldsLastSeenEvents = new ConcurrentDictionary<int, ConcurrentDictionary<string, DateTime>>();

        private readonly List<int> _ignorableWorlds = new List<int> { 25 };
        private readonly Dictionary<string, TimeSpan> _unhealthyEventIntervals = new Dictionary<string, TimeSpan>
        {
            {  "Death", TimeSpan.FromMinutes(5) }
        };

        public WebsocketHealthMonitor(ILogger<WebsocketHealthMonitor> logger)
        {
            _logger = logger;
        }

        public void ReceivedEvent(int worldId, string eventName, DateTime? timestamp = null)
        {
            if (timestamp == null)
            {
                timestamp = DateTime.UtcNow;
            }

            try
            {
                if (!_worldsLastSeenEvents.ContainsKey(worldId))
                {
                    _worldsLastSeenEvents.TryAdd(worldId, new ConcurrentDictionary<string, DateTime>());
                }

                _worldsLastSeenEvents[worldId].AddOrUpdate(eventName, timestamp.Value, (k, v) => timestamp.Value);
            }
            catch (Exception ex)
            {
                var timeDisplay = timestamp == null ? "unknown" : ((DateTime)timestamp).ToLongTimeString();
                _logger.LogError($"Failed to update world state from event eventName={eventName} worldId={worldId} time={timeDisplay}: {ex}");
            }
        }

        public void ClearWorld(int worldId)
        {
            _worldsLastSeenEvents.TryRemove(worldId, out var _);
        }

        public void ClearAllWorlds()
        {
            _worldsLastSeenEvents.Clear();
        }

        public bool IsHealthy()
        {
            var worldIds = _worldsLastSeenEvents.Keys.Where(a => !_ignorableWorlds.Contains(a)).ToList();

            return !worldIds.Where(a => !TryEvaluateWorldHealth(a)).Any();
        }

        private bool TryEvaluateWorldHealth(int worldId)
        {
            if (_worldsLastSeenEvents.TryGetValue(worldId, out var eventList))
            {
                if (!EvaluateWorldHealth(eventList))
                {
                    _logger.LogWarning(34214, "Stream for world '{worldId}' failed health check", worldId);
                    return false;
                }
            }
            return true;
        }

        private bool EvaluateWorldHealth(ConcurrentDictionary<string, DateTime> worldEvents)
        {
            var now = DateTime.UtcNow;

            foreach ((var eventName, var interval) in _unhealthyEventIntervals)
            {
                if (worldEvents != null && worldEvents.TryGetValue(eventName, out var lastReceivedTime))
                {
                    if (now - lastReceivedTime > interval)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
