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
using MusicDrone.IntegrationTests.Tests.Custom.Authentication;

namespace MusicDrone.IntegrationTests.Tests
{
    public class AccountControllerTests : BaseControllerTests
    {

        public AccountControllerTests() : base(nameof(AccountControllerTests)) 
        {

        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.RegistrationBadRequests), MemberType = typeof(AccountTestDataGenerator))]
        public async Task Register_RequestWithoutRequiredProperties_BadRequest(RegisterRequest request)
        {
            //Arrange
            var usersCount = _context.Users.Count();

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Register, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _context.Users.Count().Should().Be(usersCount);
        }

        [Fact]
        public async Task Register_ValidRequest_RegisteredAndResponseIsValid()
        {
            //Arrange
            var request = new RegisterRequest
            {
                Name = "Tribbiani",
                Surname = "Mancini",
                Email = "TribbianiMancini@gmail.com",
                Password = "Tribbian1Mancini_@"
            };

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Register, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseObject = JsonConvert.DeserializeObject<RegisterResponse>(responseContent);
            responseObject.Role.Should().BeEquivalentTo(Roles.USERS);
        }

        [Fact]
        public async Task Register_ValidRequest_RegisteredAndAddedToDatabase()
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
            var usersRoleId = _context.Roles.Single(r => r.Name == Roles.USERS).Id;

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Register, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            _context.Users.Count().Should().Be(usersCount + 1);
            var registeredUsers = _context.Users.Where(u => u.Email == request.Email).ToList();
            registeredUsers.Count.Should().Be(1);

            var registeredUser = registeredUsers.Single();
            var userRoles = _context.UserRoles.Where(p => p.UserId.Equals(registeredUser.Id)).ToList();
            userRoles.Count.Should().Be(1);
            userRoles.Single().RoleId.Should().Be(usersRoleId);
        }

        [Fact]
        public async Task Register_ValidRequest_UsernameAlreadyTakenReponse()
        {
            //Arrange
            var userId = new Guid("8d8614c1-72cc-487d-a979-fd34a04d16ba");
            var username = "TestUser";
            var password = TestConstants.DefaultTestPassword;

            var existingUser = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(existingUser);
            var usersCount = _context.Users.Count();

            var request = new RegisterRequest
            {
                Name = username,
                Surname = username,
                Email = username,
                Password = password
            };

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Register, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            responseContent.Should().NotBeNull();
            responseContent.Should().ContainEquivalentOf($"Username '{username}' is already taken.");

            _context.Users.Count().Should().Be(usersCount);
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
        public async Task Register_InvalidPasswordFormat_ErrorMessageReturned(string invalidPassword, string errorMessage)
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
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Register, request);
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
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Login, request);

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
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Login, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            responseContent.Should().NotBeNull();
            responseContent.Should().BeEquivalentTo("Invalid login or password");
        }

        [Fact]
        public async Task Login_ValidRequest_UserSuccessfullyLoginedAndReponseIsValid()
        {
            //Arrange
            var userId = new Guid("134f56ee-89ec-4037-a485-c80bd79b9679");
            var username = "TestUser";
            var password = TestConstants.DefaultTestPassword;

            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var request = new LoginRequest
            {
                Login = username,
                Password = password
            };

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Login, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
            responseData.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_ValidRequest_UserSuccessfullyLoginedAndSavedToDatabase()
        {
            //Arrange
            var userId = new Guid("134f56ee-89ec-4037-a485-c80bd79b9679");
            var username = "TestUser";
            var password = TestConstants.DefaultTestPassword;

            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var request = new LoginRequest
            {
                Login = username,
                Password = password
            };

            //Act
            await _client.SendTestRequest(HttpMethod.Post, ApiEndpoints.AccountEndpoints.Login, request);

            //Assert
            _context.Users.Find(userId).Should().NotBeNull();
            _context.Users.Count().Should().Be(2);
        }

        [Fact]
        public async Task Profile_UnauthorizedRequest_UnathorizedResponseStatusCode()
        {
            //Arrange
            var userId = new Guid("b756adc1-8451-4db1-835c-ee759f3463c8");
            var username = "UnathorizedProfileTestUser";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            //Act
            var response = await _client.SendTestRequest(HttpMethod.Get, ApiEndpoints.AccountEndpoints.Profile);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.UsersWithAuthorizationRoles), MemberType = typeof(AccountTestDataGenerator))]
        public async Task Profile_AuthorizedRequestUserExists_UsersProfileWasReturned(Guid userId, string username, string role)
        {
            //Arrange
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var userProvider = TestClaimsProvider.FromApplicationUser(user, role);
            var client = _factory.CreateClientWithTestAuth(userProvider);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.AccountEndpoints.Profile);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<ProfileResponse>(responseContent);
            responseData.FirstName.Should().BeEquivalentTo(user.FirstName);
            responseData.LastName.Should().BeEquivalentTo(user.LastName);
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.UsersWithAuthorizationRoles), MemberType = typeof(AccountTestDataGenerator))]
        public async Task Profile_AuthorizedRequestUserDoesntExist_NotFoundResponse(Guid userId, string username, string role)
        {
            //Arrange
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);

            var userProvider = TestClaimsProvider.FromApplicationUser(user, role);
            var client = _factory.CreateClientWithTestAuth(userProvider);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.AccountEndpoints.Profile);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
