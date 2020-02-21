using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusRabbitMQ
{
    public interface IEventBus
    {
        void Subscribe<TEvent, TEventCallback>()
            where TEvent : Event
            where TEventCallback : IEventCallback;

        void Publish(Event @event);
    }
}
