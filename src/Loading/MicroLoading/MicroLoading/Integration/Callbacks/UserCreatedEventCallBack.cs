using EventBusCore;
using EventBusRabbitMQ;
using MicroLoading.DataAccess;
using MicroLoading.Integration.Events;
using Microsoft.Extensions.Logging;

namespace MicroLoading.Integration.Callbacks
{
    public class UserCreatedEventCallBack : IEventCallback
    {
        private readonly ILogger<UserCreatedEventCallBack> _logger;
        private readonly InMemoryLoadingContext _context;

        public UserCreatedEventCallBack(ILogger<UserCreatedEventCallBack> logger, InMemoryLoadingContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void Execute(Event @event)
        {
            var userCreatedEvent = (UserCreatedEvent)@event;

            _logger.LogInformation($"Executing user created event call back '{userCreatedEvent.Id}'");

            _context.Users.Add(userCreatedEvent.User);
        }
    }
}
