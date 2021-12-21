using System;
using System.Collections.Generic;
using System.Security.Claims;
using MusicDrone.Data.Constants;
using MusicDrone.Data.Models;

namespace MusicDrone.IntegrationTests.Tests.Custom.Authentication
{
    public class TestClaimsProvider
    {
        public readonly IList<Claim> Claims;

        public TestClaimsProvider(IList<Claim> claims)
        {
            Claims = claims;
        }

        public TestClaimsProvider()
        {
            Claims = new List<Claim>();
        }

        public static TestClaimsProvider FromApplicationUser(ApplicationUser user, string role)
        {
            var provider = new TestClaimsProvider();
            provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            provider.Claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            provider.Claims.Add(new Claim(ClaimTypes.Email, user.Email));
            provider.Claims.Add(new Claim(ClaimTypes.Role, role));

            return provider;
        }

        public static TestClaimsProvider WithAdminClaims()
        {
            var provider = new TestClaimsProvider();
            provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            provider.Claims.Add(new Claim(ClaimTypes.Name, "admin@music.drone"));
            provider.Claims.Add(new Claim(ClaimTypes.Email, "admin@music.drone"));
            provider.Claims.Add(new Claim(ClaimTypes.Role, Roles.ADMINISTRATORS));

            return provider;
        }

        public static TestClaimsProvider WithUserClaims()
        {
            var provider = new TestClaimsProvider();
            provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            provider.Claims.Add(new Claim(ClaimTypes.Name, "testuser@music.drone"));
            provider.Claims.Add(new Claim(ClaimTypes.Email, "testuser@music.drone"));
            provider.Claims.Add(new Claim(ClaimTypes.Role, Roles.USERS));

            return provider;
        }
    }
}
