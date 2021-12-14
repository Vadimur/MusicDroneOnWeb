using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
