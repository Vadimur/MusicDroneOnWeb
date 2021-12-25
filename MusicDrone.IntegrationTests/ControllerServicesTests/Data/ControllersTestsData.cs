using MusicDrone.API.Models.Requests;
using MusicDrone.Data.Constants;
using System;
using System.Collections.Generic;

namespace MusicDrone.IntegrationTests.ControllerServicesTests.Data
{
    public class ControllersTestsData
    {
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
    }
}
