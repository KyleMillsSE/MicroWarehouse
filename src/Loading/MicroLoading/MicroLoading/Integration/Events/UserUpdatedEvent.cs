using EventBusCore;
using MicroLoading.Domain;

namespace MicroLoading.Integration.Events
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
