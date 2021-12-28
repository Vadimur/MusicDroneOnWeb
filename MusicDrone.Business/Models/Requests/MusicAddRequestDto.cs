using System;

namespace MusicDrone.Business.Models.Requests
{
    public class MusicAddRequestDto
    {
        public string MusicName { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public byte[] File { get; set; }
        public string FileExtension { get; set; }
    }
}