using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroPicking.Domain;

namespace MicroPicking.DataAccess.Repositories
{
    public interface IPalletRepository
    {
        IEnumerable<Pallet> GetAll();

        void Insert(Pallet pallet);
    }
}
