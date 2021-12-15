using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Data.Constants;
using AutoMapper;

namespace MusicDrone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        public RoomController(IRoomService roomService) 
        {
            _roomService = roomService;
        }
        [Authorize]
        [HttpPost("createRoom")]
        public async Task<ActionResult<RoomCreateRequestModel>> CreateRoom([FromBody]RoomCreateRequestModel request) 
        {
            var serviceRequest = _mapper.Map<RoomCreateRequestDto>(request);
            serviceRequest.userClaims = User;
            await _roomService.CreateAsync(serviceRequest);
            var actionName = "ReturnCreatedModel";
            return CreatedAtAction(actionName, request);
        }
        [Authorize]
        [HttpGet("getRooms")]
        public async Task<ActionResult<IEnumerable<RoomResponseModel>>> GetAllRooms() 
        {
            var rooms = _mapper.Map<RoomResponseModel>(await _roomService.GetAll());
            return Ok(rooms);
        }
        [Authorize]
        [HttpGet("getRoom")]
        public async Task<ActionResult<RoomResponseModel>> GetConcreteRoom([FromBody]RoomGetByIdRequestModel request) 
        {
            var serviceRequest = _mapper.Map<RoomGetByIdRequestDto>(request);
            var room = _mapper.Map<RoomResponseModel>(await _roomService.GetById(serviceRequest));
            return Ok(room);
        }
        [Authorize(Roles = Roles.ADMINISTRATORS)]
        [HttpDelete("deleteRoom")]
        public async Task<ActionResult> DeleteRoom([FromBody]RoomDeleteRequestModel request) 
        {
            var serviceRequest = _mapper.Map<RoomDeleteRequestDto>(request);
            serviceRequest.userClaims = User;
            await _roomService.DeleteByIdAsync(serviceRequest);
            return new NoContentResult();
        }
    }
}
