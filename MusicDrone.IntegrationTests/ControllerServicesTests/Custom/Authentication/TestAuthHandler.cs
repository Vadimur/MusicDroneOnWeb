using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicDrone.IntegrationTests.ControllerServicesTests.Data;

namespace MusicDrone.IntegrationTests.ControllerServicesTests.Custom.Authentication
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IList<Claim> _claims;

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, TestClaimsProvider claimsProvider) : base(options, logger, encoder, clock)
        {
            _claims = claimsProvider.Claims;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(_claims, ControllerTestsConstants.AuthenticationType);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, ControllerTestsConstants.AuthenticationType);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
