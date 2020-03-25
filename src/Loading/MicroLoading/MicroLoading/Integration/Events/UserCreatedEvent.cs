using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBusRabbitMQ;
using MicroLoading.Domain;

namespace MicroLoading.Integration.Events
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
