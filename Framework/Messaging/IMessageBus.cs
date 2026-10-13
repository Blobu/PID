using System;

namespace Framework.Messaging
{
    public interface IMessageBus
    {
        void Subscribe<TMessage>(object subscriber, Action<TMessage> handler);
        void Unsubscribe<TMessage>(object subscriber);
        void Publish<TMessage>(TMessage message);
    }
}