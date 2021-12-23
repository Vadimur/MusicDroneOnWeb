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
            var roomId = _context.Rooms.Single().Id;

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
            var roomId = _context.Rooms.Single().Id;

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
            var roomId = _context.Rooms.Single().Id;

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
            var roomId = _context.Rooms.Single().Id;

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

        /*[Fact]
        public async Task EnterRoom_UnexistingRoom_ConflictResponse()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var randomRoomId = new Guid("0d862327-cb0a-4fb0-9e57-5cd3ceafc486");

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = randomRoomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task EnterRoom_UnexistingRoom_DatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var randomRoomId = new Guid("b1b40625-ccf9-4603-9acd-ce3917da2e41");

            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUsersInRoom = _context.RoomsUsers.Count();

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = randomRoomId.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert

            _context.Rooms.Count().Should().Be(preTestRoomsCount);
            _context.RoomsUsers.Count().Should().Be(preTestUsersInRoom);
        }*/

        [Fact]
        public async Task EnterRoom_IvalidRequest_BadRequestResponse()
        {
            //Arrange
            var request = new RoomsUsersCreateRequestModel();

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
            var roomId = _context.Rooms.Single().Id;
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
            var roomId = _context.Rooms.Single().Id;
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

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.UserAndRooms), MemberType = typeof(AccountTestDataGenerator))]
        public async Task GetnRoomsByUser_ValidRequest_ReturnsUserRoomsInfo(ApplicationUser user, IEnumerable<Room> rooms)
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
        public async Task GetnRoomsByUser_ValidRequest_DatabaseRemainsTheSame(ApplicationUser user, IEnumerable<Room> rooms)
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
    }
}
