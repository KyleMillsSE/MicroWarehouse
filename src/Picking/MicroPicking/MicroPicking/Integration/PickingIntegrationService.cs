using EventBusRabbitMQ;
using Microsoft.Extensions.Logging;

namespace MicroPicking.Integration
{
    public class PickingIntegrationService : IPickingIntegrationService
    {
        private readonly ILogger<PickingIntegrationService> _logger;
        private readonly IEventBus _eventBus;

        public PickingIntegrationService(ILogger<PickingIntegrationService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public void OnlyPublishEvent(Event @event)
        {
            _logger.LogInformation($"Publishing picking event '{@event.GetType().Name} - {@event.Id}'");

            _eventBus.Publish(@event);
        }
    }
}
