using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicDrone.Data;

namespace MusicDrone.API
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using var appContext = services.GetRequiredService<MusicDroneDbContext>();
                appContext.Database.Migrate();
            }

            return host;
        }
    }
}
