using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Framework.Messaging
{
    public class MessageBus : IMessageBus
    {
        private static readonly Lazy<MessageBus> _instance =
            new Lazy<MessageBus>(() => new MessageBus());

        public static MessageBus Instance => _instance.Value;

        private readonly Dictionary<Type, List<Subscription>> _subscriptions =
            new Dictionary<Type, List<Subscription>>();

        private readonly ConditionalWeakTable<object, List<Action<object>>> _handlers =
            new ConditionalWeakTable<object, List<Action<object>>>();

        private readonly object _lock = new object();

        private MessageBus() { }

        public void Subscribe<TMessage>(object subscriber, Action<TMessage> handler)
        {
            lock (_lock)
            {
                var messageType = typeof(TMessage);

                if (!_subscriptions.ContainsKey(messageType))
                    _subscriptions[messageType] = new List<Subscription>();

                Action<object> wrapper = msg => handler((TMessage)msg);
                _handlers.GetOrCreateValue(subscriber).Add(wrapper);
                _subscriptions[messageType].Add(new Subscription(subscriber, wrapper));
            }
        }

        public void Unsubscribe<TMessage>(object subscriber)
        {
            lock (_lock)
            {
                var messageType = typeof(TMessage);

                if (_subscriptions.ContainsKey(messageType))
                {
                    if (_handlers.TryGetValue(subscriber, out var handlers))
                    {
                        foreach (var subscription in _subscriptions[messageType].Where(s => s.Subscriber == subscriber))
                            handlers.Remove(subscription.Handler);
                    }

                    _subscriptions[messageType].RemoveAll(s =>
                        s.Subscriber == subscriber || !s.IsAlive);
                }
            }
        }

        public void Publish<TMessage>(TMessage message)
        {
            List<Subscription> snapshot;

            lock (_lock)
            {
                var messageType = typeof(TMessage);
                if (!_subscriptions.ContainsKey(messageType))
                    return;

                _subscriptions[messageType].RemoveAll(s => !s.IsAlive);
                snapshot = _subscriptions[messageType].ToList();
            }

            foreach (var subscription in snapshot)
            {
                subscription.Invoke(message);
            }
        }

        private class Subscription
        {
            private readonly WeakReference _subscriberRef;
            private readonly WeakReference _handlerRef;

            public Subscription(object subscriber, Action<object> handler)
            {
                _subscriberRef = new WeakReference(subscriber);
                _handlerRef = new WeakReference(handler);
            }

            public object Subscriber => _subscriberRef.Target;
            public Action<object> Handler => _handlerRef.Target as Action<object>;
            public bool IsAlive => _subscriberRef.IsAlive && _handlerRef.IsAlive;

            public void Invoke(object message)
            {
                if (_subscriberRef.IsAlive)
                    Handler?.Invoke(message);
            }
        }
    }
}
