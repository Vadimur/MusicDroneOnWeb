using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using AutoMapper;

namespace MusicDrone.API.Controllers
{
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
        [Authorize]
        [HttpPost("enterRoom")]
        public async Task<ActionResult> EnterRoom([FromBody]RoomsUsersCreateRequestModel request)
        {
            var serviceRequest = _mapper.Map<RoomsUsersCreateRequestDto>(request);
            await _roomsUsersService.CreateAsync(serviceRequest);
            var actionName = "ReturnCreatedModel";
            return CreatedAtAction(actionName, request);
        }
        [Authorize]
        [HttpGet("usersInRoom")]
        public async Task<ActionResult<IEnumerable<RoomsUsersGetByRoomIdResponseModel>>> GetAllUsersInRoom([FromBody] RoomsUsersGetByRoomIdRequestModel request) 
        {
            var serviceRequest = _mapper.Map<RoomsUsersGetByRoomIdRequestDto>(request);
            var users = await _roomsUsersService.GetAllInRoom(serviceRequest);
            return Ok(users);
        }
        [Authorize]
        [HttpDelete("exitTheRoom")]
        public async Task<ActionResult> ExitTheRoom([FromBody] RoomsUsersDeleteRequestModel request) 
        {
            var serviceRequest = _mapper.Map<RoomsUsersDeleteRequestDto>(request);
            await _roomsUsersService.DeleteByUserIdAsync(serviceRequest);
            return new NoContentResult();
        }
    }
}
