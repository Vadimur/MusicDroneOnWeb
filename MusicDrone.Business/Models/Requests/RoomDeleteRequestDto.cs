using System.Security.Claims;

namespace MusicDrone.Business.Models.Requests
{
    public class RoomDeleteRequestDto
    {
        public string Id { get; set; }
        public ClaimsPrincipal userClaims { get; set; }
    }
}
