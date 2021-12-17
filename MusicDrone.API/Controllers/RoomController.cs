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
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomController(IRoomService roomService, IMapper mapper) 
        {
            _roomService = roomService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<RoomCreateRequestModel>> CreateRoom([FromBody]RoomCreateRequestModel request) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var serviceRequest = new RoomCreateRequestDto { Name = request.Name, UserId = new Guid(userId) };
            var serviceResponse = await _roomService.CreateAsync(serviceRequest);
            return CreatedAtAction("GetConcreteRoom", new { id = serviceResponse.Id }, request);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomResponseModel>>> GetAllRooms() 
        {
            var serviceResponse = await _roomService.GetAll();
            var rooms = _mapper.Map<IEnumerable<RoomResponseDto>,IEnumerable<RoomResponseModel>>(serviceResponse);
            return Ok(rooms);
        }
        [ActionName("GetConcreteRoom")]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<RoomResponseModel>> GetConcreteRoom(string id) 
        {
            var serviceRequest = new RoomGetByIdRequestDto { Id = new Guid(id) };
            var serviceResponse = await _roomService.GetById(serviceRequest);
            if (serviceResponse == null) 
            {
                return NotFound();
            }
            var room = new RoomResponseModel { Id = serviceResponse.Id.ToString(), Name = serviceResponse.Name };
            return Ok(room);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteRoom([FromBody]RoomDeleteRequestModel request) 
        {
            var serviceRequest = new RoomDeleteRequestDto { Id = new Guid(request.Id), UserId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier))};
            var serviceResponse = await _roomService.DeleteByIdAsync(serviceRequest);
            if (serviceResponse.Exists == false) 
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
