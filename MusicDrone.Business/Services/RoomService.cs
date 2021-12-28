using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Data;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.Models;

namespace MusicDrone.Business.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _uow;
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomsUsersRepository _roomsUsersRepository;
        
        public RoomService(IUnitOfWork uow) 
        {
            _uow = uow;
            _roomRepository = uow.GetRepository<IRoomRepository>();
            _roomsUsersRepository = uow.GetRepository<IRoomsUsersRepository>();
        }
        
        public async Task<RoomResponseDto> CreateAsync(RoomCreateRequestDto request)
        {
            var room = new Room { Name = request.Name };
            await _roomRepository.AddAsync(room);
            
            var roomUser = new RoomUser { RoomId = room.Id, UserId = request.UserId, Role = RoomUserRole.Owner };
            await _roomsUsersRepository.AddAsync(roomUser);

            await _uow.SaveChanges();
            var response = new RoomResponseDto { Id = room.Id, Name = room.Name };
            return response;
        }
        
        public async Task<IEnumerable<RoomResponseDto>> GetAll() 
        {
            var rooms = await _roomRepository.ListAllAsync();

            var roomsResponses = rooms
                .Select(room => new RoomResponseDto
                {
                    Id = room.Id, 
                    Name = room.Name
                }).ToList();

            return roomsResponses;
        }
        
        public async Task<RoomResponseDto> GetById(RoomGetByIdRequestDto request)
        {
            var room = await _roomRepository.GetByIdAsync(request.Id);
            var roomResponse = new RoomResponseDto
            {
                Id = room.Id,
                Name = room.Name
            };
            return roomResponse;
        }
        
        public async Task<RoomDeleteResponseDto> DeleteByIdAsync(RoomDeleteRequestDto request)
        {
            var response = new RoomDeleteResponseDto { Exists = false };
            bool userIsOwner = await _roomsUsersRepository.CheckUserOwnsRoomAsync(request.Id, request.UserId);
            
            if (userIsOwner)
            {
                response.Exists = true;
                var room = await _roomRepository.GetByIdAsync(request.Id);
                _roomRepository.Delete(room);
                await _uow.SaveChanges();
            }
            
            return response;
        }
    }
}
