using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDrone.Business.Models.Requests
{
    public class RoomsUsersCreateRequestDto
    {
        public string RoomId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}
