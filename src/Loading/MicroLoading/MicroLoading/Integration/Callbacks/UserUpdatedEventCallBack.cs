using EventBusCore;
using MicroLoading.DataAccess;
using MicroLoading.Integration.Events;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MicroLoading.Integration.Callbacks
{
    public class UserUpdatedEventCallBack : IEventCallback
    {
        private readonly ILogger<UserUpdatedEventCallBack> _logger;
        private readonly InMemoryLoadingContext _context;

        public UserUpdatedEventCallBack(ILogger<UserUpdatedEventCallBack> logger, InMemoryLoadingContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void Execute(Event @event)
        {
            var userUpdatedEvent = (UserUpdatedEvent)@event;

            _logger.LogInformation($"Executing user updated event call back '{userUpdatedEvent.Id}'");

            var memUser = _context.Users.First(x => x.Id == userUpdatedEvent.User.Id);

            memUser.FirstName = userUpdatedEvent.User.FirstName;
            memUser.LastName = userUpdatedEvent.User.LastName;
        }
    }
}
