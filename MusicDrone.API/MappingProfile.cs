using AutoMapper;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data.Models;
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
            CreateMap<Room, RoomResponseDto>().ForMember(res => res.Id, req => req.MapFrom(r => r.Id)).ForMember(res => res.Name, req => req.MapFrom(r => r.Name));
            CreateMap<ApplicationUser, RoomsUsersGetByRoomIdResponseDto>();
            CreateMap<RoomCreateRequestModel, RoomCreateRequestDto>();
            CreateMap<RoomResponseDto, RoomResponseModel>();
            CreateMap<RoomCreateRequestModel, RoomCreateRequestDto>();
            CreateMap<RoomDeleteRequestModel, RoomDeleteRequestDto>();
            CreateMap<RoomsUsersCreateRequestModel, RoomsUsersCreateRequestDto>();
            CreateMap<RoomsUsersGetByRoomIdRequestModel, RoomsUsersGetByRoomIdRequestDto>();
            CreateMap<RoomsUsersGetByRoomIdResponseDto, RoomsUsersGetByRoomIdResponseModel>();
            CreateMap<RoomsUsersDeleteRequestModel, RoomsUsersDeleteRequestDto>();
        }
    }
}
