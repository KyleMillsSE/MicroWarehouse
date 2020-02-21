using EventBusRabbitMQ;
using MicroUsers.Domain;

namespace MicroUsers.Integration.Events
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
