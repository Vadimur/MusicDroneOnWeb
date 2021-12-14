using System.Security.Claims;
using System.Threading.Tasks;
using MusicDrone.Data.Services.Models.Requests;
using MusicDrone.Data.Services.Models.Responses;

namespace MusicDrone.Data.Services.Abstraction
{
    public interface IAccountManagement
    {
        Task<BaseResponse<string>> Login(LoginDto credentials);
        Task<BaseResponse<string>> Register(RegisterDto request, string role);
        Task<BaseResponse<ProfileDto>> RetrieveProfile(ClaimsPrincipal user);

        //TODO: check token
        //Task<string> GetTokenAsync(string userName);
    }
}
