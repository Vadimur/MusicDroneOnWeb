using System;

namespace MusicDrone.Data.DTO
{
    public class RoomsUsersGetByUserIdResponseDto
    {
        public Guid RoomId { get; set; } 
        public string RoomName { get; set; }
    }
}
