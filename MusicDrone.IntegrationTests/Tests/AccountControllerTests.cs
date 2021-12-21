using Microsoft.AspNetCore.Mvc.Testing;
using MusicDrone.API.Models.Requests;
using System.Net;
using System.Net.Http;
using MusicDrone.IntegrationTests.Helpers;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using MusicDrone.IntegrationTests.Tests.Data;
using MusicDrone.Data.Constants;
using MusicDrone.API.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Linq;
using MusicDrone.Data;
using Microsoft.Extensions.DependencyInjection;
using MusicDrone.IntegrationTests.Tests.Custom;
using MusicDrone.IntegrationTests.Tests.Custom.Authentication;
using MusicDrone.IntegrationTests.Tests.Custom.Authentication.Extensions;
using MusicDrone.Data.Models;

namespace MusicDrone.IntegrationTests.Tests
{
    public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory<API.Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<API.Startup> _factory;
        private readonly MusicDroneDbContext _context;
        private readonly IServiceScope _scope;

        public AccountControllerTests(CustomWebApplicationFactory<API.Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<MusicDroneDbContext>();

            //var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            //var scope = scopeFactory.CreateScope();
            //_context = scope.ServiceProvider.GetService<MusicDroneDbContext>();

            //var x = _context.Database.EnsureCreated();
            //var t = 1;
            /*var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<MusicDroneDbContext>();

            context.Database.EnsureCreated(); */
        }

        public void Dispose()
        {
            _client?.Dispose();
            _scope?.Dispose();
        }

        private async Task SaveUserAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.RegistrationBadRequests), MemberType = typeof(AccountTestDataGenerator))]
        public async Task Register_RequestWithoutRequiredProperties_BadRequest(RegisterRequest request)
        {
            //Arrange
            var usersCount = _context.Users.Count();

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.Register, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _context.Users.Count().Should().Be(usersCount);
        }

        [Fact]
        public async Task Register_ValidRequest_UserRegistered()
        {
            //Arrange
            var request = new RegisterRequest
            {
                Name = "Tribbiani",
                Surname = "Mancini",
                Email = "TribbianiMancini@gmail.com",
                Password = "Tribbian1Mancini_@"
            };
            var usersCount = _context.Users.Count();

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.Register, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var result = JsonConvert.DeserializeObject<RegisterResponse>(responseContent);
            result.Role.Should().BeEquivalentTo(Roles.USERS);
            _context.Users.Count().Should().Be(usersCount + 1);
        }

        [Fact]
        public async Task Register_ValidRequest_UsernameAlreadyTakenReponse()
        {
            //Arrange
            var userId = new Guid("8d8614c1-72cc-487d-a979-fd34a04d16ba");
            var username = "TestUser";
            var password = TestConstants.DefaultTestPassword;

            var existingUser = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveUserAsync(existingUser);

            var request = new RegisterRequest
            {
                Name = username,
                Surname = username,
                Email = username,
                Password = password
            };

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.Register, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            responseContent.Should().NotBeNull();
            responseContent.Should().ContainEquivalentOf($"Username '{username}' is already taken.");
            _context.Users.Find(userId).Should().NotBeNull();
        }

        [Theory]
        [InlineData("qwerty", "Passwords must have at least one non alphanumeric character.")]
        [InlineData("qwerty1", "Passwords must have at least one non alphanumeric character.")]
        [InlineData("qwerty_", "Passwords must have at least one digit ('0'-'9').")]
        [InlineData("qwerty1_", "Passwords must have at least one uppercase ('A'-'Z').")]
        [InlineData("QWERTY", "Passwords must have at least one non alphanumeric character.")]
        [InlineData("QWerty1", "Passwords must have at least one non alphanumeric character.")]
        [InlineData("QWerty_", "Passwords must have at least one digit ('0'-'9').")]
        [InlineData("QWERTY1_", "Passwords must have at least one lowercase ('a'-'z').")]
        public async Task Register_InvalidPassword_ErrorMessageReturned(string invalidPassword, string errorMessage)
        {
            //Arrange
            var request = new RegisterRequest
            {
                Name = "Tribbiani",
                Surname = "Mancini",
                Email = "TribbianiMancini@gmail.com",
                Password = invalidPassword
            };
            var usersCount = _context.Users.Count();

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.Register, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            responseContent.Should().NotBeNull();
            responseContent.Should().BeEquivalentTo(errorMessage);
            _context.Users.Count().Should().Be(usersCount);
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.LoginBadRequests), MemberType = typeof(AccountTestDataGenerator))]
        public async Task Login_RequestWithoutRequiredProperties_BadRequest(LoginRequest request)
        {
            //Arrange
            var usersCount = _context.Users.Count();

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.Login, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _context.Users.Count().Should().Be(usersCount);
        }

        [Fact]
        public async Task Login_ValidRequest_UserDoesntExist()
        {
            //Arrange
            var request = new LoginRequest
            {
                Login = "test_test",
                Password = "VerySecretP@assw0rd__!"
            };

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.Login, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseContent.Should().NotBeNull();
            responseContent.Should().BeEquivalentTo("Invalid login or password");
        }

        [Fact]
        public async Task Login_ValidRequest_UserSuccessfulLogin()
        {
            //Arrange
            var userId = new Guid("134f56ee-89ec-4037-a485-c80bd79b9679");
            var username = "TestUser";
            var password = TestConstants.DefaultTestPassword;

            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveUserAsync(user);

            var request = new LoginRequest
            {
                Login = username,
                Password = password
            };

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.Login, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
            responseData.Token.Should().NotBeNullOrWhiteSpace();
            _context.Users.Find(userId).Should().NotBeNull();
        }

        [Fact]
        public async Task Profile_UnauthorizedRequest_UnathorizedResponseStatusCode()
        {
            //Arrange
            var userId = new Guid("b756adc1-8451-4db1-835c-ee759f3463c8");
            var username = "UnathorizedProfileTestUser";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveUserAsync(user);

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Get, ApiEndpoints.Profile);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.UsersWithAuthorizationRoles), MemberType = typeof(AccountTestDataGenerator))]
        public async Task Profile_AuthorizedRequestUsersRole_UsersProfile(Guid userId, string username, string role)
        {
            //Arrange
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveUserAsync(user);

            var userProvider = TestClaimsProvider.FromApplicationUser(user, role);
            var client = _factory.CreateClientWithTestAuth(userProvider);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.Profile);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<ProfileResponse>(responseContent);
            responseData.FirstName.Should().BeEquivalentTo(user.FirstName);
            responseData.LastName.Should().BeEquivalentTo(user.LastName);
        }
    }
}
