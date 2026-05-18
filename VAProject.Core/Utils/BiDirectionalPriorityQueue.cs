using System;
using System.Collections.Generic;
using System.Text;
using VAProject.Core.Enums;
using VAProject.Core.Logger;

namespace VAProject.Core.Utils
{
    public class BiDirectionalPriorityQueue<T>
    {
        private class QueueItem
        {
            public T Item { get; }
            public int Priority { get; }
            public DateTime InsertTime { get; }

            public QueueItem(T item, int priority)
            {
                Item = item;
                Priority = priority;
                InsertTime = DateTime.Now;
            }
        }

        private readonly LinkedList<QueueItem> _items = new LinkedList<QueueItem>();
        public int Count => _items.Count;
        public bool IsEmpty => _items.Count == 0;

        public void Enqueue(T item, int priority)
        {
            _items.AddLast(new QueueItem(item, priority));
        }

        public T Peek(QueueStrategy strategy)
        {
            if (_items.Count == 0)
            {
                LogManager.Log("Queue is empty", LogLevel.Error);
                throw new InvalidOperationException("Queue is empty");
            }

            var node = FindNodeByStrategy(strategy);
            return node.Value.Item;
        }

        public T Dequeue(QueueStrategy strategy)
        {
            if (_items.Count == 0) 
            {
                LogManager.Log("Queue is empty", LogLevel.Error);
                throw new InvalidOperationException("Queue is empty");
            }

            var node = FindNodeByStrategy(strategy);
            _items.Remove(node);
            return node.Value.Item;
        }


        private LinkedListNode<QueueItem> FindNodeByStrategy(QueueStrategy strategy)
        {
            switch (strategy)
            {
                case QueueStrategy.Oldest:
                    return _items.First;

                case QueueStrategy.Newest:
                    return _items.Last;

                case QueueStrategy.HighestPriority:
                    return FindNode(i => i.Priority == _items.Max(x => x.Priority));

                case QueueStrategy.LowestPriority:
                    return FindNode(i => i.Priority == _items.Min(x => x.Priority));

                default:
                    LogManager.Log("Unknown strategy", LogLevel.Error);
                    throw new InvalidOperationException("Unknown strategy");
            }
        }

        private LinkedListNode<QueueItem> FindNode(Func<QueueItem, bool> predicate)
        {
            var current = _items.First;
            while (current != null)
            {
                if (predicate(current.Value)) return current;
                current = current.Next;
            }
            return _items.First;
        }
    }
}
