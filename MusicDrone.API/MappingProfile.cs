using AutoMapper;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data.Services.Models.Requests;
using MusicDrone.Data.Services.Models.Responses;

namespace MusicDrone.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginRequest, LoginDto>();
            CreateMap<RegisterRequest, RegisterDto>();
            CreateMap<ProfileDto, ProfileResponse>();
            CreateMap<RoomResponseDto, RoomResponseModel>();
        }
    }
}
