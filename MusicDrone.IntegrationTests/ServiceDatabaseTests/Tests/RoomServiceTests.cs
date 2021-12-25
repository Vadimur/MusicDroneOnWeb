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
   // [CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
    public class RoomServiceTests : TestsBase
    {
        private readonly RoomService _testRoomService;
        public RoomServiceTests(BaseTestsFixture fixture) : base(fixture)
        {
            _testRoomService = new RoomService(Context);
        }

        [Fact]
        public async Task Create_ValidRequest_RoomCreatedAndTiedWithOwner()
        {
            //Arrange
            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomPairsCount = Context.RoomsUsers.Count();

            var testUser = SharedTestData.DefaultUser;
            var testRoomName = "TestRoom123";

            var request = new RoomCreateRequestDto
            {
                UserId = testUser.Id,
                Name = testRoomName
            };

            //Act
            var response = await _testRoomService.CreateAsync(request);

            //Assert
            response.Name.Should().BeEquivalentTo(testRoomName);

            // room is added
            Context.Rooms.Count().Should().Be(preTestRoomsCount + 1);
            var rooms = Context.Rooms.Where(r => r.Id == response.Id && r.Name == response.Name).ToList();
            rooms.Count.Should().Be(1);

            // room tied with user in owner role
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomPairsCount + 1);
            var newRoomUserPairs = Context.RoomsUsers.Where(p => p.UserId == testUser.Id && p.RoomId == response.Id).ToList();
            newRoomUserPairs.Count.Should().Be(1);
            newRoomUserPairs.Single().Role.Should().Be(RoomUserRole.Owner);
        }

        /*[Fact] // should be fixed
        public async Task Create_UserDoesNotExist_NoChangesInDatabase()
        {
            //Arrange
            var preTestUsersCount = Context.Users.Count();
            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomPairsCount = Context.RoomsUsers.Count();

            var testUser = SharedTestData.CreateTestUser(new Guid("6b869193-972c-4f99-8224-d7fa26ab381b"), "UnexistingUsername", SharedTestData.DefaultTestPassword);
            var testRoomName = "TestRoom123";

            var request = new RoomCreateRequestDto
            {
                UserId = testUser.Id,
                Name = testRoomName
            };

            var testService = new RoomService(Context);

            //Act
            var response = await testService.CreateAsync(request);

            //Assert
            //response.Name.Should().BeEquivalentTo(testRoomName);

            // new user is not created
            Context.Users.Count().Should().Be(preTestUsersCount);

            // room-user pair shouldn't be creater
            Context.RoomsUsers.Count().Should().Be(preTestUserRoomPairsCount);
        }*/

        [Theory]
        [MemberData(nameof(SharedTestData.ExistingRooms), MemberType = typeof(SharedTestData))]
        public async Task GetAll_ValidRequest_AllRoomsReturned(List<Room> existingRooms)
        {
            //Arrange
            await Context.SaveEntityRange(existingRooms);
            var roomsCount = Context.Rooms.Count();

            //Act
            var response = await _testRoomService.GetAll();

            //Assert
            response.Count().Should().Be(roomsCount);
            response.Select(r => r.Name).Should().BeEquivalentTo(existingRooms.Select(r => r.Name));
        }

        [Fact]
        public async Task GetRoom_ExistingRoomId_RoomInformationReturned()
        {
            //Arrange
            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            var request = new RoomGetByIdRequestDto
            {
                Id = existingRoom.Id
            };

            //Act
            var response = await _testRoomService.GetById(request);

            //Assert
            response.Id.Should().Be(existingRoom.Id);
            response.Name.Should().Be(existingRoom.Name);
        }

        [Fact]
        public async Task GetRoom_UnexistingRoomId_NullReturned()
        {
            //Arrange
            var request = new RoomGetByIdRequestDto
            {
                Id = new Guid("2ec03917-c038-4edc-b4cb-310c42b09a0a")
            };

            //Act
            var response = await _testRoomService.GetById(request);

            //Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task DeleteRoom_ExistingRoom_RoomIsDeletedFromDatabase()
        {
            //Arrange
            var testUser = SharedTestData.DefaultUser;
            var existingRoom = new Room
            {
                Name = "TestRoomName"
            };
            await Context.SaveEntity(existingRoom);

            var roomUserPair = new RoomUser()
            {
                RoomId = existingRoom.Id,
                UserId = testUser.Id,
                Role = RoomUserRole.Owner
            };
            await Context.SaveEntity(roomUserPair);

            var request = new RoomDeleteRequestDto
            {
                Id = existingRoom.Id,
                UserId = testUser.Id
            };

            var preTestRoomsCount = Context.Rooms.Count();
            var preTestUserRoomPairsCount = Context.RoomsUsers.Count();

            //Act
            var response = await _testRoomService.DeleteByIdAsync(request);

            //Assert
            Context.Rooms.Count().Should().Be(preTestRoomsCount - 1);
            Context.Rooms.Count(r => r.Id == existingRoom.Id).Should().Be(0);
            Context.RoomsUsers.Count(r => r.Id == existingRoom.Id).Should().Be(0);
        }

        //existing, unexisting, only owner can delete database
    }
}
