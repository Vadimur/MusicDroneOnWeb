using System;

namespace MusicDrone.Business.Models.Requests
{
    public class MusicDeleteRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
