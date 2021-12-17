using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class RoomsUsersCreateRequestModel
    {
        [Required]
        public string RoomId { get; set; }
    }
}
