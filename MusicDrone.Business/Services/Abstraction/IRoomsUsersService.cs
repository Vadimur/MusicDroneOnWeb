using System.Collections.Generic;
using System.Threading.Tasks;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data.DTO;

namespace MusicDrone.Business.Services.Abstraction
{
    public interface IRoomsUsersService
    {
        Task<RoomsUsersCreateResponseDto> CreateAsync(RoomsUsersCreateRequestDto request);
        Task<ICollection<RoomsUsersGetByRoomIdResponseDto>> GetAllInRoom(RoomsUsersGetByRoomIdRequestDto request);
        Task<IEnumerable<RoomsUsersGetByUserIdResponseDto>> GetAllRoomsByUser(RoomsUsersGetByUserIdRequestDto request);
        Task<RoomsUsersDeleteByUserIdResponseDto> DeleteByUserIdAsync(RoomsUsersDeleteRequestDto request);
    }
}
