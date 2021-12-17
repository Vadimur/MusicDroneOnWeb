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
using System.Linq;

namespace MusicDrone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomController(IRoomService roomService, IMapper mapper) 
        {
            _roomService = roomService;
            _mapper = mapper;
        }
        
        [HttpPost("createRoom")]
        public async Task<ActionResult<RoomCreateRequestModel>> CreateRoom([FromBody]RoomCreateRequestModel request) 
        {
            var serviceRequest = _mapper.Map<RoomCreateRequestDto>(request);
            serviceRequest.userClaims = User;
            await _roomService.CreateAsync(serviceRequest);
            var actionName = "ReturnCreatedModel";
            return CreatedAtAction(actionName, request);
        }

        [HttpGet("getRooms")]
        public async Task<ActionResult<IEnumerable<RoomResponseModel>>> GetAllRooms() 
        {
            var existingRooms = await _roomService.GetAll();
            var rooms = existingRooms.Select(r => _mapper.Map<RoomResponseModel>(r)).ToList();

            return Ok(rooms);
        }

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
            return NoContent();
        }
    }
}
