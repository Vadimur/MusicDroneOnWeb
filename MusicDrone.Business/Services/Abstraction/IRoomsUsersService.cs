using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;

namespace MusicDrone.Business.Services.Abstraction
{
    public interface IRoomsUsersService
    {
        Task CreateAsync(RoomsUsersCreateRequestDto request);
        Task<IEnumerable<RoomsUsersGetByRoomIdResponseDto>> GetAllInRoom(RoomsUsersGetByRoomIdRequestDto request);
        Task<RoomsUsersDeleteByUserIdResponse> DeleteByUserIdAsync(RoomsUsersDeleteRequestDto request);
    }
}
