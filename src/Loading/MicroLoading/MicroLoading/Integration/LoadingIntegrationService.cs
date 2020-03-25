using EventBusRabbitMQ;
using Microsoft.Extensions.Logging;

namespace MicroLoading.Integration
{
    public class LoadingIntegrationService : ILoadingIntegrationService
    {

        private readonly ILogger<LoadingIntegrationService> _logger;
        private readonly IEventBus _eventBus;

        public LoadingIntegrationService(ILogger<LoadingIntegrationService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public void OnlyPublishEvent(Event @event)
        {
            _logger.LogInformation($"Publishing loading event '{@event.GetType().Name} - {@event.Id}'");

            _eventBus.Publish(@event);
        }
    }
}
