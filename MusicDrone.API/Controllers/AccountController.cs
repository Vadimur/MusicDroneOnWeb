using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Data.Constants;
using MusicDrone.Data.Services.Abstraction;
using MusicDrone.Data.Services.Models.Requests;

namespace MusicDrone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManagement _accountManagementService;
        private readonly IMapper _mapper;

        public AccountController(IAccountManagement accountManagementService, IMapper mapper)
        {
            _accountManagementService = accountManagementService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest credentials)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input");

            var mappedCredentials = _mapper.Map<LoginDto>(credentials);
            var result = await _accountManagementService.Login(mappedCredentials);

            if (!result.Success)
            {
                return Unauthorized(result.Message);
            }

            var response = new LoginResponse
            {
                Token = result.Data
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileResponse>> Profile()
        {
            var profileOperationDetails = await _accountManagementService.RetrieveProfile(User);
            if (!profileOperationDetails.Success)
            {
                return NotFound();
            }

            var profile = _mapper.Map<ProfileResponse>(profileOperationDetails.Data);
            return Ok(profile);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> RegisterUser(RegisterRequest request)
        {
            return await Register(request, Roles.USERS);
        }

        [Authorize(Roles = Roles.ADMINISTRATORS)]
        [HttpPost("registerModerator")]
        public async Task<ActionResult<RegisterResponse>> RegisterModerator(RegisterRequest request)
        {
            return await Register(request, Roles.MODERATORS);
        }

        private async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request, string role)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input");

            var mappedRegistrationDetails = _mapper.Map<RegisterDto>(request);
            var registerResult = await _accountManagementService.Register(mappedRegistrationDetails, role);

            if (!registerResult.Success)
            {
                return UnprocessableEntity(registerResult.Message);
            }

            var response = new RegisterResponse
            {
                Role = registerResult.Data
            };

            return Ok(response);
        }
    }
}
