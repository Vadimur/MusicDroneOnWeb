using Microsoft.AspNetCore.Identity;
using MusicDrone.API.Models.Requests;
using MusicDrone.Data.Models;
using MusicDrone.Data.Constants;
using System;
using System.Collections.Generic;

namespace MusicDrone.IntegrationTests.Tests.Data
{
    public class AccountTestDataGenerator
    {
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

        public static IEnumerable<object[]> UsersWithAuthorizationRoles()
        {
            yield return new object[] { new Guid("ff323ef5-acdc-4ea7-9317-47ffa8b34961"), $"{Roles.ADMINISTRATORS}_user", Roles.ADMINISTRATORS };
            yield return new object[] { new Guid("65db8c24-24a8-42b1-adde-62db410dd1f4"), $"{Roles.MODERATORS}_user", Roles.MODERATORS };
            yield return new object[] { new Guid("c230fde3-9605-466a-b012-a733ff8bb74a"), $"{Roles.PREMIUMUSERS}_user", Roles.PREMIUMUSERS };
            yield return new object[] { new Guid("593e3309-be20-4ca8-8324-963e3732296b"), $"{Roles.USERS}_user", Roles.USERS };
        }

        public static IEnumerable<object[]> RegistrationBadRequests()
        {
            yield return new object[] { new RegisterRequest { } };
            yield return new object[] { new RegisterRequest { Name = "Tribbiani" } };
            yield return new object[] { new RegisterRequest { Name = "Tribbiani", Surname = "Mancini" } };
            yield return new object[] { new RegisterRequest { Name = "Tribbiani", Surname = "Mancini", Email = "TribbianiMancini@gmail.com" } };
            yield return new object[] { new RegisterRequest { Surname = "Mancini", Email = "TribbianiMancini@gmail.com", Password = "Tribbian1Mancini_@" } };
            yield return new object[] { new RegisterRequest { Email = "TribbianiMancini@gmail.com", Password = "Tribbian1Mancini_@" } };
        }

        public static IEnumerable<object[]> LoginBadRequests()
        {
            yield return new object[] { new LoginRequest { } };
            yield return new object[] { new LoginRequest { Login = "TribbianiMancini@gmail.com" } };
            yield return new object[] { new LoginRequest { Password = "Tribbian1Mancini_@" } };
        }

        public static IEnumerable<object[]> UsersInRoom()
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

            yield return new object[]
            {
                new List<ApplicationUser> {  }
            };
        }

        public static IEnumerable<object[]> UserAndRooms()
        {
            yield return new object[]
            {
                CreateTestUser(new Guid("8a20f128-046f-4817-bb1a-2dacfb1fb25c"), "TestUsername", TestConstants.DefaultTestPassword),
                new List<Room>
                {
                    new Room { Name = "TestRoom1" },
                    new Room { Name = "TestRoom2" },
                    new Room { Name = "TestRoom3" }
                }
            };

            yield return new object[]
            {
                CreateTestUser(new Guid("e6c27b39-9cae-45de-af42-b62ae6498160"), "TestUsername", TestConstants.DefaultTestPassword),
                new List<Room>
                {
                    new Room { Name = "TestRoom1" }
                }
            };

            yield return new object[]
            {
                CreateTestUser(new Guid("fd8418aa-187b-4e38-afbc-4c90547529ab"), "TestUsername", TestConstants.DefaultTestPassword),
                new List<Room> { }
            };
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
