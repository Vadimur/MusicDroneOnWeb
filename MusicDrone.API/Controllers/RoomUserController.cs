using System;
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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomUserController : ControllerBase
    {
        private readonly IRoomsUsersService _roomsUsersService;
        public RoomUserController(IRoomsUsersService roomsUsersService)
        {
            _roomsUsersService = roomsUsersService;
        }
        [HttpPost]
        public async Task<ActionResult> EnterRoom([FromBody]RoomsUsersCreateRequestModel request)
        {
            var serviceRequest = new RoomsUsersCreateRequestDto { RoomId = new Guid(request.RoomId), UserClaims = User };
            await _roomsUsersService.CreateAsync(serviceRequest);
            return Ok();
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<RoomsUsersGetByRoomIdResponseModel>>> GetAllUsersInRoom(string id) 
        {
            var serviceRequest = new RoomsUsersGetByRoomIdRequestDto { RoomId = new Guid(id) };
            var users = await _roomsUsersService.GetAllInRoom(serviceRequest);
            return Ok(users);
        }
        [HttpDelete]
        public async Task<ActionResult> ExitTheRoom([FromBody] RoomsUsersDeleteRequestModel request) 
        {
            var serviceRequest = new RoomsUsersDeleteRequestDto { RoomId = new Guid(request.RoomId), UserClaims = User };
            var serviceResponse = await _roomsUsersService.DeleteByUserIdAsync(serviceRequest);
            if (serviceResponse.Exists == false)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
