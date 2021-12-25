using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data;
using MusicDrone.Data.Models;

namespace MusicDrone.Business.Services
{
    public class RoomService:IRoomService
    {
        private readonly MusicDroneDbContext _context;
        public RoomService(MusicDroneDbContext context) 
        {
            _context = context;
        }
        public async Task<RoomResponseDto> CreateAsync(RoomCreateRequestDto request)
        {
            var room = new Room { Name = request.Name };
            await _context.Rooms.AddAsync(room);
            
            var roomUser = new RoomUser { RoomId = room.Id, UserId = request.UserId, Role = RoomUserRole.Owner };
            await _context.RoomsUsers.AddAsync(roomUser);
            
            await _context.SaveChangesAsync();
            var response = new RoomResponseDto { Id = room.Id, Name = room.Name };
            return response;
        }
        
        public async Task<IEnumerable<RoomResponseDto>> GetAll() 
        {
            var rooms =
                (
                from r in _context.Rooms
                select new RoomResponseDto
                {
                    Id = r.Id,
                    Name = r.Name
                }
                ).ToListAsync();
            return await rooms;
        }
        public async Task<RoomResponseDto> GetById(RoomGetByIdRequestDto request) 
        {
            var room =
                (
                from r in _context.Rooms
                where r.Id == request.Id
                select new RoomResponseDto
                {
                    Id = r.Id,
                    Name = r.Name
                }
                ).FirstOrDefaultAsync();
            return await room;
        }
        public async Task<RoomDeleteResponseDto> DeleteByIdAsync(RoomDeleteRequestDto request) 
        {
            var validate = _context.RoomsUsers.Count(r => r.RoomId == request.Id && r.UserId == request.UserId && r.Role == RoomUserRole.Owner);
            var response = new RoomDeleteResponseDto { Exists = false };
            if (validate > 0)
            {
                response.Exists = true;
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.Id);
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return response;
            }
            else return response;
        }
    }
}
