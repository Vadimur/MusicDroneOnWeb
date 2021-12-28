using System;
using System.IO;
using System.Threading.Tasks;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;

namespace MusicDrone.Business.Services.Abstraction
{
    public interface IMusicService
    {
        Task<Stream> GetMusicStream(MusicGetByIdRequestDto request);
        Task AddMusic(MusicAddRequestDto request);
        Task<MusicDeleteResponseDto> RemoveMusic(MusicDeleteRequestDto requestDto);
    }
}