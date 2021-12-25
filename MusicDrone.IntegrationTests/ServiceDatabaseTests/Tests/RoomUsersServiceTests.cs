using FluentAssertions;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Services;
using MusicDrone.Data.Models;
using MusicDrone.IntegrationTests.ServiceDatabaseTests.Tests.Base;
using MusicDrone.IntegrationTests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MusicDrone.IntegrationTests.ServiceDatabaseTests.Tests
{
    public class RoomUsersServiceTests : TestsBase
    {
        private readonly RoomsUsersService _testService;
        public RoomUsersServiceTests(BaseTestsFixture fixture) : base(fixture)
        {
            _testService = new RoomsUsersService(Context);
        }

        [Fact]
        public async Task EnterRoom_ExistingRoom_RoomEnteredInUserRole()
        {
            //Arrange
            var testUser = SharedTestData.DefaultUser;
            var existingRoom = new Room()
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            var preTestRoomsCount = Context.Rooms.Count();

            var request = new RoomsUsersCreateRequestDto
            {
                RoomId = existingRoom.Id,
                UserId = testUser.Id
            };

            //Act
            var response = await _testService.CreateAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.Exists.Should().BeFalse();

            var userRooms = Context.RoomsUsers.Count(p => p.UserId == testUser.Id);
            userRooms.Should().Be(1);
            Context.Rooms.Count().Should().Be(preTestRoomsCount);
        }

        [Theory]
        [InlineData(RoomUserRole.User)]
        [InlineData(RoomUserRole.Owner)]
        [InlineData(RoomUserRole.Moderator)]
        public async Task EnterRoom_UserAlreadyInRoomAnyRole_ExistsTrueReturned(RoomUserRole role)
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room()
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            var roomUserPair = new RoomUser()
            {
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = role
            };
            await Context.SaveEntity(roomUserPair);

            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUsersInRoom = Context.RoomsUsers.Count(p => p.RoomId == existingRoom.Id);

            var request = new RoomsUsersCreateRequestDto
            {
                UserId = user.Id,
                RoomId = existingRoom.Id
            };

            //Act
            var response = await _testService.CreateAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.Exists.Should().BeTrue();

            Context.RoomsUsers.Count(p => p.UserId == user.Id).Should().Be(1);
            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count(p => p.RoomId == existingRoom.Id).Should().Be(preTestUsersInRoom);
        }

        [Fact]
        public async Task EnterRoom_UnexistingRoom_NullResponse()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var unexstingRoomId = new Guid("0c82b861-a4ef-4328-9cf2-d7270834e22b");

            var preTestRoomsCount = Context.Rooms.Count();
            var preTestRoomUsers = Context.RoomsUsers.Count();

            var request = new RoomsUsersCreateRequestDto
            {
                UserId = user.Id,
                RoomId = unexstingRoomId
            };

            //Act
            var response = await _testService.CreateAsync(request);

            //Assert
            response.Should().BeNull();

            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestRoomUsers);
        }

        /*[Fact] // should be fixed
        public async Task EnterRoom_UnexistingUser_NullResponse()
        {
            //Arrange
            var unextingUserId = new Guid("7f9e937d-5b47-4c90-8894-dc78fc6e16ac");

            var existingRoom = new Room()
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            var preTestRoomsCount = Context.Rooms.Count();
            var preTestRoomUsers = Context.RoomsUsers.Count();

            var request = new RoomsUsersCreateRequestDto
            {
                UserId = unextingUserId,
                RoomId = existingRoom.Id
            };

            //Act
            var response = await _testService.CreateAsync(request);

            //Assert
            response.Should().BeNull();

            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestRoomUsers);
        }*/

        [Theory]
        [MemberData(nameof(SharedTestData.UserListsNotEmpty), MemberType = typeof(SharedTestData))]
        [MemberData(nameof(SharedTestData.UsersListEmpty), MemberType = typeof(SharedTestData))]
        public async Task GetUsersInRoom_FromExistingRoom_UsersFromRoomReturned(IEnumerable<ApplicationUser> usersInRoom)
        {
            //Arrange
            await Context.SaveEntityRange(usersInRoom);
            
            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            foreach (var user in usersInRoom)
            {
                var roomUserPair = new RoomUser
                {
                    RoomId = existingRoom.Id,
                    UserId = user.Id,
                    Role = RoomUserRole.Owner
                };
                await Context.SaveEntity(roomUserPair);
            }

            var usersInRoomCount = usersInRoom.Count();
            var preTestUsersCount = Context.Users.Count();
            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomsCount = Context.RoomsUsers.Count();

            var request = new RoomsUsersGetByRoomIdRequestDto
            {
                RoomId = existingRoom.Id
            };

            //Act
            var response = await _testService.GetAllInRoom(request);

            //Assert
            response.Should().NotBeNull();
            response.Count().Should().Be(usersInRoomCount);
            response.Select(u => u.FirstName).SequenceEqual(usersInRoom.Select(u => u.FirstName)).Should().BeTrue();
            response.Select(u => u.LastName).SequenceEqual(usersInRoom.Select(u => u.LastName)).Should().BeTrue();

            Context.Users.Count().Should().Be(preTestUsersCount);
            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Fact]
        public async Task GetUsersInRoom_UnexistingRoom_NullResponse()
        {
            //Arrange
            var unexistingRoomId = new Guid("b85a7f67-9d06-44e3-aa64-9dc82cccd5a1");

            var preTestUsersCount = Context.Users.Count();
            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomsCount = Context.RoomsUsers.Count();

            var request = new RoomsUsersGetByRoomIdRequestDto
            {
                RoomId = unexistingRoomId
            };

            //Act
            var response = await _testService.GetAllInRoom(request);

            //Assert
            response.Should().BeNull();

            Context.Users.Count().Should().Be(preTestUsersCount);
            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Theory]
        [MemberData(nameof(SharedTestData.RoomListsNotEmpty), MemberType = typeof(SharedTestData))]
        [MemberData(nameof(SharedTestData.RoomsListEmpty), MemberType = typeof(SharedTestData))]
        public async Task GetRoomsByUser_ExistingUser_UsersRoomReturned(IEnumerable<Room> rooms)
        {
            //Arrange
            var user = SharedTestData.DefaultUser;
            await Context.SaveEntityRange(rooms);

            foreach (var room in rooms)
            {
                var roomUserPair = new RoomUser
                {
                    RoomId = room.Id,
                    UserId = user.Id,
                    Role = RoomUserRole.User
                };

                await Context.SaveEntity(roomUserPair);
            }

            var userRoomsCount = rooms.Count();
            var preTestUsersCount = Context.Users.Count();
            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomsCount = Context.RoomsUsers.Count();

            var request = new RoomsUsersGetByUserIdRequestDto
            {
                UserId = user.Id
            };

            //Act
            var response = await _testService.GetAllRoomsByUser(request);

            //Assert
            response.Should().NotBeNull();
            response.Count().Should().Be(userRoomsCount);

            Context.Users.Count().Should().Be(preTestUsersCount);
            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Fact]
        public async Task GetRoomsByUser_UnexistingUser_NullResponse()
        {
            //Arrange
            var unexistingUserId = new Guid("9ad3edbe-17e1-4715-96ff-9e51d81766b1");

            var preTestUsersCount = Context.Users.Count();
            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomsCount = Context.RoomsUsers.Count();

            var request = new RoomsUsersGetByUserIdRequestDto
            {
                UserId = unexistingUserId
            };

            //Act
            var response = await _testService.GetAllRoomsByUser(request);

            //Assert
            response.Should().BeNull();

            Context.Users.Count().Should().Be(preTestUsersCount);
            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Theory]
        [InlineData(RoomUserRole.User)]
        [InlineData(RoomUserRole.Moderator)]
        public async Task ExitRoom_UserExitsValidRoom_RoomUserPairRemoved(RoomUserRole role)
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            var userRoomPair = new RoomUser
            {
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = role
            };
            await Context.SaveEntity(userRoomPair);

            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomsCount = Context.RoomsUsers.Count();

            var request = new RoomsUsersDeleteRequestDto
            {
                RoomId = existingRoom.Id,
                UserId = user.Id
            };

            //Act
            var response = await _testService.DeleteByUserIdAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.Exists.Should().BeTrue();
            response.isAdministrator.Should().BeFalse();

            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount - 1);
        }

        [Fact]
        public async Task ExitRoom_OwnerUserExitsValidRoom_UserStillInRoom()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;
            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            var userRoomPair = new RoomUser
            {
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.Owner
            };
            await Context.SaveEntity(userRoomPair);

            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomsCount = Context.RoomsUsers.Count();

            var request = new RoomsUsersDeleteRequestDto
            {
                RoomId = existingRoom.Id,
                UserId = user.Id
            };

            //Act
            var response = await _testService.DeleteByUserIdAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.Exists.Should().BeTrue();
            response.isAdministrator.Should().BeTrue(); 
            
            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
        }

        [Fact]
        public async Task ExitRoom_UserTryingToExitUnexistingRoom_CannotExitRoom()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            var userRoomPair = new RoomUser
            {
                RoomId = existingRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.User
            };
            await Context.SaveEntity(userRoomPair);

            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomsCount = Context.RoomsUsers.Count();
            var preTestUsersInRoomsCount = Context.RoomsUsers.Count(p => p.RoomId == existingRoom.Id);

            var notExistingRoomId = new Guid("18b25d12-9937-4f32-ac16-65de05822343");
            var request = new RoomsUsersDeleteRequestDto
            {
                RoomId = notExistingRoomId,
                UserId = user.Id
            };

            //Act
            var response = await _testService.DeleteByUserIdAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.Exists.Should().BeFalse();
            response.isAdministrator.Should().BeFalse();

            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
            Context.RoomsUsers.Count(p => p.RoomId == existingRoom.Id).Should().Be(preTestUsersInRoomsCount);
        }

        [Fact]
        public async Task ExitRoom_UserTryingToExitAnotherRoom_CannotExitRoom()
        {
            //Arrange
            var user = SharedTestData.DefaultUser;

            var usersRoom = new Room
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(usersRoom);

            var anotherRoom = new Room
            {
                Name = "AnotherTestRoom"
            };
            await Context.SaveEntity(anotherRoom);

            var userRoomPair = new RoomUser
            {
                RoomId = usersRoom.Id,
                UserId = user.Id,
                Role = RoomUserRole.User
            };
            await Context.SaveEntity(userRoomPair);

            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomsCount = Context.RoomsUsers.Count();
            var preTestUsersInRoomsCount = Context.RoomsUsers.Count(p => p.RoomId == usersRoom.Id);

            var request = new RoomsUsersDeleteRequestDto
            {
                RoomId = anotherRoom.Id,
                UserId = user.Id
            };

            //Act
            var response = await _testService.DeleteByUserIdAsync(request);

            //Assert
            response.Should().NotBeNull();
            response.Exists.Should().BeFalse();
            response.isAdministrator.Should().BeFalse();

            Context.Rooms.Count().Should().Be(preTestRoomsCount);
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomsCount);
            Context.RoomsUsers.Count(p => p.RoomId == usersRoom.Id).Should().Be(preTestUsersInRoomsCount);
        }
    }
}
