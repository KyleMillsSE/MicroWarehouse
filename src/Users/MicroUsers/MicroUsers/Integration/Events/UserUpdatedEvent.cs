using EventBusCore;
using MicroUsers.Domain;

namespace MicroUsers.Integration.Events
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
