using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MicroPicking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(serverOptions =>
                    {
                        serverOptions.ListenLocalhost(6001);
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
