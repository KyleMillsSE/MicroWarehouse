using EventBusCore;

namespace MicroUsers.Integration
{
    public interface IUserIntegrationService
    {
        void OnlyPublishEvent(Event @event);
    }
}
