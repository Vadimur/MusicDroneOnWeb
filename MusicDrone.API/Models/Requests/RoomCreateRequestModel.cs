using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class RoomCreateRequestModel
    {
        [Required]
        public string Name { get; set; }
    }
}
