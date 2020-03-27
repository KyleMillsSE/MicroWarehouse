using EventBusCore;
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
