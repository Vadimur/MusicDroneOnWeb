using System;

namespace MusicDrone.Business.Models.Responses
{
    public class RoomsUsersGetByUserIdResponseDto
    {
        public Guid RoomId { get; set; } 
        public string RoomName { get; set; }
    }
}
