using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class RoomsUsersDeleteRequestModel
    {
        [Required]
        public string RoomId { get; set; }
    }
}
