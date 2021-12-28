using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicDrone.Data.DTO;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Abstractions
{
    public interface IUserRepository : IRepository
    {
        Task<ApplicationUser> GetByIdAsync(Guid id);
        Task<IReadOnlyList<ApplicationUser>> ListAllAsync();
        Task<IEnumerable<RoomsUsersGetByRoomIdResponseDto>> ListUsersByRoomIdAsync(Guid roomId);
    }
}