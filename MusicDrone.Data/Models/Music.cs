using System;
using System.ComponentModel.DataAnnotations;

namespace MusicDrone.Data.Models
{
    public class Music : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public bool PassedModeration { get; set; }
        [Range(0, 5)]
        public float AverageRating { get; set; }
        public Guid FileInfoId { get; set; }
        public Guid RoomId { get; set; }
        public Guid OwnerId { get; set; }
    }
}