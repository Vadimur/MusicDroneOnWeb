using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MusicDrone.Business.Models.Requests;
using MusicDrone.Business.Models.Responses;
using MusicDrone.Business.Services.Abstraction;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.Models;
using MusicDrone.Data.Services.Abstraction;

namespace MusicDrone.Business.Services
{
    public class MusicService : IMusicService
    {
        private readonly IUnitOfWork _uow;
        private readonly IRoomsUsersRepository _roomsUsersRepository;
        private readonly IMusicRepository _musicRepository;
        private readonly IFileDescRepository _fileDescRepository;
        private readonly IFileService _fileService;
        
        private readonly string _filesFolder = "Music";
        
        public MusicService(IUnitOfWork uow, IFileService fileService)
        {
            _uow = uow;
            _musicRepository = uow.GetRepository<IMusicRepository>();
            _fileDescRepository = uow.GetRepository<IFileDescRepository>();
            _roomsUsersRepository = uow.GetRepository<IRoomsUsersRepository>();
            _fileService = fileService;
        }
        
        public async Task<Stream> GetMusicStream(MusicGetByIdRequestDto request)
        {
            Stream stream = Stream.Null;
            
            Music music = await _musicRepository.GetByIdAsync(request.MusicId);
            if (music != null)
            {
                RoomUser roomUsers = await _roomsUsersRepository.FindAsync(music.RoomId, request.UserId);
                if (roomUsers != null && 
                    (music.PassedModeration || roomUsers.Role == RoomUserRole.Moderator || roomUsers.Role == RoomUserRole.Owner))
                {
                    FileDesc fileDescription = await _fileDescRepository.GetByIdAsync(music.FileInfoId);
            
                    if (fileDescription != null && !fileDescription.IsRemoved)
                        stream = _fileService.GetFileStream(fileDescription.FilePath);
                }
            }

            return stream;
        }

        public async Task AddMusic(MusicAddRequestDto request)
        {
            string path = Path.Combine(_filesFolder, GenerateFileName(request.File), request.FileExtension);
            await _fileService.SaveFile(request.File, path);
            
            FileDesc fileDescription = new FileDesc()
            {
                FilePath = path,
                IsRemoved = false
            };

            await _fileDescRepository.AddAsync(fileDescription);
            
            Music music = new Music()
            {
                Name = request.MusicName,
                PassedModeration = false,
                AverageRating = 0,
                OwnerId = request.UserId,
                RoomId = request.RoomId,
                FileInfoId = fileDescription.Id
            };
            
            await _musicRepository.AddAsync(music);
            await _uow.SaveChanges();
        }

        public async Task<MusicDeleteResponseDto> RemoveMusic(MusicDeleteRequestDto requestDto)
        {
            MusicDeleteResponseDto musicDeleteResponse = new MusicDeleteResponseDto()
            {
                Exists = false
            };
            
            return musicDeleteResponse;
        }
        
        private string GenerateFileName(byte[] data)
        {
            using (var sha1 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                string hash = string.Concat(sha1.ComputeHash(data).Select(x => x.ToString("X2")));
                string date = DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss");
                return hash + date;
            }
        }
    }
}