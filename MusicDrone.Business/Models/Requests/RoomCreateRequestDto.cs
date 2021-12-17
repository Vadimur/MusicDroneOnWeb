using System.Security.Claims;

namespace MusicDrone.Business.Models.Requests
{
    public class RoomCreateRequestDto
    {
        public string Name { get; set; }
        public ClaimsPrincipal UserClaims { get; set; }
    }
}
