using System.IO;
using Microsoft.AspNetCore.Http;

namespace MusicDrone.API.Extensions
{
    public static class FormFileExtensions
    {
        public static byte[] ToByteArray(this IFormFile input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}