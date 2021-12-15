using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class RoomsUsersCreateRequestModel
    {
        [Required]
        public string RoomId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
