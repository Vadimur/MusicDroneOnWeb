using FluentAssertions;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Services;
using MusicDrone.IntegrationTests.ServiceDatabaseTests.Tests.Base;
using MusicDrone.IntegrationTests.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MusicDrone.IntegrationTests.ServiceDatabaseTests.Tests
{
    public class RoomUsersServiceTests : TestsBase
    {
        public RoomUsersServiceTests(BaseTestsFixture fixture) : base(fixture)
        {

        }

        [Fact]
        public async Task Can_add_item()
        {
            var testUser = SharedTestData.DefaultUser;

            var controller = new RoomsUsersService(Context);

            var request = new RoomsUsersCreateRequestDto()
            {
                RoomId = new Guid("50044808-3427-478f-99e1-69a71445c3c3"),
                UserId = testUser.Id
            };

            _ = await controller.CreateAsync(request);

            //Assert.Empty(context.Rooms);
            Context.RoomsUsers.Count().Should().Be(0);
            Context.Users.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetEmpty()
        {
            var testUser = SharedTestData.DefaultUser;

            var controller = new RoomsUsersService(Context);

            var request = new RoomsUsersCreateRequestDto()
            {
                RoomId = new Guid("50044808-3427-478f-99e1-69a71445c3c3"),
                UserId = testUser.Id
            };

            _ = await controller.CreateAsync(request);

            //Assert.Empty(context.Rooms);
            Context.RoomsUsers.Count().Should().Be(0);
            Context.Users.Count().Should().Be(2);
        }
    }
}
