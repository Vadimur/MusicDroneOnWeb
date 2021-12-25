using FluentAssertions;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Data.Models;
using MusicDrone.IntegrationTests.ControllerServicesTests.Custom.Authentication;
using MusicDrone.IntegrationTests.ControllerServicesTests.Helpers;
using MusicDrone.IntegrationTests.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MusicDrone.IntegrationTests.ControllerServicesTests.Tests
{
    public class RoomUserControllerTests : BaseControllerTests
    {
        public RoomUserControllerTests() : base(nameof(RoomUserControllerTests))
        {

        }

        [Fact] //TODO should be fixed. unexistig user cannot enter room
        public async Task EnterRoom_ValidRequest_RoomEnteredOkResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(existingRoom);

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = existingRoom.Id.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task EnterRoom_UserAlreadyInRoom_ConflictResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(existingRoom);

            var roomUserPair = new RoomUser
            {
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.Owner
            };
            await _context.SaveEntity(roomUserPair);

            var preTestRoomsCount = _context.Rooms.Count();
            var preTestUsersInRoom = _context.RoomsUsers.Count(p => p.RoomId == existingRoom.Id);

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = existingRoom.Id.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task EnterRoom_UnexistingRoom_NotFoundResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = new Guid("2b95067e-434c-431a-b95e-df61b23a3811").ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

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
            var request = new RoomsUsersCreateRequestModel
            {
                RoomId = "xx_invalidId_xx"
            };

            var client = _factory.CreateClientWithTestAuth(TestClaimsProvider.WithUserClaims());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Post, ApiEndpoints.RoomUserEndpoints.EnterRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [MemberData(nameof(SharedTestData.UserListsNotEmpty), MemberType = typeof(SharedTestData))]
        [MemberData(nameof(SharedTestData.UsersListEmpty), MemberType = typeof(SharedTestData))]
        public async Task GetUsersInRoom_ExistingRoom_ReturnsUsersInfo(IEnumerable<ApplicationUser> usersInRoom)
        {
            //Arrange
            var authUser = SharedTestData.DefaultUser;

            await _context.SaveEntityRange(usersInRoom);

            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(existingRoom);
            var usersInRoomCount = usersInRoom.Count();

            foreach (var user in usersInRoom)
            {
                var roomUserPair = new RoomUser
                {
                    RoomId = existingRoom.Id,
                    UserId = user.Id,
                    Role = RoomUserRole.Owner
                };
                await _context.SaveEntity(roomUserPair);
            }

            var client = _factory.CreateClientWithTestAuth(authUser);
            var apiEndpoint = ApiEndpoints.RoomUserEndpoints.WithId(existingRoom.Id.ToString());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<List<RoomsUsersGetByRoomIdResponseModel>>(responseContent);
            responseData.Count.Should().Be(usersInRoomCount);
            responseData.Select(u => u.FirstName).SequenceEqual(usersInRoom.Select(u => u.FirstName)).Should().BeTrue();
            responseData.Select(u => u.LastName).SequenceEqual(usersInRoom.Select(u => u.LastName)).Should().BeTrue();
        }

        [Fact]
        public async Task GetUsersInRoom_UnexistingRoom_NotFoundResponse()
        {
            //Arrange
            var authUser = SharedTestData.DefaultUser;

            var unexistingRoomId = new Guid("23bf5cfd-b25d-4e18-9983-527275906f1a");

            var client = _factory.CreateClientWithTestAuth(authUser);
            var apiEndpoint = ApiEndpoints.RoomUserEndpoints.WithId(unexistingRoomId.ToString());

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, apiEndpoint);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [MemberData(nameof(SharedTestData.RoomListsNotEmpty), MemberType = typeof(SharedTestData))]
        [MemberData(nameof(SharedTestData.RoomsListEmpty), MemberType = typeof(SharedTestData))]
        public async Task GetRoomsByUser_ValidRequest_ReturnsUserRoomsInfo(IEnumerable<Room> rooms)
        {
            //Arrange
            var user = SharedTestData.DefaultUser;
            await _context.SaveEntity(user);

            await _context.SaveEntityRange(rooms);

            var userRoomsCount = rooms.Count();

            foreach (var room in rooms)
            {
                var roomUserPair = new RoomUser
                {
                    RoomId = room.Id,
                    UserId = user.Id,
                    Role = RoomUserRole.User
                };

                await _context.SaveEntity(roomUserPair);
            }

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.RoomUserEndpoints.UserRooms);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().NotBeNull();
            var responseData = JsonConvert.DeserializeObject<List<RoomsUsersGetByUserIdResponseModel>>(responseContent);
            responseData.Count.Should().Be(userRoomsCount);
            responseData.Select(r => r.RoomName).SequenceEqual(rooms.Select(r => r.Name)).Should().BeTrue();
            responseData.Select(r => r.RoomId).SequenceEqual(rooms.Select(r => r.Id.ToString())).Should().BeTrue();
        }

        [Fact]
        public async Task GetRoomsByUser_UnexistingUser_NotFoundResponse()
        {
            //Arrange
            var authUser = SharedTestData.DefaultUser;

            var client = _factory.CreateClientWithTestAuth(authUser);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Get, ApiEndpoints.RoomUserEndpoints.UserRooms);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        } 

        [Fact]
        public async Task ExitRoom_UserInRoom_NoContentResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(existingRoom);

            var userRoomPair = new RoomUser
            {
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.User
            };
            await _context.SaveEntity(userRoomPair);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = existingRoom.Id.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
     
        [Fact]
        public async Task ExitRoom_UserInRoomWithOwnerRole_ForbiddenResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(existingRoom);

            var userRoomPair = new RoomUser
            {
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.Owner
            };
            await _context.SaveEntity(userRoomPair);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = existingRoom.Id.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ExitRoom_RoomUserTryingToExitAnotherRoom_NotFoundResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var usersRoom = new Room
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(usersRoom);

            var anotherRoom = new Room
            {
                Name = "AnotherTestRoom"
            };
            await _context.SaveEntity(anotherRoom);

            var userRoomPair = new RoomUser
            {
                RoomId = usersRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.User
            };
            await _context.SaveEntity(userRoomPair);

            var request = new RoomsUsersDeleteRequestModel
            {
                RoomId = anotherRoom.Id.ToString()
            };

            var client = _factory.CreateClientWithTestAuth(user);

            //Act
            var response = await client.SendTestRequest(HttpMethod.Delete, ApiEndpoints.RoomUserEndpoints.ExitRoom, request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExitRoom_RoomUserTryingToExitUnexistingRoom_NotFoundResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await _context.SaveEntity(existingRoom);

            var userRoomPair = new RoomUser
            {
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.User
            };
            await _context.SaveEntity(userRoomPair);

            var notExistingRoomId = "0b80deca-296d-4894-b55b-f44759aff2c6";
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
    }
}
