using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicDrone.API.Helpers;

namespace MusicDrone.API.Models.Requests
{
    [ModelBinder(typeof(JsonModelBinder), Name = "json")]
    public class MusicAddRequestModel
    {
        public string Name { get; set; }
        public string RoomId { get; set; }
        public IFormFile UploadedFile { get; set; }
    }
}