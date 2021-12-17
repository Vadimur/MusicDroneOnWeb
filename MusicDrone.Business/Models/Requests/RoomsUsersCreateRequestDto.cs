using System;
using System.Security.Claims;

namespace MusicDrone.Business.Models.Requests
{
    public class RoomsUsersCreateRequestDto
    {
        public Guid RoomId { get; set; }
        public ClaimsPrincipal UserClaims { get; set; }
    }
}
