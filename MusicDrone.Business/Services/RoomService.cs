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
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoomService(MusicDroneDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateAsync(RoomCreateRequestDto request)
        {
            var user = await _userManager.GetUserAsync(request.userClaims);
            var room = new Room { Id = Guid.NewGuid().ToString(), Name = request.Name };
            var roomsusers = new RoomsUsers { Id = Guid.NewGuid().ToString(), RoomId = room.Id, UserId = user.Id, Role =  Roles.ADMINISTRATORS};
            await _context.Rooms.AddAsync(room);
            await _context.RoomsUsers.AddAsync(roomsusers);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<RoomResponseDto>> GetAll() 
        {
            IEnumerable<Room> rooms = await _context.Rooms.ToListAsync();
            return _mapper.Map<IEnumerable<RoomResponseDto>>(rooms);
        }
        public async Task<RoomResponseDto> GetById(RoomGetByIdRequestDto request) 
        {
            return _mapper.Map<RoomResponseDto>(await _context.Rooms.FindAsync(request.Id));
        }
        public async Task DeleteByIdAsync(RoomDeleteRequestDto request) 
        {
            var deleter = await _context.Users.FindAsync(request.userClaims);
            var validate = _context.RoomsUsers.Where(r => r.RoomId == request.Id && r.UserId == deleter.Id && r.Role == Roles.ADMINISTRATORS);
            if (validate.Count() > 0)
            {
                var room = await _context.Rooms.FindAsync(request.Id);
                var roomsusers = _context.RoomsUsers.Where(r => r.Id == request.Id);
                foreach (var roomuser in roomsusers)
                {
                    _context.RoomsUsers.Remove(roomuser);
                }
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            else return;
        }
    }
}
