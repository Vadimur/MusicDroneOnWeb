using System;

namespace MusicDrone.Business.Models.Requests
{
    public class RoomCreateRequestDto
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }
}
