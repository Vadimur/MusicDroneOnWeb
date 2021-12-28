using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.DTO;
using MusicDrone.Data.Models;

namespace MusicDrone.Business.Services
{
    public class RoomsUsersService : IRoomsUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomsUsersRepository _roomsUsersRepository;
        private readonly IUserRepository _usersRepository;

        public RoomsUsersService(IUnitOfWork uow)
        {
            _uow = uow;
            _roomRepository = uow.GetRepository<IRoomRepository>();
            _roomsUsersRepository = uow.GetRepository<IRoomsUsersRepository>();
            _usersRepository = uow.GetRepository<IUserRepository>();
        }
        
        public async Task<RoomsUsersCreateResponseDto> CreateAsync(RoomsUsersCreateRequestDto request)
        {
            var response = new RoomsUsersCreateResponseDto { Exists = false };
            var room = await _roomRepository.GetByIdAsync(request.RoomId);
            
            if (room == null)
                return null;
            
            var roomUser = await _roomsUsersRepository.FindAsync(request.RoomId, request.UserId);
            if (roomUser == null)
            {
                roomUser = new RoomUser { RoomId = request.RoomId, UserId = request.UserId, Role = RoomUserRole.User };
                await _roomsUsersRepository.AddAsync(roomUser);
                await _uow.SaveChanges();
            }
            else
                response.Exists = true;
            
            return response;
        }
        
        public async Task<ICollection<RoomsUsersGetByRoomIdResponseDto>> GetAllInRoom(RoomsUsersGetByRoomIdRequestDto request)
        {
            var roomExists = await _roomRepository.GetByIdAsync(request.RoomId);

            var roomUsersResponseList = new List<RoomsUsersGetByRoomIdResponseDto>();
            
            if (roomExists != null)
                roomUsersResponseList.AddRange(await _usersRepository.ListUsersByRoomIdAsync(request.RoomId));
            
            return roomUsersResponseList;
        }
        
        public async Task<IEnumerable<RoomsUsersGetByUserIdResponseDto>> GetAllRoomsByUser(RoomsUsersGetByUserIdRequestDto request)
        {
            var userExists = await _usersRepository.GetByIdAsync(request.UserId);
            var roomUsersResponseList = new List<RoomsUsersGetByUserIdResponseDto>();
            
            if (userExists != null)
                roomUsersResponseList.AddRange(await _roomsUsersRepository.ListRoomsByUserIdAsync(request.UserId));

            return roomUsersResponseList;
        }
        
        public async Task<RoomsUsersDeleteByUserIdResponseDto> DeleteByUserIdAsync(RoomsUsersDeleteRequestDto request)
        {
            var response = new RoomsUsersDeleteByUserIdResponseDto { Exists = false };
            var roomUser = await _roomsUsersRepository.FindAsync(request.RoomId, request.UserId);
            
            if (roomUser != null)
            {
                response.Exists = true;
                
                if (roomUser.Role == RoomUserRole.Owner)
                    response.isAdministrator = true;
                else
                {
                    _roomsUsersRepository.Delete(roomUser);
                    await _uow.SaveChanges();
                }
            }
            
            return response;
        }
    }
}
