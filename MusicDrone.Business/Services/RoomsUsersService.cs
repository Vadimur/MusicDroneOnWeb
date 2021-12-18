using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data;
using MusicDrone.Data.Constants;
using MusicDrone.Data.Models;

namespace MusicDrone.Business.Services
{
    public class RoomsUsersService : IRoomsUsersService
    {
        private readonly MusicDroneDbContext _context;
        public RoomsUsersService(MusicDroneDbContext context)
        {
            _context = context;
        }
        public async Task<RoomsUsersCreateResponseDto> CreateAsync(RoomsUsersCreateRequestDto request)
        {
            var response = new RoomsUsersCreateResponseDto { Exists = false };
            var validate = await _context.RoomsUsers.Where(r => r.RoomId == request.RoomId && r.UserId == request.UserId).FirstOrDefaultAsync();
            if (validate == null)
            {
                var roomuser = new RoomsUsers { RoomId = request.RoomId, UserId = request.UserId, Role = Roles.USERS };
                await _context.RoomsUsers.AddAsync(roomuser);
                await _context.SaveChangesAsync();
                return response;
            }
            else 
            {
                response.Exists = true;
                return response; 
            }
        }
        public async Task<IEnumerable<RoomsUsersGetByRoomIdResponseDto>> GetAllInRoom(RoomsUsersGetByRoomIdRequestDto request)
        {
            var roomusers = await _context.RoomsUsers.Where(r => r.RoomId == request.RoomId).ToListAsync();
            var users = new List<RoomsUsersGetByRoomIdResponseDto>();
            foreach (var roomuser in roomusers)
            {
                var user = await _context.Users.Where(u => u.Id == roomuser.UserId.ToString()).FirstOrDefaultAsync();
                var responseUser = new RoomsUsersGetByRoomIdResponseDto { FirstName = user.FirstName, LastName = user.LastName };
                users.Add(responseUser);
            }
            return users;
        }
        public async Task<IEnumerable<RoomsUsersGetByUserIdResponseDto>> GetAllRoomsByUser(RoomsUsersGetByUserIdRequestDto request) 
        {
            var roomusers = await _context.RoomsUsers.Where(r => r.UserId == request.UserId).ToListAsync();
            var rooms = new List<RoomsUsersGetByUserIdResponseDto>();
            foreach (var roomuser in roomusers)
            {
                var room = await _context.Rooms.Where(u => u.Id == roomuser.RoomId).FirstOrDefaultAsync();
                var responseRoom = new RoomsUsersGetByUserIdResponseDto { RoomId = room.Id, RoomName = room.Name };
                rooms.Add(responseRoom);
            }
            return rooms;
        }
        public async Task<RoomsUsersDeleteByUserIdResponseDto> DeleteByUserIdAsync(RoomsUsersDeleteRequestDto request)
        {
            var roomuser = await _context.RoomsUsers.Where(r => r.RoomId == request.RoomId && r.UserId == request.UserId).FirstOrDefaultAsync();
            var response = new RoomsUsersDeleteByUserIdResponseDto { Exists = false };
            if (roomuser != null)
            {
                response.Exists = true;
                if (roomuser.Role == Roles.ADMINISTRATORS)
                {
                    response.isAdministrator = true;
                    return response;
                }
                _context.RoomsUsers.Remove(roomuser);
                await _context.SaveChangesAsync();
                return response;
            }
            else 
            {
                return response;
            }
        }
    }
}
