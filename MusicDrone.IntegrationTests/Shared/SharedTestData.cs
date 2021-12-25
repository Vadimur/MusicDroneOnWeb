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

        public static IEnumerable<object[]> ExistingRooms()
        {
            yield return new object[] { new List<Room> { } };
            yield return new object[]
            {
                new List<Room>
                {
                    new Room { Name = "TestRoom1" },
                    new Room { Name = "TestRoom2" }
                }
            };
        }
    }
}
