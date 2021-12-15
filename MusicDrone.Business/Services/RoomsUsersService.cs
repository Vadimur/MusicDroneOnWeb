using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data;
using MusicDrone.Data.Models;
using AutoMapper;

namespace MusicDrone.Business.Services
{
    public class RoomsUsersService:IRoomsUsersService
    {
        private readonly MusicDroneDbContext _context;
        private readonly IMapper _mapper;
        public RoomsUsersService(MusicDroneDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateAsync(RoomsUsersCreateRequestDto request) 
        {
            var roomuser = new RoomsUsers { Id = Guid.NewGuid().ToString(), RoomId = request.RoomId, UserId = request.RoomId, Role = request.Role };
            await _context.RoomsUsers.AddAsync(roomuser);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<RoomsUsersGetByRoomIdResponseDto>> GetAllInRoom(RoomsUsersGetByRoomIdRequestDto request)
        {
            var roomusers = _context.RoomsUsers.Where(r => r.RoomId == request.RoomId);
            var users = new List<RoomsUsersGetByRoomIdResponseDto>();
            foreach (var roomuser in roomusers) 
            {
                var user = _mapper.Map<RoomsUsersGetByRoomIdResponseDto>(await _context.Users.FindAsync(roomuser.UserId));
                users.Add(user);
            }
            return users;
        }
        public async Task DeleteByUserIdAsync(RoomsUsersDeleteRequestDto request) 
        {
            var roomuser = await _context.RoomsUsers.FindAsync(request.Id);
            _context.RoomsUsers.Remove(roomuser);
            await _context.SaveChangesAsync();
        }
    }
}
