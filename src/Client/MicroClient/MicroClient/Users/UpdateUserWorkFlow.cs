using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MicroClient.Users
{
    public class UpdateUserWorkFlow
    {
        private readonly HttpClient _httpClient;
        private readonly User _user;

        public UpdateUserWorkFlow(HttpClient httpClient, User user)
        {
            _httpClient = httpClient;
            _user = user;
        }

        public async Task<User> ExecuteAsync()
        {
            Console.Clear();

            Console.WriteLine("UPDATE USER");
            Console.WriteLine();
            Thread.Sleep(250);

            Console.WriteLine("New first name?");
            _user.FirstName = Console.ReadLine();

            Console.WriteLine("New last name?");
            _user.LastName = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Updating user...");

            var result = await _httpClient.PutAsync("/users", new JsonContent(_user));

            Console.WriteLine("Successfully updated user!");

            return JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());
        }
    }
}
