using System;
using System.Collections.Generic;

namespace MusicDrone.Data.Models
{
    public class Room : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public virtual ICollection<RoomUser> RoomsUsers { get; set; }
    }
}
