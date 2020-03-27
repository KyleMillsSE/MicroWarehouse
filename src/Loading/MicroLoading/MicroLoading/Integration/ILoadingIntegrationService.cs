using EventBusCore;

namespace MicroLoading.Integration
{
    public interface ILoadingIntegrationService
    {
        void OnlyPublishEvent(Event @event);
    }
}
