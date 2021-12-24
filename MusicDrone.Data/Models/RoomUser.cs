using System;

namespace MusicDrone.Data.Models
{
    public class RoomUser : BaseEntity<Guid>
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public RoomUserRole Role { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Room Room { get; set; }
    }
}
