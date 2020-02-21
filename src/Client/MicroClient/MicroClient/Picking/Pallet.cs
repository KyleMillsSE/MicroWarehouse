using System;
using System.Collections.Generic;
using System.Text;
using MicroClient.Users;

namespace MicroClient.Picking
{
    public class Pallet
    {
        public string PalletNumber { get; set; }
        public string LineDescription { get; set; }
        public int LineQuantity { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
