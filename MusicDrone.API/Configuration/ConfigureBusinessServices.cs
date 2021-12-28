using Microsoft.Extensions.DependencyInjection;
using MusicDrone.Business.Services;
using MusicDrone.Business.Services.Abstraction;

namespace MusicDrone.API.Configuration
{
    public static class ConfigureBusinessServices
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IRoomsUsersService, RoomsUsersService>();
            services.AddScoped<IMusicService, MusicService>();

            return services;
        }
    }
}