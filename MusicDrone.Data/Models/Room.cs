using System;

namespace MusicDrone.Data.Models
{
    public class Room : BaseEntity<Guid>
    {
        public string Name { get; set; }
    }
}
