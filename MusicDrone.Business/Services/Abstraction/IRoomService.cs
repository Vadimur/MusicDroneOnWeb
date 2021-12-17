using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;

namespace MusicDrone.Business.Services.Abstraction
{
    public interface IRoomService
    {
        Task<RoomResponseDto> CreateAsync (RoomCreateRequestDto request);
        Task<IEnumerable<RoomResponseDto>> GetAll();
        Task<RoomResponseDto> GetById(RoomGetByIdRequestDto request);
        Task<RoomDeleteResponseDto> DeleteByIdAsync(RoomDeleteRequestDto request);
    }
}
