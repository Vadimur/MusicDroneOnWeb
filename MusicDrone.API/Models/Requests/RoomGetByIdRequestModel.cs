using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class RoomGetByIdRequestModel
    {
        [Required]
        public string Id { get; set; }
    }
}
