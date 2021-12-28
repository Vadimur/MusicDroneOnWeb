using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.DTO;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private MusicDroneDbContext _dbContext;
        private DbSet<ApplicationUser> _applicationUsers;

        public UserRepository(MusicDroneDbContext musicDroneDbContext)
        {
            _dbContext = musicDroneDbContext;
            _applicationUsers = _dbContext.Set<ApplicationUser>();
        }
        
        public async Task<ApplicationUser> GetByIdAsync(Guid id)
        {
            return await _applicationUsers.FindAsync(id);
        }

        public async Task<IReadOnlyList<ApplicationUser>> ListAllAsync()
        {
            return await _applicationUsers.ToListAsync();
        }
        
        public async Task<IEnumerable<RoomsUsersGetByRoomIdResponseDto>> ListUsersByRoomIdAsync(Guid roomId)
        {
            var usersInRoom = await
            (
                from roomsUsers in _dbContext.Set<RoomUser>()
                join users in _dbContext.Users on roomsUsers.UserId equals users.Id
                where roomsUsers.RoomId == roomId
                select new RoomsUsersGetByRoomIdResponseDto
                {
                    FirstName = users.FirstName,
                    LastName = users.LastName
                }
            ).ToListAsync();

            return usersInRoom;
        }
    }
}