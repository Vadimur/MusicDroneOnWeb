﻿using System.Collections.Generic;
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
                var roomUser = new RoomUser { RoomId = request.RoomId, UserId = request.UserId, Role = RoomUserRole.User };
                await _context.RoomsUsers.AddAsync(roomUser);
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
            var usersInRoom = await
                (
                from roomsUsers in _context.RoomsUsers
                join users in _context.Users on roomsUsers.UserId equals users.Id
                where roomsUsers.RoomId == request.RoomId
                select new RoomsUsersGetByRoomIdResponseDto
                {
                    FirstName = users.FirstName,
                    LastName = users.LastName
                }
                ).ToListAsync();
            if (usersInRoom.Count == 0) { return null; }
            return usersInRoom;
        }
        public async Task<IEnumerable<RoomsUsersGetByUserIdResponseDto>> GetAllRoomsByUser(RoomsUsersGetByUserIdRequestDto request) 
        {
            var roomsOfUser = await
                (
                from roomsUsers in _context.RoomsUsers
                join rooms in _context.Rooms on roomsUsers.RoomId equals rooms.Id
                where roomsUsers.UserId == request.UserId
                select new RoomsUsersGetByUserIdResponseDto
                {
                    RoomId = rooms.Id,
                    RoomName = rooms.Name
                }
                ).ToListAsync();
            if (roomsOfUser.Count == 0) { return null; }
            return roomsOfUser;
        }
        public async Task<RoomsUsersDeleteByUserIdResponseDto> DeleteByUserIdAsync(RoomsUsersDeleteRequestDto request)
        {
            var roomuser = await _context.RoomsUsers.Where(r => r.RoomId == request.RoomId && r.UserId == request.UserId).FirstOrDefaultAsync();
            var response = new RoomsUsersDeleteByUserIdResponseDto { Exists = false };
            if (roomuser != null)
            {
                response.Exists = true;
                if (roomuser.Role == RoomUserRole.Owner)
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
