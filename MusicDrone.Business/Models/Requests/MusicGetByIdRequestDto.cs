using System;

namespace MusicDrone.Business.Models.Requests
{
    public class MusicGetByIdRequestDto
    {
        public Guid MusicId { get; set; }
        public Guid UserId { get; set; }
    }
}