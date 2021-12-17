using System;
using System.Security.Claims;

namespace MusicDrone.Business.Models.Requests
{
    public class RoomDeleteRequestDto
    {
        public Guid Id { get; set; }
        public ClaimsPrincipal UserClaims { get; set; }
    }
}
