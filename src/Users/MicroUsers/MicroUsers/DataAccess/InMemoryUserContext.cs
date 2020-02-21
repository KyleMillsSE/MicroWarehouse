using MicroUsers.Domain;
using System.Collections.Generic;

namespace MicroUsers.DataAccess
{
    public class InMemoryUserContext
    {
        private readonly List<User> _users = new List<User>();

        public List<User> Users => _users;
    }
}
