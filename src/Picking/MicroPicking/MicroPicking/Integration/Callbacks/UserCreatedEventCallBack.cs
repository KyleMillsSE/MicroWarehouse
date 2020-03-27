using EventBusCore;
using MicroPicking.DataAccess;
using MicroPicking.Integration.Events;
using Microsoft.Extensions.Logging;

namespace MicroPicking.Integration.Callbacks
{
    public class UserCreatedEventCallBack : IEventCallback
    {
        private readonly ILogger<UserCreatedEventCallBack> _logger;
        private readonly InMemoryPickingContext _context;

        public UserCreatedEventCallBack(ILogger<UserCreatedEventCallBack> logger, InMemoryPickingContext context)
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
