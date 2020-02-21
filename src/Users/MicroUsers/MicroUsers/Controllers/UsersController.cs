using Microsoft.AspNetCore.Mvc;
using MicroUsers.Domain;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MicroUsers.DataAccess.Repositories;
using MicroUsers.Integration;
using MicroUsers.Integration.Events;

namespace MicroUsers.Controllers
{
    [Route("/users")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUserIntegrationService _userIntegrationService;

        public UsersController(ILogger<UsersController> logger, IUserRepository userRepository, IUserIntegrationService userIntegrationService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userIntegrationService = userIntegrationService;
        }

        [HttpPost]
        public IActionResult PostAsync([FromBody]User user)
        {
            _logger.LogInformation($"Creating user {user.FirstName} with last name {user.LastName}");

            _userRepository.Insert(user);

            _userIntegrationService.OnlyPublishEvent(new UserCreatedEvent(user));

            return Ok(user);
        }

        [HttpPut]
        public IActionResult PutAsync([FromBody]User user)
        {
            _logger.LogInformation($"Updating user {user.Id}");

            _userRepository.Update(user);

            _userIntegrationService.OnlyPublishEvent(new UserUpdatedEvent(user));

            return Ok(user);
        }
    }
}