using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MusicDrone.Data;
using MusicDrone.IntegrationTests.Tests.Custom;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MusicDrone.IntegrationTests.Tests
{
    public abstract class BaseControllerTests : IClassFixture<CustomWebApplicationFactory<API.Startup>>, IDisposable
    {
        protected readonly HttpClient _client;
        protected readonly CustomWebApplicationFactory<API.Startup> _factory;
        protected readonly MusicDroneDbContext _context;
        protected readonly IServiceScope _scope;

        public BaseControllerTests(string databaseName)
        {
            _factory = new CustomWebApplicationFactory<API.Startup>()
            {
                DatabaseName = databaseName
            };
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<MusicDroneDbContext>();
        }

        public virtual void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context?.Dispose();
            _scope?.Dispose();
            _factory?.Dispose();
            _client?.Dispose();
        }

        protected async Task SaveEntity<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
    }
}
