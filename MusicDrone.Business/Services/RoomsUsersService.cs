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
        public RoomsUsersService(MusicDroneDbContext context)
        {
            _context = context;
        }
        public async Task<RoomsUsersCreateResponseDto> CreateAsync(RoomsUsersCreateRequestDto request)
        {
            var validate = await _context.RoomsUsers.Where(r => r.RoomId == request.RoomId && r.UserId == request.UserId).FirstOrDefaultAsync();
            if (validate is null)
            {
                var roomuser = new RoomsUsers { RoomId = request.RoomId, UserId = request.UserId, Role = Roles.USERS };
                await _context.RoomsUsers.AddAsync(roomuser);
                await _context.SaveChangesAsync();
                return new RoomsUsersCreateResponseDto { Exists = false };
            }
            else return new RoomsUsersCreateResponseDto { Exists = true };
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
