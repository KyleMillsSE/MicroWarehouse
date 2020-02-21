using Newtonsoft.Json;
using System;

namespace EventBusRabbitMQ
{
    public abstract class Event
    {
        [JsonProperty]
        public Guid Id { get; set; }

        public Event()
        {
            Id = Guid.NewGuid();
        }

        [JsonConstructor]
        public Event(Guid id)
        {
            Id = id;
        }
    }
}
