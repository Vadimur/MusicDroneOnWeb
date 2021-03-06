using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicDrone.Data;
using MusicDrone.Data.Constants;
using MusicDrone.Data.Models;
using MusicDrone.IntegrationTests.ControllerServicesTests.Custom.Authentication;
using MusicDrone.IntegrationTests.ControllerServicesTests.Data;
using MusicDrone.IntegrationTests.Shared;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MusicDrone.IntegrationTests.ControllerServicesTests.Custom
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public string DatabaseName { get; set; } = Guid.NewGuid().ToString();

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
                    options.UseInMemoryDatabase(DatabaseName);
                });

                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MusicDroneDbContext>();

                context.Database.EnsureCreated();
            });
        }

        public HttpClient CreateClientWithTestAuth(TestClaimsProvider claimsProvider)
        {
            var client = WithAuthentication(claimsProvider)
                        .CreateClient(new WebApplicationFactoryClientOptions
                        {
                            AllowAutoRedirect = false
                        });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ControllerTestsConstants.AuthenticationType);

            return client;
        }

        public HttpClient CreateClientWithTestAuth(Guid guid, string username, string password, string role)
        {
            var user = SharedTestData.CreateTestUser(guid, username, password);
            var userProvider = TestClaimsProvider.FromApplicationUser(user, role);

            var client = CreateClientWithTestAuth(userProvider);

            return client;
        }

        public HttpClient CreateClientWithTestAuth(ApplicationUser user, string role = null)
        {
            role ??= Roles.USERS;
            var userProvider = TestClaimsProvider.FromApplicationUser(user, role);
            var client = CreateClientWithTestAuth(userProvider);

            return client;
        }

        private WebApplicationFactory<TStartup> WithAuthentication(TestClaimsProvider claimsProvider)
        {
            return WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<TestClaimsProvider>(_ => claimsProvider);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = ControllerTestsConstants.AuthenticationType;
                        options.DefaultScheme = ControllerTestsConstants.AuthenticationType;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(ControllerTestsConstants.AuthenticationType, options => { });

                    services.AddAuthorization(options =>
                    {
                        options.DefaultPolicy = new AuthorizationPolicyBuilder(ControllerTestsConstants.AuthenticationType).RequireAuthenticatedUser().Build();
                    });
                });
            });
        }
    }
}
