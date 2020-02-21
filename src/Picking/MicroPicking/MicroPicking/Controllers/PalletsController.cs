using MicroPicking.DataAccess.Repositories;
using MicroPicking.Domain;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MicroPicking.Controllers
{

    [Route("/pallets")]
    public class PalletsController : Controller
    {
        private readonly IPalletRepository _palletRepository;

        public PalletsController(IPalletRepository palletRepository)
        {
            _palletRepository = palletRepository;
        }

        [HttpPost]
        public IActionResult PostAsync([FromBody]Pallet pallet)
        {
            Console.WriteLine($"User {pallet.UserId} created pallet {pallet.PalletNumber} with item {pallet.LineDescription} and quantity {pallet.LineQuantity}");

            _palletRepository.Insert(pallet);
            
            return Ok(pallet);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_palletRepository.GetAll());
        }
    }
}
