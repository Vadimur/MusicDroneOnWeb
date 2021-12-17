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
    public class RoomsUsersService : IRoomsUsersService
    {
        private readonly MusicDroneDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoomsUsersService(MusicDroneDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task CreateAsync(RoomsUsersCreateRequestDto request)
        {
            var requestuser = await _userManager.GetUserAsync(request.UserClaims);
            var roomuser = new RoomsUsers { RoomId = request.RoomId, UserId = new Guid(requestuser.Id), Role = Roles.USERS };
            await _context.RoomsUsers.AddAsync(roomuser);
            await _context.SaveChangesAsync();
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
        public async Task<RoomsUsersDeleteByUserIdResponse> DeleteByUserIdAsync(RoomsUsersDeleteRequestDto request)
        {
            var user = await _userManager.GetUserAsync(request.UserClaims);
            var userId = new Guid(user.Id);
            var roomuser = await _context.RoomsUsers.Where(r => r.RoomId == request.RoomId && r.UserId == userId).FirstOrDefaultAsync();
            if (roomuser is not null)
            {
                _context.RoomsUsers.Remove(roomuser);
                await _context.SaveChangesAsync();
                return new RoomsUsersDeleteByUserIdResponse { Exists = true };
            }
            else return new RoomsUsersDeleteByUserIdResponse { Exists = false }; 
        }
    }
}
