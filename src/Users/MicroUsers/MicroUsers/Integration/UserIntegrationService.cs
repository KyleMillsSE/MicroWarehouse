using EventBusCore;
using Microsoft.Extensions.Logging;

namespace MicroUsers.Integration
{
    public class UserIntegrationService : IUserIntegrationService
    {
        private readonly ILogger<UserIntegrationService> _logger;
        private readonly IEventBus _eventBus;

        public UserIntegrationService(ILogger<UserIntegrationService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public void OnlyPublishEvent(Event @event)
        {
            _logger.LogInformation($"Publishing user event '{@event.GetType().Name} - {@event.Id}'");

            _eventBus.Publish(@event);
        }
    }
}
