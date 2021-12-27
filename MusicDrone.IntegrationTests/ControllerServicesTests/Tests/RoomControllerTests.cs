using FluentAssertions;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Data.Models;
using MusicDrone.IntegrationTests.ControllerServicesTests.Custom.Authentication;
using MusicDrone.IntegrationTests.ControllerServicesTests.Helpers;
using MusicDrone.IntegrationTests.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MusicDrone.IntegrationTests.ControllerServicesTests.Tests
{
    public class RoomControllerTests : BaseControllerTests
    {
        public RoomControllerTests() : base(nameof(RoomControllerTests))
        {

        }

        /*[Fact]
        public async Task Create_ValidRequest_CreatedResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var request = new RoomCreateRequestModel
            {
                Name = "TestRoom123"
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
            responseData.Name.Should().BeEquivalentTo(request.Name);
        }*/

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
        [MemberData(nameof(SharedTestData.RoomListsNotEmpty), MemberType = typeof(SharedTestData))]
        [MemberData(nameof(SharedTestData.RoomsListEmpty), MemberType = typeof(SharedTestData))]
        public async Task GetAll_ValidRequest_AllRoomsReturned(List<Room> existingRooms)
        {
            //Arrange
            await _context.SaveEntityRange(existingRooms);
            var roomsCount = _context.Rooms.Count();

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.RoomEndpoints.GetAll);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<List<RoomResponseModel>>(responseContent);
            responseData.Count.Should().Be(roomsCount);
        }

        [Fact]
        public async Task GetRoom_ExistingRoomId_RoomInformationInResponse()
        {
            //Arrange
            var existingRoom = new Room()
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(existingRoom);

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(existingRoom.Id.ToString());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<RoomResponseModel>(responseContent);
            responseData.Id.Should().Be(existingRoom.Id.ToString());
            responseData.Name.Should().Be(existingRoom.Name);
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
        public async Task DeleteRoom_ExistingRoom_NoContentResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room()
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(existingRoom);

            var roomUserPair = new RoomUser() 
            { 
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.Owner
            };
            await _context.SaveEntity(roomUserPair);

            var request = new RoomDeleteRequestModel
            {
                Id = existingRoom.Id.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(request.Id);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, apiEndpoint, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }   

        [Fact]
        public async Task DeleteRoom_InvalidRoomId_NotFoundResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var request = new RoomDeleteRequestModel
            {
                Id = "xx_notvalidid_xx"
            };

            var client = _factory.CreateClientWithTestAuth(user);
            var apiEndpoint = ApiEndpoints.RoomEndpoints.WithId(request.Id);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, apiEndpoint, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteRoom_UnexistingRoom_NotFoundResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

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
    }
}
