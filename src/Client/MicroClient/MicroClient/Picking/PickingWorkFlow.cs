using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicroClient.Users;
using Newtonsoft.Json;

namespace MicroClient.Picking
{
    public class PickingWorkFlow
    {
        private const string PICK_PALLET = "1";
        private const string GET_PALLETS = "2";
        private const string EXIT = "Q";

        private readonly HttpClient _httpClient;
        private readonly User _user;

        public PickingWorkFlow(HttpClient httpClient, User user)
        {
            _httpClient = httpClient;
            _user = user;
        }

        public async Task ExecuteAsync()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine($"PICKING");
                Console.WriteLine();
                Console.WriteLine($"Press the following option along with [ENTER] to continue");
                Console.WriteLine();
                Console.WriteLine($"Press {PICK_PALLET} to create and pick pallet");
                Console.WriteLine($"Press {GET_PALLETS} to view pallets");
                Console.WriteLine($"Press {EXIT} to exit");

                var result = Console.ReadLine();

                switch (result)
                {
                    case PICK_PALLET:
                        Console.Clear();
                        Console.WriteLine("Pallet number?");
                        var palletNumber = Console.ReadLine();

                        Console.WriteLine("Line description?");
                        var lineDescription = Console.ReadLine();

                        Console.WriteLine("Line quantity?");

                        var lineQuantity = 0;
                        while (!Int32.TryParse(Console.ReadLine(), out lineQuantity))
                        {
                            Console.WriteLine("Must be integer");
                        };

                        await _httpClient.PostAsync("/pallets", new JsonContent(new Pallet() { PalletNumber = palletNumber, LineDescription = lineDescription, LineQuantity = lineQuantity, UserId = _user.Id}));

                        break;

                    case GET_PALLETS:
                        Console.Clear();

                        var requestResult = await _httpClient.GetAsync("/pallets");

                        var pallets = JsonConvert.DeserializeObject<List<Pallet>>(await requestResult.Content.ReadAsStringAsync());

                        pallets.ForEach(x => Console.WriteLine($"Pallet number: {x.PalletNumber} - Picked description: {x.LineDescription} - Picked quantity: {x.LineQuantity} - by user: {x.User.FirstName} {x.User.LastName}"));

                        Console.WriteLine();
                        Console.WriteLine("Press any key to stop displaying pallets");
                        Console.ReadKey();
                        break;

                    case EXIT:
                        return;

                    default:
                        Console.WriteLine("Invalid option");
                        Thread.Sleep(250);
                        break;
                }

                Console.Clear();
            }
        }
    }
}
