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
            yield return new object[] { new Guid("ff323ef5-acdc-4ea7-9317-47ffa8b34961"), $"{Roles.ADMINISTRATORS}user", Roles.ADMINISTRATORS };
            yield return new object[] { new Guid("65db8c24-24a8-42b1-adde-62db410dd1f4"), $"{Roles.MODERATORS}user", Roles.MODERATORS };
            yield return new object[] { new Guid("c230fde3-9605-466a-b012-a733ff8bb74a"), $"{Roles.PREMIUMUSERS}user", Roles.PREMIUMUSERS };
            yield return new object[] { new Guid("593e3309-be20-4ca8-8324-963e3732296b"), $"{Roles.USERS}user", Roles.USERS };
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
    }
}
