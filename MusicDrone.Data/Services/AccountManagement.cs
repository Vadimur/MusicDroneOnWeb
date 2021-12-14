using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MusicDrone.Data.Constants;
using MusicDrone.Data.Models;
using MusicDrone.Data.Services.Abstraction;
using MusicDrone.Data.Services.Models.Requests;
using MusicDrone.Data.Services.Models.Responses;

namespace MusicDrone.Data.Services
{
    public class AccountManagement : IAccountManagement
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountManagement(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<BaseResponse<string>> Login(LoginDto credentials)
        {
            var result = await _signInManager.PasswordSignInAsync(credentials.Login, credentials.Password, false, false);

            var token = result.Succeeded ? await GetTokenAsync(credentials.Login) : string.Empty;

            return BaseResponse<string>.Ok(token);
        }

        public async Task<BaseResponse<string>> Register(RegisterDto request, string role)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.Name,
                LastName = request.Surname
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BaseResponse<string>.Fail(result.Errors?.FirstOrDefault()?.Description);
            }

            await _userManager.AddToRoleAsync(user, role);

            return BaseResponse<string>.Ok(user.UserName);
        }

        public async Task<BaseResponse<ProfileDto>> RetrieveProfile(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            var profile = new ProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return BaseResponse<ProfileDto>.Ok(profile);
        }

        private async Task<string> GetTokenAsync(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);
            var user = await _userManager.FindByNameAsync(userName);
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim> {new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, userName) };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
