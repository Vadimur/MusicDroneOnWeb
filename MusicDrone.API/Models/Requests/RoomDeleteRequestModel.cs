using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class RoomDeleteRequestModel
    {
        [Required]
        public string Id { get; set; }
    }
}
