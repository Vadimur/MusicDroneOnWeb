using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class RoomsUsersGetByRoomIdRequestModel
    {
        [Required]
        public string RoomId { get; set; }
    }
}
