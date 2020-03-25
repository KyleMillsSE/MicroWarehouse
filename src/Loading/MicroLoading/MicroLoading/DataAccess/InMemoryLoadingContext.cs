using MicroLoading.Domain;
using System.Collections.Generic;

namespace MicroLoading.DataAccess
{
    public class InMemoryLoadingContext
    {
        private readonly List<User> _users = new List<User>();

        public List<User> Users => _users;
    }
}
