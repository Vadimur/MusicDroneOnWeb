using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data;
using MusicDrone.Data.Constants;
using MusicDrone.Data.Models;
using AutoMapper;

namespace MusicDrone.Business.Services
{
    public class RoomService:IRoomService
    {
        private readonly MusicDroneDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoomService(MusicDroneDbContext context, UserManager<ApplicationUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<RoomResponseDto> CreateAsync(RoomCreateRequestDto request)
        {
            var user = await _userManager.GetUserAsync(request.UserClaims);
            var room = new Room { Name = request.Name };
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            await AddFirstUserToRoom(room, user);
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
            var deleter = await _userManager.GetUserAsync(request.UserClaims);
            var validate = _context.RoomsUsers.Where(r => r.RoomId == request.Id && r.UserId.ToString() == deleter.Id && r.Role == Roles.ADMINISTRATORS);
            if (validate.Count() > 0)
            {
                var room = await _context.Rooms.Where(r => r.Id == request.Id).FirstOrDefaultAsync();
                var roomsusers = await _context.RoomsUsers.Where(r => r.RoomId == request.Id).ToListAsync();
                foreach (var roomuser in roomsusers)
                {
                    _context.RoomsUsers.Remove(roomuser);
                }
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return new RoomDeleteResponseDto { Exists = true };
            }
            else return new RoomDeleteResponseDto { Exists = false };
        }
        private async Task AddFirstUserToRoom(Room requestedRoom, ApplicationUser requestedUser) 
        {
            var room = await _context.Rooms.FindAsync(requestedRoom.Id);
            var roomsusers = new RoomsUsers { RoomId = room.Id, UserId = Guid.Parse(requestedUser.Id), Role = Roles.ADMINISTRATORS };
            await _context.RoomsUsers.AddAsync(roomsusers);
            await _context.SaveChangesAsync();
        }
    }
}
