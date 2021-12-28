using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicDrone.API.Models.Requests;
using MusicDrone.API.Models.Responses;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Data.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MusicDrone.API.Extensions;

namespace MusicDrone.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MusicController : ControllerBase
    {
        private readonly IMusicService _musicService;

        public MusicController(IMusicService musicService, IMapper mapper)
        {
            _musicService = musicService;
        }
        
        [HttpPost]
        public async Task<ActionResult> UploadMusic(MusicAddRequestModel request) 
        {
            if (Guid.TryParse(request.RoomId, out Guid roomId) 
                && Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
            {
                if (request.UploadedFile.Length > 0)
                {
                    byte[] file = request.UploadedFile.ToByteArray();
                
                    var requestDto = new MusicAddRequestDto()
                    {
                        File = file,
                        FileExtension = ".mp3", //Temporary solution
                        MusicName = request.Name,
                        RoomId = roomId,
                        UserId = userId
                    };
                
                    await _musicService.AddMusic(requestDto);
                    return Ok();
                }
            }
            
            return BadRequest();
        }
        
        [HttpGet]
        public async Task<ActionResult> GetConcreteMusic(MusicGetRequestModel request)
        {
            if (Guid.TryParse(request.MusicId, out Guid musicId)
                && Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
            {
                var musicRequestDto = new MusicGetByIdRequestDto()
                {
                    MusicId = musicId,
                    UserId = userId
                };
                Stream stream = await _musicService.GetMusicStream(musicRequestDto);
                if (stream != Stream.Null)
                {
                    string file_type = "audio/mpeg"; //Temporary solution
                    FileStreamResult fileStreamResult = new FileStreamResult(stream, file_type)
                    {
                        EnableRangeProcessing = true
                    };
                    return fileStreamResult;
                }
                
                return new NotFoundResult();
            }

            return new BadRequestResult();
        }
        
        [HttpDelete("deleteMusic")]
        public async Task<ActionResult> DeleteMusic([FromBody]MusicDeleteRequestModel request)
        {
            var serviceRequest = new MusicDeleteRequestDto
            {
                Id = new Guid(request.Id),
                UserId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier))
            };

            return NoContent();
        }
    }
}