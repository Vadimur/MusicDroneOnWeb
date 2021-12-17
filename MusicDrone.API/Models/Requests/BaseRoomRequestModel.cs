using System;
using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class BaseRoomRequestModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
