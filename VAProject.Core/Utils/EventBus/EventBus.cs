using System;
using System.Collections.Generic;
using System.Text;
using VAProject.Core.Interfaces;
using VAProject.Core.Logger;

namespace VAProject.Core.Utils.EventBus
{
    public class EventBus
    {
        private class Subscription : ISubscription
        {
            private readonly Action _unsubscribeAction;

            public Subscription(Action unsubscribeAction)
            {
                _unsubscribeAction = unsubscribeAction;
            }

            public void Dispose()
            {
                _unsubscribeAction?.Invoke();
            }
        }

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        public ISubscription Subscribe<TMessage>(Action<TMessage> handler)
        {
            if (handler == null)
            {
                LogManager.Log("Attempted to subscribe with a null handler.", LogLevel.Error);
                throw new ArgumentNullException(nameof(handler));
            }

            var messageType = typeof(TMessage);
            if (!_subscribers.ContainsKey(messageType))
            {
                _subscribers[messageType] = new List<Delegate>();
            }
            _subscribers[messageType].Add(handler);

            return new Subscription (() => Unsubscribe<TMessage>(handler));
        }

        public void Unsubscribe<TMessage>(Action<TMessage> handler)
        {
            if (handler == null)
            {
                LogManager.Log("Attempted to unsubscribe with a null handler.", LogLevel.Error);
                throw new ArgumentNullException(nameof(handler));
            }

            var messageType = typeof(TMessage);
            if (_subscribers.ContainsKey(messageType))
            {
                _subscribers[messageType].Remove(handler);
                if (_subscribers[messageType].Count == 0)
                {
                    _subscribers.Remove(messageType);
                }
            }
        }

        public void Publish<TMessage>(TMessage message)
        {
            var messageType = typeof(TMessage);
            if (_subscribers.ContainsKey(messageType))
            {
                foreach (var handler in _subscribers[messageType])
                {
                    try
                    {
                        ((Action<TMessage>)handler)?.Invoke(message);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log($"Error while publishing message of type {messageType.Name}: {ex.Message}", LogLevel.Error);
                    }
                }
            }
        }
    }
}
