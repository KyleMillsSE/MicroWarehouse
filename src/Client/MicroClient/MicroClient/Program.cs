using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MicroClient.Picking;
using MicroClient.Users;

namespace MicroClient
{
    class Program
    {
        private const string PICKING = "1";
        private const string UPDATE_USER = "9";
        private const string EXIT = "Q";

        private static HttpClient _httpClient;
        private static User _user;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Setting up client");

            _httpClient = new HttpClient {BaseAddress = new Uri("http://localhost:55799/")};
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Create user for this session
            _user = await (new CreateUserWorkFlow(_httpClient)).ExecuteAsync();

            if (_user == null) return;

            while (true)
            {
                Console.Clear();

                Console.WriteLine($"MAIN MENU");
                Console.WriteLine();
                Console.WriteLine($"Press the following option along with [ENTER] to continue");
                Console.WriteLine();
                Console.WriteLine($"Press {PICKING} for picking");
                Console.WriteLine($"Press {UPDATE_USER} to update session user");
                Console.WriteLine($"Press {EXIT} to exit");

                var result = Console.ReadLine();

                switch (result)
                {
                    case PICKING:
                        await new PickingWorkFlow(_httpClient, _user).ExecuteAsync();
                        break;

                    case UPDATE_USER:
                        _user = await new UpdateUserWorkFlow(_httpClient, _user).ExecuteAsync();
                        break;

                    case EXIT:
                        return;

                    default:
                        Console.WriteLine("Invalid option");
                        Thread.Sleep(250);
                        break;
                }
            }
        }
    }
}
