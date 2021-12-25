using Microsoft.AspNetCore.Identity;
using MusicDrone.Data.Models;
using System;
using System.Collections.Generic;

namespace MusicDrone.IntegrationTests.Shared
{
    public class SharedTestData
    {
        public const string DefaultTestPassword = "Password@123_";

        public static ApplicationUser DefaultUser => CreateTestUser(new Guid("e97b807d-a4ff-4c3f-a343-e6cecdc4cf8d"), "DefaultUsername", DefaultTestPassword);

        public static ApplicationUser CreateTestUser(Guid guid, string username, string password)
        {
            var userId = guid;
            var hasher = new PasswordHasher<IdentityUser>();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Email = username,
                NormalizedEmail = username.ToUpper(),
                FirstName = "FirstName",
                LastName = "LastName",
                PasswordHash = hasher.HashPassword(null, password),
                SecurityStamp = userId.ToString()
            };

            return user;
        }

        public static IEnumerable<object[]> UserListsNotEmpty()
        {
            yield return new object[]
            {
                new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        Id = new Guid("adc53eb7-ff70-4699-9c68-20de87941b2e"),
                        UserName = "Username1",
                        NormalizedUserName = "Username1".ToUpper(),
                        Email = "Username1",
                        NormalizedEmail = "Username1".ToUpper(),
                        FirstName = "FirstName1",
                        LastName = "LastName1",
                        SecurityStamp = "adc53eb7-ff70-4699-9c68-20de87941b2e"
                    },
                    new ApplicationUser
                    {
                        Id = new Guid("8edb4bbf-ddc8-45ff-96ff-261b3f180479"),
                        UserName = "Username2",
                        NormalizedUserName = "Username2".ToUpper(),
                        Email = "Username2",
                        NormalizedEmail = "Username2".ToUpper(),
                        FirstName = "FirstName2",
                        LastName = "LastName2",
                        SecurityStamp = "8edb4bbf-ddc8-45ff-96ff-261b3f180479"
                    }
                }
            };

            yield return new object[]
            {   new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        Id = new Guid("be020696-ffac-442a-a140-488138241fe9"),
                        UserName = "Username3",
                        NormalizedUserName = "Username3".ToUpper(),
                        Email = "Username3",
                        NormalizedEmail = "Username3".ToUpper(),
                        FirstName = "FirstName3",
                        LastName = "LastName3",
                        SecurityStamp = "be020696-ffac-442a-a140-488138241fe9"
                    }
                }
            };
        }

        public static IEnumerable<object[]> UsersListEmpty()
        {
            yield return new object[]
            {
                new List<ApplicationUser> {  }
            };
        }

        public static IEnumerable<object[]> RoomListsNotEmpty()
        {
            yield return new object[]
            {
                new List<Room>
                {
                    new Room { Name = "TestRoom111" },
                    new Room { Name = "TestRoom222" },
                    new Room { Name = "TestRoom333" }
                }
            };

            yield return new object[]
            {
                new List<Room>
                {
                    new Room { Name = "TestRoom11" },
                    new Room { Name = "TestRoom22" }
                }
            };

            yield return new object[]
            {
                new List<Room>
                {
                    new Room { Name = "TestRoom1" }
                }
            };
        }

        public static IEnumerable<object[]> RoomsListEmpty()
        {
            yield return new object[] { new List<Room>() };
        }
    }
}
