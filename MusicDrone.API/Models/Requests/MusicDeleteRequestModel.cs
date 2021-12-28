using System.ComponentModel.DataAnnotations;

namespace MusicDrone.API.Models.Requests
{
    public class MusicDeleteRequestModel
    {
        [Required]
        public string Id { get; set; }
    }
}