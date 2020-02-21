using EventBusRabbitMQ;
using MicroPicking.Domain;

namespace MicroPicking.Integration.Events
{
    public class UserUpdatedEvent : Event
    {
        public User User { get; }

        public UserUpdatedEvent(User user)
        {
            User = user;
        }
    }
}
