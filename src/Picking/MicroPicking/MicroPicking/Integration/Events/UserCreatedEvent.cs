using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBusRabbitMQ;
using MicroPicking.Domain;

namespace MicroPicking.Integration.Events
{
    public class UserCreatedEvent : Event
    {
        public User User { get; }

        public UserCreatedEvent(User user)
        {
            User = user;
        }
    }

}
