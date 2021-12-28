using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class MusicGetRequestModel
    {
        [Required] 
        public string UserId { get; set; }
        [Required] 
        public string MusicId { get; set; }
    }
}