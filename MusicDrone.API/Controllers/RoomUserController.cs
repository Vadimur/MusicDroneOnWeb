using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using AutoMapper;

namespace MusicDrone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomUserController : ControllerBase
    {
        private readonly IRoomsUsersService _roomsUsersService;
        private readonly IMapper _mapper;
        public RoomUserController(IRoomsUsersService roomsUsersService, IMapper mapper)
        {
            _roomsUsersService = roomsUsersService;
            _mapper = mapper;
        }
        [HttpPost("enter")]
        public async Task<ActionResult> EnterRoom([FromBody]RoomsUsersCreateRequestModel request)
        {
            bool isUserValid = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userGuid);
            bool isRoomValid = Guid.TryParse(request.RoomId, out Guid roomGuid);
            if (!isUserValid || !isRoomValid)
            {
                return NotFound();
            }
            var serviceRequest = new RoomsUsersCreateRequestDto { RoomId = roomGuid, UserId = userGuid };
            var serviceResponse = await _roomsUsersService.CreateAsync(serviceRequest);
            if (serviceResponse == null) 
            {
                return NotFound();
            }
            if (serviceResponse.Exists)
            {
                return Conflict();
            }
            return Ok();
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<RoomsUsersGetByRoomIdResponseModel>>> GetAllUsersInRoom(string id)
        {
            bool isRoomValid = Guid.TryParse(id, out Guid roomGuid);
            if (!isRoomValid)
            {
                return NotFound();
            }
            var serviceRequest = new RoomsUsersGetByRoomIdRequestDto { RoomId = roomGuid };
            var serviceResponse = await _roomsUsersService.GetAllInRoom(serviceRequest);
            if (serviceResponse == null)
            {
                return NotFound();
            }
            var users = _mapper.Map<IEnumerable<RoomsUsersGetByRoomIdResponseDto>, IEnumerable<RoomsUsersGetByRoomIdResponseModel>>(serviceResponse);
            return Ok(users);
        }
        [HttpGet("rooms")]
        public async Task<ActionResult<IEnumerable<RoomsUsersGetByUserIdResponseModel>>> GetAllRoomsByUser()
        {
            bool isValid = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userGuid);
            if (!isValid)
            {
                return NotFound();
            }
            var serviceRequest = new RoomsUsersGetByUserIdRequestDto { UserId = userGuid };
            var serviceResponse = await _roomsUsersService.GetAllRoomsByUser(serviceRequest);
            if (serviceResponse == null)
            {
                return NotFound();
            }
            var rooms = _mapper.Map<IEnumerable<RoomsUsersGetByUserIdResponseDto>, IEnumerable<RoomsUsersGetByUserIdResponseModel>>(serviceResponse);
            return Ok(rooms);
        }
        [HttpDelete("exit")]
        public async Task<ActionResult> ExitTheRoom([FromBody] RoomsUsersDeleteRequestModel request) 
        {
            bool isUserValid = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userGuid);
            bool isRoomValid = Guid.TryParse(request.RoomId, out Guid roomGuid);
            if (!isUserValid || !isRoomValid) 
            {
                return NotFound();
            }
            var serviceRequest = new RoomsUsersDeleteRequestDto { RoomId = roomGuid, UserId = userGuid };
            var serviceResponse = await _roomsUsersService.DeleteByUserIdAsync(serviceRequest);
            if (serviceResponse.Exists == false)
            {
                return NotFound();
            }
            if (serviceResponse.isAdministrator == true) 
            {
                return Forbid();
            }
            return NoContent();
        }
    }
}
