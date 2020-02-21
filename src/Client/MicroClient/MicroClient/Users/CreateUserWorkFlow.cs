using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MicroClient.Users
{
    public class CreateUserWorkFlow
    {
        private readonly HttpClient _httpClient;

        public CreateUserWorkFlow(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> ExecuteAsync()
        {
            Console.Clear();

            Console.WriteLine("CREATE USER");
            Console.WriteLine();
            Thread.Sleep(250);

            Console.WriteLine("User's first name?");
            var firstName = Console.ReadLine();


            Console.WriteLine("User's second name?");
            var lastName = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Creating user...");

            var result = await _httpClient.PostAsync("/users", new JsonContent(new User() { FirstName = firstName, LastName = lastName }));

            Console.WriteLine("Successfully created user!");

            return JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());
        }
    }
}
