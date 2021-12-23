using FluentAssertions;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Data.Models;
using MusicDrone.IntegrationTests.Helpers;
using MusicDrone.IntegrationTests.Tests.Custom.Authentication;
using MusicDrone.IntegrationTests.Tests.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MusicDrone.IntegrationTests.Tests
{
    public class RoomUserControllerTests : BaseControllerTests
    {
        public RoomUserControllerTests() : base(nameof(RoomUserControllerTests))
        {

        }

        [Fact]
        public async Task EnterRoom_ValidRequest_RoomEnteredValidResponse()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = roomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task EnterRoom_ValidRequest_RoomEnteredDatabaseUpdated()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;

            var preTestRoomsCount = _context.Rooms.Count();

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = roomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            var userRooms = _context.RoomsUsers.Count(p => p.UserId == userId);

            userRooms.Should().Be(1);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
        }

        [Fact]
        public async Task EnterRoom_UserAlreadyInRoom_ConflictResponse()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;

            var roomUserPair = new RoomUser()
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.Owner
            };
            await SaveEntity(roomUserPair);

            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUsersInRoom = _context.RoomsUsers.Count(p => p.RoomId == roomId);

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = roomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task EnterRoom_UserAlreadyInRoom_DatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;

            var roomUserPair = new RoomUser()
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.Owner
            };
            await SaveEntity(roomUserPair);

            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUsersInRoom = _context.RoomsUsers.Count(p => p.RoomId == roomId);

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = roomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            var userRooms = _context.RoomsUsers.Count(p => p.UserId == userId);

            userRooms.Should().Be(1);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count(p => p.RoomId == roomId).Should().Be(preTestUsersInRoom);
        }

        /*[Fact] // should be fixed
        public async Task EnterRoom_UnexistingRoom_NotFoundResponse()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var randomRoomId = new Guid("2b95067e-434c-431a-b95e-df61b23a3811");

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = randomRoomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact] // should be fixed
        public async Task EnterRoom_UnexistingRoom_DatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var randomRoomId = new Guid("0c82b861-a4ef-4328-9cf2-d7270834e22b");

            var preTestRoomsCount = _context.Rooms.Count();
            var preTestRoomUsers = _context.RoomsUsers.Count();

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = randomRoomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert

            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestRoomUsers);
        }*/

        [Fact]
        public async Task EnterRoom_InvalidRequest_BadRequestResponse()
        {
            //Arrange
            var request = new RoomsUsersCreateRequestModel();

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task EnterRoom_InvalidRoomId_NotFoundResponse()
        {
            //Arrange
            var roomId = "xx_invalidId_xx";
            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = roomId
            };

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.UsersInRoom), MemberType = typeof(AccountTestDataGenerator))]
        public async Task GetUsersInRoom_ExistingRoom_ReturnsEachUsersInfo(IEnumerable<ApplicationUser> usersInRoom)
        {
            //Arrange
            var userId = new Guid("2c2a3fa5-7918-43fe-9d56-c8782fa08761");
            var username = "TestGetUsersInRoomUser";
            var password = TestConstants.DefaultTestPassword;
            var authUser = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(authUser);

            Task.WaitAll(usersInRoom.Select(r => SaveEntity(r)).ToArray());

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;
            var usersInRoomCount = usersInRoom.Count();

            foreach (var user in usersInRoom)
            {
                var roomUserPair = new RoomUser()
                {
                    RoomId = roomId,
                    UserId = user.Id,
                    Role = RoomUserRole.Owner
                };
                await SaveEntity(roomUserPair);
            }

            var client = _factory.CreateClientWithTestAuth(authUser);
            var apiEndpoint = ApiEndpoints.RoomUserEndpoints.WithId(roomId.ToString());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<List<RoomsUsersGetByRoomIdResponseModel>>(responseContent);
            responseData.Count().Should().Be(usersInRoomCount);
            responseData.Select(u => u.FirstName).SequenceEqual(usersInRoom.Select(u => u.FirstName)).Should().BeTrue();
            responseData.Select(u => u.LastName).SequenceEqual(usersInRoom.Select(u => u.LastName)).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.UsersInRoom), MemberType = typeof(AccountTestDataGenerator))]
        public async Task GetUsersInRoom_ExistingRoom_DatabaseRemainsTheSame(IEnumerable<ApplicationUser> usersInRoom)
        {
            //Arrange
            var userId = new Guid("139a878f-98ff-488c-a910-67f6f59d4657");
            var username = "TestGetUsersInRoomUser";
            var password = TestConstants.DefaultTestPassword;
            var authUser = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(authUser);

            Task.WaitAll(usersInRoom.Select(r => SaveEntity(r)).ToArray());

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;
            var usersInRoomCount = usersInRoom.Count();

            foreach (var user in usersInRoom)
            {
                var roomUserPair = new RoomUser()
                {
                    RoomId = roomId,
                    UserId = user.Id,
                    Role = RoomUserRole.Owner
                };
                await SaveEntity(roomUserPair);
            }

            var preTestUsersCount = _context.Users.Count();
            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUserRoomsCount = _context.RoomsUsers.Count();

            var client = _factory.CreateClientWithTestAuth(authUser);
            var apiEndpoint = ApiEndpoints.RoomUserEndpoints.WithId(roomId.ToString());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            _context.Users.Count().Should().Be(preTestUsersCount);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Fact]
        public async Task GetUsersInRoom_UnexistingRoom_NotFoundResponse()
        {
            //Arrange
            var userId = new Guid("2c2a3fa5-7918-43fe-9d56-c8782fa08761");
            var username = "TestGetUsersInRoomUser";
            var password = TestConstants.DefaultTestPassword;
            var authUser = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(authUser);

            var unexistingRoomId = new Guid("23bf5cfd-b25d-4e18-9983-527275906f1a");

            var client = _factory.CreateClientWithTestAuth(authUser);
            var apiEndpoint = ApiEndpoints.RoomUserEndpoints.WithId(unexistingRoomId.ToString());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUsersInRoom_UnexistingRoom_DatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("139a878f-98ff-488c-a910-67f6f59d4657");
            var username = "TestGetUsersInRoomUser";
            var password = TestConstants.DefaultTestPassword;
            var authUser = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(authUser);

            var unexistingRoomId = new Guid("b85a7f67-9d06-44e3-aa64-9dc82cccd5a1");

            var preTestUsersCount = _context.Users.Count();
            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUserRoomsCount = _context.RoomsUsers.Count();

            var client = _factory.CreateClientWithTestAuth(authUser);
            var apiEndpoint = ApiEndpoints.RoomUserEndpoints.WithId(unexistingRoomId.ToString());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            _context.Users.Count().Should().Be(preTestUsersCount);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.UserAndRooms), MemberType = typeof(AccountTestDataGenerator))]
        public async Task GetRoomsByUser_ValidRequest_ReturnsUserRoomsInfo(ApplicationUser user, IEnumerable<Room> rooms)
        {
            //Arrange
            await SaveEntity(user);
            Task.WaitAll(rooms.Select(r => SaveEntity(r)).ToArray());

            var userRoomsCount = rooms.Count();

            foreach (var room in rooms)
            {
                var roomUserPair = new RoomUser
                {
                    RoomId = room.Id,
                    UserId = user.Id,
                    Role = RoomUserRole.User
                };

                await SaveEntity(roomUserPair);
            }

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.RoomUserEndpoints.UserRooms);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<List<RoomsUsersGetByUserIdResponseModel>>(responseContent);
            responseData.Count().Should().Be(userRoomsCount);
            responseData.Select(r => r.RoomName).SequenceEqual(rooms.Select(r => r.Name)).Should().BeTrue();
            responseData.Select(r => r.RoomId).SequenceEqual(rooms.Select(r => r.Id.ToString())).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.UserAndRooms), MemberType = typeof(AccountTestDataGenerator))]
        public async Task GetRoomsByUser_ValidRequest_DatabaseRemainsTheSame(ApplicationUser user, IEnumerable<Room> rooms)
        {
            //Arrange
            await SaveEntity(user);
            Task.WaitAll(rooms.Select(r => SaveEntity(r)).ToArray());

            var userRoomsCount = rooms.Count();

            foreach (var room in rooms)
            {
                var roomUserPair = new RoomUser
                {
                    RoomId = room.Id,
                    UserId = user.Id,
                    Role = RoomUserRole.User
                };

                await SaveEntity(roomUserPair);
            }

            var preTestUsersCount = _context.Users.Count();
            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUserRoomsCount = _context.RoomsUsers.Count();

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.RoomUserEndpoints.UserRooms);

            //Assert
            _context.Users.Count().Should().Be(preTestUsersCount);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Fact]
        public async Task GetRoomsByUser_UnexistingUser_NotFoundResponse()
        {
            //Arrange
            var userId = new Guid("165e63e9-0ba6-4390-8244-ca745f2061f5");
            var username = "TestGetUsersInRoomUser";
            var password = TestConstants.DefaultTestPassword;
            var authUser = AccountTestDataGenerator.CreateTestUser(userId, username, password);
  
            var client = _factory.CreateClientWithTestAuth(authUser);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.RoomUserEndpoints.UserRooms);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetRoomsByUser_UnexistingUser_DatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("165e63e9-0ba6-4390-8244-ca745f2061f5");
            var username = "TestGetUsersInRoomUser";
            var password = TestConstants.DefaultTestPassword;
            var authUser = AccountTestDataGenerator.CreateTestUser(userId, username, password);

            var preTestUsersCount = _context.Users.Count();
            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUserRoomsCount = _context.RoomsUsers.Count();

            var client = _factory.CreateClientWithTestAuth(authUser);

            //Act
            await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.RoomUserEndpoints.UserRooms);

            //Assert
            _context.Users.Count().Should().Be(preTestUsersCount);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Fact]
        public async Task ExitRoom_UserExits_NoContentResponse()
        {
            //Arrange
            var userId = new Guid("15f850c4-4d53-4725-8ae4-f8a0399157b9");
            var username = "TestExitRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;

            var userRoomPair = new RoomUser
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.User
            };
            await SaveEntity(userRoomPair);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = roomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task ExitRoom_UserExits_RoomUserDeletedDatabaseUpdated()
        {
            //Arrange
            var userId = new Guid("e0137077-f19a-4cbf-8470-a7341bef5782");
            var username = "TestExitRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;

            var userRoomPair = new RoomUser
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.User
            };
            await SaveEntity(userRoomPair);

            var preTestUsersCount = _context.Users.Count();
            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUserRoomsCount = _context.RoomsUsers.Count();

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = roomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            _context.Users.Count().Should().Be(preTestUsersCount);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount - 1);
        }

        [Fact]
        public async Task ExitRoom_RoomOwner_ForbiddenResponse()
        {
            //Arrange
            var userId = new Guid("53196b7b-5696-4d13-817a-c6a87d6157b8");
            var username = "TestExitRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;

            var userRoomPair = new RoomUser
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.Owner
            };
            await SaveEntity(userRoomPair);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = roomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ExitRoom_RoomOwner_CannotExitRoomDatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("53196b7b-5696-4d13-817a-c6a87d6157b8");
            var username = "TestExitRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;

            var userRoomPair = new RoomUser
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.Owner
            };
            await SaveEntity(userRoomPair);

            var preTestUsersCount = _context.Users.Count();
            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUserRoomsCount = _context.RoomsUsers.Count();

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = roomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            _context.Users.Count().Should().Be(preTestUsersCount);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Fact]
        public async Task ExitRoom_RoomUserTryingToExitAnotherRoom_NotFoundResponse()
        {
            //Arrange
            var userId = new Guid("31c0470a-89d7-4604-a1d5-5cdbf0e04f9e");
            var username = "TestExitRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var usersRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(usersRoom);
            var usersRoomId = usersRoom.Id;

            var anotherRoom = new Room()
            {
                Name = "AnotherTestRoom"
            };
            await SaveEntity(anotherRoom);

            var notExistingRoomId = "18b25d12-9937-4f32-ac16-65de05822343";

            var userRoomPair = new RoomUser
            {
                RoomId = usersRoomId,
                UserId = userId,
                Role = RoomUserRole.User
            };
            await SaveEntity(userRoomPair);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = notExistingRoomId
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExitRoom_RoomUserTryingToExitAnotherRoom_CannotExitRoomDatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("31c0470a-89d7-4604-a1d5-5cdbf0e04f9e");
            var username = "TestExitRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;
            var notExistingRoomId = "18b25d12-9937-4f32-ac16-65de05822343";

            var userRoomPair = new RoomUser
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.User
            };
            await SaveEntity(userRoomPair);

            var preTestUsersCount = _context.Users.Count();
            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUserRoomsCount = _context.RoomsUsers.Count();
            var preTestUsersInRoomsCount = _context.RoomsUsers.Count(p => p.RoomId == roomId);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = notExistingRoomId
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            _context.Users.Count().Should().Be(preTestUsersCount);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
            _context.RoomsUsers.Count(p => p.RoomId == roomId).Should().Be(preTestUsersInRoomsCount);
        }

        [Fact]
        public async Task ExitRoom_RoomUserTryingToExitUnexistingRoom_NotFoundResponse()
        {
            //Arrange
            var userId = new Guid("fc46f932-cea3-4089-a1e0-f6f428ac0768");
            var username = "TestExitRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;
            var notExistingRoomId = "0b80deca-296d-4894-b55b-f44759aff2c6";

            var userRoomPair = new RoomUser
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.User
            };
            await SaveEntity(userRoomPair);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = notExistingRoomId
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExitRoom_RoomUserTryingToExitUnexistingRoom_DatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("1d4fb4d9-42ee-40fa-9127-710c746e6e87");
            var username = "TestExitRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = existingRoom.Id;
            var notExistingRoomId = "f16babfb-0a91-4ac5-a508-30caecee0869";

            var userRoomPair = new RoomUser
            {
                RoomId = roomId,
                UserId = userId,
                Role = RoomUserRole.User
            };
            await SaveEntity(userRoomPair);

            var preTestUsersCount = _context.Users.Count();
            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUserRoomsCount = _context.RoomsUsers.Count();
            var preTestUsersInRoomsCount = _context.RoomsUsers.Count(p => p.RoomId == roomId);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = notExistingRoomId
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            _context.Users.Count().Should().Be(preTestUsersCount);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
            _context.RoomsUsers.Count(p => p.RoomId == roomId).Should().Be(preTestUsersInRoomsCount);
        }
    }
}
