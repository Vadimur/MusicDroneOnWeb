using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MusicDrone.IntegrationTests.Tests.Data;

namespace MusicDrone.IntegrationTests.Tests.Custom.Authentication.Extensions
{
    public static class CreateClientExtension
    {
        public static WebApplicationFactory<T> WithAuthentication<T>(this CustomWebApplicationFactory<T> factory, TestClaimsProvider claimsProvider) where T : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<TestClaimsProvider>(_ => claimsProvider);
                    
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestConstants.AuthenticationType;
                        options.DefaultScheme = TestConstants.AuthenticationType;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestConstants.AuthenticationType, options => { });

                    services.AddAuthorization(options =>
                    {
                        options.DefaultPolicy = new AuthorizationPolicyBuilder(TestConstants.AuthenticationType).RequireAuthenticatedUser().Build();
                    });
                });
            });
        }

        public static HttpClient CreateClientWithTestAuth<T>(this CustomWebApplicationFactory<T> factory, TestClaimsProvider claimsProvider) where T : class
        {
            var client = factory.WithAuthentication(claimsProvider)
                        .CreateClient(new WebApplicationFactoryClientOptions
                        {
                            AllowAutoRedirect = false
                        });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestConstants.AuthenticationType);

            return client;
        }
    }
}
