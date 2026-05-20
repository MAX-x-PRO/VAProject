using System;
using System.Collections.Generic;
using VAProject.Core.Logger;

namespace VAProject.Core.Utils.Memorization
{
    public class LfuCache<TKey, TValue>
    {
        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; }
            public int Frequency { get; set; }
            public LinkedListNode<TKey> NodePointer { get; set; }

            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Frequency = 1;
            }
        }

        private readonly int _capacity;

        private Dictionary<TKey, CacheItem> _cacheMap = new Dictionary<TKey, CacheItem>();
        private Dictionary<int, LinkedList<TKey>> _frequencyMap = new Dictionary<int, LinkedList<TKey>>();

        private int _minFrequency = 0;

        public LfuCache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be greater than 0", nameof(capacity));
            }
            _capacity = capacity;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> function)
        {
            if (_cacheMap.TryGetValue(key, out var cacheItem))
            {
                UpdateFrequency(cacheItem);
                LogManager.Log($"[CACHE HIT] Returning from cache: {key}");

                return cacheItem.Value;
            }

            LogManager.Log($"[CACHE MISS] Performing heavy work for: {key}");
            TValue result = function(key);

            if (_cacheMap.Count >= _capacity)
            {
                EvictLeastFrequentlyUsed();
            }

            AddNewItem(key, result);

            return result;
        }


        private void UpdateFrequency(CacheItem item)
        {
            int oldFreq = item.Frequency;

            _frequencyMap[oldFreq].Remove(item.NodePointer);

            if (oldFreq == _minFrequency && _frequencyMap[oldFreq].Count == 0)
            {
                _minFrequency++;
            }

            item.Frequency++;
            int newFreq = item.Frequency;

            if (!_frequencyMap.ContainsKey(newFreq))
            {
                _frequencyMap[newFreq] = new LinkedList<TKey>();
            }

            item.NodePointer = _frequencyMap[newFreq].AddFirst(item.Key);
        }

        private void EvictLeastFrequentlyUsed()
        {
            var minFreqList = _frequencyMap[_minFrequency];

            TKey keyToRemove = minFreqList.Last.Value;

            minFreqList.RemoveLast();

            _cacheMap.Remove(keyToRemove);
        }

        private void AddNewItem(TKey key, TValue value)
        {
            var newItem = new CacheItem(key, value);

            _minFrequency = 1;

            if (!_frequencyMap.ContainsKey(1))
            {
                _frequencyMap[1] = new LinkedList<TKey>();
            }

            newItem.NodePointer = _frequencyMap[1].AddFirst(key);

            _cacheMap[key] = newItem;
        }
    }
}
