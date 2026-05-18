using VAProject.Core.Logger;

namespace VAProject.Core.Utils
{
    internal class LruCache<TKey, TValue>
    {
        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; }

            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
        private readonly int _capacity;

        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>();

        private readonly LinkedList<CacheItem> _lruList = new LinkedList<CacheItem>();

        public LruCache(int capacity)
        {
            if (capacity <= 0) 
                throw new ArgumentException("Capacity must be greater than 0", nameof(capacity));
            _capacity = capacity;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> function)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                _lruList.Remove(node);
                _lruList.AddFirst(node);

                LogManager.Log($"[CACHE HIT] Returning from cache: {key}");
                return node.Value.Value;
            }

            LogManager.Log($"[CACHE MISS] Performing heavy work for: {key}");
            TValue result = function(key);

            CacheItem cacheItem = new CacheItem(key, result);
            LinkedListNode<CacheItem> newNode = new LinkedListNode<CacheItem>(cacheItem);

            _lruList.AddFirst(newNode);
            _cacheMap.Add(key, newNode);

            if (_cacheMap.Count > _capacity)
            {
                var oldestNode = _lruList.Last;
                if (oldestNode != null)
                {
                    _cacheMap.Remove(oldestNode.Value.Key);
                    _lruList.RemoveLast();
                }
            }

            return result;
        }

        
    }
}
