using System;

namespace MusicDrone.API.Models.Responses
{
    public class RoomsUsersGetByUserIdResponseModel
    {

        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
    }
}
