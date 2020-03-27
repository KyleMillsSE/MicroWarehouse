using EventBusCore;

namespace MicroPicking.Integration
{
    public interface IPickingIntegrationService
    {
        void OnlyPublishEvent(Event @event);
    }
}
