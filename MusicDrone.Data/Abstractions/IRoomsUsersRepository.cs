using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicDrone.Data.DTO;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Abstractions
{
    public interface IRoomsUsersRepository : IAsyncRepository<Guid, RoomUser>
    {
        Task<bool> CheckUserOwnsRoomAsync(Guid roomId, Guid userId);
        Task<RoomUser> FindAsync(Guid roomId, Guid userId);
        Task<IEnumerable<RoomsUsersGetByUserIdResponseDto>> ListRoomsByUserIdAsync(Guid userId);
    }
}