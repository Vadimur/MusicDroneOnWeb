using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicDrone.Data;
using MusicDrone.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MusicDrone.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                for (int retries = 10; retries > 0; retries--)
                {
                    try
                    {
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                        await MusicDroneDbContextSeed.SeedAsync(userManager, roleManager);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString(), "An error occurred seeding the IdentityDB.");
                        Thread.Sleep(5000);
                    }
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
