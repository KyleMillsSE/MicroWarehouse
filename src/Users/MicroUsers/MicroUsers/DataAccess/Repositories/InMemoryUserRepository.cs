using System.Linq;
using MicroUsers.Domain;

namespace MicroUsers.DataAccess.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly InMemoryUserContext _context;

        public InMemoryUserRepository(InMemoryUserContext context)
        {
            _context = context;
        }

        public void Insert(User user)
        {
            // Populate id should be handled by the context but this is an in memory implementation
            user.Id = _context.Users.Count + 1;

            _context.Users.Add(user);
        }

        public void Update(User user)
        {
            var memUser = _context.Users.First(x => x.Id == user.Id);

            memUser.FirstName = user.FirstName;
            memUser.LastName = user.LastName;
        }
    }
}
