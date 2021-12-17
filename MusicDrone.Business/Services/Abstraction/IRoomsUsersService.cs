using System.Collections.Generic;
using System.Threading.Tasks;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;

namespace MusicDrone.Business.Services.Abstraction
{
    public interface IRoomsUsersService
    {
        Task<RoomsUsersCreateResponseDto> CreateAsync(RoomsUsersCreateRequestDto request);
        Task<IEnumerable<RoomsUsersGetByRoomIdResponseDto>> GetAllInRoom(RoomsUsersGetByRoomIdRequestDto request);
        Task<RoomsUsersDeleteByUserIdResponseDto> DeleteByUserIdAsync(RoomsUsersDeleteRequestDto request);
    }
}
