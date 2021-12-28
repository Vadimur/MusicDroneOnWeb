using System.IO;
using System.Threading.Tasks;

namespace MusicDrone.Data.Services.Abstraction
{
    public interface IFileService
    {
        public Stream GetFileStream(string path);
        public Task SaveFile(byte[] file, string path);
        public void RemoveFile(string path);
    }
}