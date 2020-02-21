using MicroPicking.Domain;
using System.Collections.Generic;

namespace MicroPicking.DataAccess
{
    public class InMemoryPickingContext
    {
        private readonly List<Pallet> _pallets = new List<Pallet>();
        private readonly List<User> _users = new List<User>();

        public List<Pallet> Pallets => _pallets;
        public List<User> Users => _users;
    }
}
