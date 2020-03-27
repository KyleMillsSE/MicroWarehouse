using EventBusCore;
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
