using MicroUsers.Domain;

namespace MicroUsers.DataAccess.Repositories
{
    public interface IUserRepository
    {
        void Insert(User user);

        void Update(User user);
    }
}
