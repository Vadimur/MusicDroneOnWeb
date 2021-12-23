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
    public class RoomControllerTests : BaseControllerTests
    {
        public RoomControllerTests() : base(nameof(RoomControllerTests))
        {

        }

        [Fact]
        public async Task Create_ValidRequest_RoomCreatedValidResponse()
        {
            //Arrange
            var userId = new Guid("e6be0075-db0b-46c8-a715-57c2c8cb4bbc");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoom123";
            var request = new RoomCreateRequestModel
            {
                Name = roomName
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomEndpoints.Create, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.LocalPath.Should().MatchEquivalentOf($"{ApiEndpoints.RoomEndpoints.Base}*");
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<RoomCreateRequestModel>(responseContent);
            responseData.Name.Should().BeEquivalentTo(roomName);
        }

        [Fact]
        public async Task Create_ValidRequest_RoomCreatedInDatabase()
        {
            //Arrange
            var userId = new Guid("0e66e1b7-198c-44bc-a353-d6460390100d");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var preTestRoomsCount = _context.Rooms.Count();

            var roomName = "TestRoom123";
            var request = new RoomCreateRequestModel
            {
                Name = roomName
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomEndpoints.Create, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            var responseData = JsonConvert.DeserializeObject<RoomCreateRequestModel>(responseContent);
            var roomId = new Guid(response.Headers.Location.LocalPath.Split('/').Last());
            
            _context.Rooms.Count().Should().Be(preTestRoomsCount + 1);
            var rooms = _context.Rooms.Where(r => r.Id == roomId).ToList();
            rooms.Count.Should().Be(1);
            var room = rooms.Single();
            room.Name.Should().Be(roomName);
        }

        [Fact]
        public async Task Create_ValidRequest_RoomTiedWithCreatorInDatabase()
        {
            //Arrange
            var userId = new Guid("2dfadefd-4f3b-4ab3-8336-3c9af2cf221a");
            var username = "TestCreateRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var preTestUserRoomPairsCount = _context.RoomsUsers.Count();

            var roomName = "TestRoom123";
            var request = new RoomCreateRequestModel
            {
                Name = roomName
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomEndpoints.Create, request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            var responseData = JsonConvert.DeserializeObject<RoomCreateRequestModel>(responseContent);
            var roomId = new Guid(response.Headers.Location.LocalPath.Split('/').Last());

            _context.RoomsUsers.Count().Should().Be(preTestUserRoomPairsCount + 1);
            var newRoomUserPairs = _context.RoomsUsers.Where(p => p.UserId == userId && p.RoomId == roomId).ToList();
            newRoomUserPairs.Count.Should().Be(1);
            newRoomUserPairs.Single().Role.Should().Be(RoomUserRole.Owner);
        }

        [Fact]
        public async Task Create_InvalidRequest_BadRequestResponse()
        {
            //Arrange
            var preTestRoomsCount = _context.RoomsUsers.Count();

            var request = new RoomCreateRequestModel();
            
            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomEndpoints.Create, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _context.Rooms.Count().Should().Be(preTestRoomsCount);
        }

        [Theory]
        [MemberData(nameof(AccountTestDataGenerator.ExistingRooms), MemberType = typeof(AccountTestDataGenerator))]
        public async Task GetAll_ValidRequest_AllRoomsReturned(List<Room> existingRooms)
        {
            //Arrange
            Task.WaitAll(existingRooms.Select(r => SaveEntity(r)).ToArray());
            var roomsCount = _context.Rooms.Count();

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.RoomEndpoints.GetAll);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<List<RoomResponseModel>>(responseContent);
            responseData.Count().Should().Be(roomsCount);
        }

        [Fact]
        public async Task GetRoom_ExistingRoomId_RoomInformationInResponse()
        {
            //Arrange
            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);

            var roomId = _context.Rooms.Single().Id.ToString();

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(roomId);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<RoomResponseModel>(responseContent);
            responseData.Id.Should().Be(roomId);
            responseData.Name.Should().Be(roomName);
        }

        [Fact]
        public async Task GetRoom_UnexistingRoomId_NotFoundResponse()
        {
            //Arrange
            var randomRoomId = "9e2e0119-a90a-4786-880a-8c3900f71ae6";

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(randomRoomId);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetRoom_InvalidRoomId_NotFoundResponse()
        {
            //Arrange
            var invalidRoomId = "xx_notvalidid_xx";

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(invalidRoomId);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteRoom_ExistingRoom_ResponseIsValid()
        {
            //Arrange
            var userId = new Guid("6b2e1fd0-a2f6-4288-a3b8-fda954971dd1");
            var username = "TestDeleteRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomName = "TestRoomName";
            var existingRoom = new Room()
            {
                Name = roomName
            };
            await SaveEntity(existingRoom);
            var roomId = _context.Rooms.Single().Id.ToString();

            var roomUserPair = new RoomUser() 
            { 
                RoomId = new Guid(roomId),
                UserId = userId,
                Role = RoomUserRole.Owner
            };
            await SaveEntity(roomUserPair);

            var request = new RoomDeleteRequestModel
            {
                Id = roomId
            };

            var client = _factory.CreateClientWithTestAuth(user);
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(request.Id);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, apiEndpoint, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteRoom_ExistingRoom_RoomIsDeletedFromDatabase()
        {
            //Arrange
            var userId = new Guid("6b2e1fd0-a2f6-4288-a3b8-fda954971dd1");
            var username = "TestDeleteRoomUsername";
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

            var request = new RoomDeleteRequestModel
            {
                Id = roomId.ToString()
            };

            var roomsCount = _context.Rooms.Count();
            var usersCount = _context.Users.Count();

            var client = _factory.CreateClientWithTestAuth(user);
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(request.Id);

            //Act
            await client.SendTestRequest(HttpMethod.Delete, apiEndpoint, request);

            //Assert
            _context.Rooms.Count().Should().Be(roomsCount - 1);
            _context.RoomsUsers.Count(r => r.Id == roomId).Should().Be(0);
            _context.Users.Count().Should().Be(usersCount);
        }

        [Fact]
        public async Task DeleteRoom_InvalidRoomId_ResponseIsValid()
        {
            //Arrange
            var userId = new Guid("4e584416-fdd1-4c92-a90e-eb808c8b0166");
            var username = "TestDeleteRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var invalidRoomId = "xx_notvalidid_xx";

            var request = new RoomDeleteRequestModel
            {
                Id = invalidRoomId
            };

            var client = _factory.CreateClientWithTestAuth(user);
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(request.Id);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, apiEndpoint, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteRoom_InvalidRoomId_DatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("0eed43e3-48b2-428a-8cef-5dbf68701c4a");
            var username = "TestDeleteRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var invalidRoomId = "xx_notvalidid_xx";

            var roomsCount = _context.Rooms.Count();
            var usersCount = _context.Users.Count();
            var roomUsersCount = _context.RoomsUsers.Count();

            var request = new RoomDeleteRequestModel
            {
                Id = invalidRoomId
            };

            var client = _factory.CreateClientWithTestAuth(user);
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(request.Id);

            //Act
            await client.SendTestRequest(HttpMethod.Delete, apiEndpoint, request);

            //Assert
            _context.Rooms.Count().Should().Be(roomsCount);
            _context.RoomsUsers.Count().Should().Be(roomUsersCount);
            _context.Users.Count().Should().Be(usersCount);
        }

        [Fact]
        public async Task DeleteRoom_UnexistingRoom_ResponseIsValid()
        {
            //Arrange
            var userId = new Guid("6b2e1fd0-a2f6-4288-a3b8-fda954971dd1");
            var username = "TestDeleteRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var randomRoomId = "226c67a4-72eb-4bc7-9220-f56aaef0285b";

            var request = new RoomDeleteRequestModel
            {
                Id = randomRoomId
            };

            var client = _factory.CreateClientWithTestAuth(user);
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(request.Id);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, apiEndpoint, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteRoom_UnexistingRoom_DatabaseRemainsTheSame()
        {
            //Arrange
            var userId = new Guid("6b2e1fd0-a2f6-4288-a3b8-fda954971dd1");
            var username = "TestDeleteRoomUsername";
            var password = TestConstants.DefaultTestPassword;
            var user = AccountTestDataGenerator.CreateTestUser(userId, username, password);
            await SaveEntity(user);

            var roomsCount = _context.Rooms.Count();
            var roomUsersCount = _context.RoomsUsers.Count();
            var usersCount = _context.Users.Count();

            var randomRoomId = "226c67a4-72eb-4bc7-9220-f56aaef0285b";

            var request = new RoomDeleteRequestModel
            {
                Id = randomRoomId
            };

            var client = _factory.CreateClientWithTestAuth(user);
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(request.Id);

            //Act
            await client.SendTestRequest(HttpMethod.Delete, apiEndpoint, request);

            //Assert
            _context.Rooms.Count().Should().Be(roomsCount);
            _context.RoomsUsers.Count().Should().Be(roomUsersCount);
            _context.Users.Count().Should().Be(usersCount);
        }
    }
}
