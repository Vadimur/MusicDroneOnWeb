using System;
using System.IO;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicDrone.Data;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.Models;
using MusicDrone.Data.Repositories;
using MusicDrone.Data.Services;
using MusicDrone.Data.Services.Abstraction;

namespace MusicDrone.API.Configuration
{
    public static class ConfigureDataServices
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<MusicDroneDbContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<MusicDroneDbContext>(options =>
                options.UseSqlServer(connectionString));
            
            services.AddScoped<IAccountManagement, AccountManagement>();
            services.AddScoped<IFileService>(_ => new FileService(AppDomain.CurrentDomain.BaseDirectory));
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IRoomsUsersRepository, RoomsUsersRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMusicRepository, MusicRepository>();
            services.AddScoped<IFileDescRepository, FileDescRepository>();

            return services;
        }
    }
}