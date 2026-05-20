using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using VAProject.Core.Logger;

namespace VAProject.Core.Utils.Memorization
{
    public class ActiveTtlCache<TKey, TValue> : IDisposable
    {
        private class CacheItem
        {
            public TValue Value { get; }
            public DateTime ExpirationTime { get; }

            public CacheItem(TValue value, TimeSpan timeToLive)
            {
                Value = value;
                ExpirationTime = DateTime.UtcNow.Add(timeToLive);
            }

            public bool IsExpired => DateTime.UtcNow > ExpirationTime;
        }

        private readonly ConcurrentDictionary<TKey, CacheItem> _cache = new();

        private readonly Timer _cleanupTimer;

        public ActiveTtlCache(TimeSpan cleanupInterval)
        {
            _cleanupTimer = new Timer(CleanupExpiredItems, null, cleanupInterval, cleanupInterval);
        }

        public void Set(TKey key, TValue value, TimeSpan timeToLive)
        {
            var item = new CacheItem(value, timeToLive);
            _cache[key] = item;
        }

        public TValue GetOrAdd(TKey key, TimeSpan timeToLive, Func<TKey, TValue> function)
        {
            if (_cache.TryGetValue(key, out var item))
            {
                if (!item.IsExpired)
                {
                    LogManager.Log($"[CACHE HIT] Returning from cache: {key}");
                    return item.Value;
                }
                else
                {
                    _cache.TryRemove(key, out _);
                }
            }

            LogManager.Log($"[CACHE MISS] Performing heavy work for: {key}");
            TValue value = function(key);
            Set(key, value, timeToLive);
            return value;
        }

        private void CleanupExpiredItems(object state)
        {
            int removedCount = 0;

            foreach (var kvp in _cache)
            {
                if (kvp.Value.IsExpired)
                {
                    if (_cache.TryRemove(kvp.Key, out _))
                    {
                        removedCount++;
                    }
                }
            }

            if (removedCount > 0)
            {
                LogManager.Log($"[Cache sweeper] deleted elements: {removedCount}");
            }
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }
}
