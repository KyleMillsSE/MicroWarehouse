using MicroPicking.Domain;
using System.Collections.Generic;
using System.Linq;

namespace MicroPicking.DataAccess.Repositories
{
    public class InMemoryPalletRepository : IPalletRepository
    {
        private readonly InMemoryPickingContext _context;

        public InMemoryPalletRepository(InMemoryPickingContext context)
        {
            _context = context;
        }

        public IEnumerable<Pallet> GetAll()
        {
            var pallets = _context.Pallets;

            // Populate navigation should be handled by the context but this is an in memory implementation
            pallets.ForEach(x => x.User = _context.Users.FirstOrDefault(y => y.Id == x.UserId));

            return pallets;
        }

        public void Insert(Pallet pallet)
        {
            // Populate id should be handled by the context but this is an in memory implementation
            pallet.Id = _context.Pallets.Count + 1;

            _context.Pallets.Add(pallet);
        }
    }
}
