using System;

namespace MusicDrone.Business.Models.Requests
{
    public class RoomDeleteRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
