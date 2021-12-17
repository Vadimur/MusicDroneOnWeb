using System;

namespace MusicDrone.Business.Models.Requests
{
    public class RoomsUsersDeleteRequestDto
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
    }
}
