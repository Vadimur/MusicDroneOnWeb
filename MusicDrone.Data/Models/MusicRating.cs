using System;
using System.ComponentModel.DataAnnotations;

namespace MusicDrone.Data.Models
{
    public class MusicRating : BaseEntity<Guid>
    {
        [Required]
        public Guid MusicId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}