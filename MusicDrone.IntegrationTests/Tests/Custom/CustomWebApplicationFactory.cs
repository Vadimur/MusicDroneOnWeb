using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicDrone.Data;
using System;
using System.Linq;

namespace MusicDrone.IntegrationTests.Tests.Custom
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<MusicDroneDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<MusicDroneDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDatabase"); // Guid.NewGuid().ToString()
                });

                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MusicDroneDbContext>();

                context.Database.EnsureCreated();
            });
        }
    }
}
