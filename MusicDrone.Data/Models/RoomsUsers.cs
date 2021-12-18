using System;

namespace MusicDrone.Data.Models
{
    public class RoomsUsers : BaseEntity<Guid>
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; }
    }
}
