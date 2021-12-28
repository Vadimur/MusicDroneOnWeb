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
    public class RoomsUsersRepository : GenericRepository<Guid, RoomUser>, IRoomsUsersRepository
    {
        private readonly DbSet<RoomUser> _roomsUsers;
        
        public RoomsUsersRepository(MusicDroneDbContext musicDroneDbContext) : base(musicDroneDbContext)
        {
            _roomsUsers = DbContext.Set<RoomUser>();
        }

        public async Task<RoomUser> FindAsync(Guid roomId, Guid userId)
        {
            var roomUsers = await _roomsUsers
                .Where(r => r.RoomId == roomId && r.UserId == userId).FirstOrDefaultAsync();
            
            return roomUsers;
        }
        
        public async Task<bool> CheckUserOwnsRoomAsync(Guid roomId, Guid userId)
        {
            var count = await _roomsUsers
                .CountAsync(r => r.RoomId == roomId && r.UserId == userId && r.Role == RoomUserRole.Owner);
            
            return (count > 0);
        }
        
        public async Task<IEnumerable<RoomsUsersGetByUserIdResponseDto>> ListRoomsByUserIdAsync(Guid userId)
        {
            DbSet<Room> roomsDbSet = DbContext.Set<Room>();

            var userJoinedRooms = await
            (
                from roomsUsers in _roomsUsers
                join rooms in roomsDbSet on roomsUsers.RoomId equals rooms.Id
                where roomsUsers.UserId == userId
                select new RoomsUsersGetByUserIdResponseDto
                {
                    RoomId = rooms.Id,
                    RoomName = rooms.Name
                }
            ).ToListAsync();

            return userJoinedRooms;
        }
    }
}