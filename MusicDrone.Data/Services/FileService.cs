using System;
using System.IO;
using System.Threading.Tasks;
using MusicDrone.Data.Services.Abstraction;

namespace MusicDrone.Data.Services
{
    public class FileService : IFileService
    {
        private readonly string _basePath;
        
        public FileService(string basePath)
        {
            _basePath = basePath;
        }

        public Stream GetFileStream(string path)
        {
            string fullPath = Path.Combine(_basePath, path);
            FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return fs; 
        }

        public async Task SaveFile(byte[] fileBytes, string path)
        {
            string fullPath = Path.Combine(_basePath, path);
            FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
            await fs.WriteAsync(fileBytes);
        }

        public void RemoveFile(string path)
        {
            string fullPath = Path.Combine(_basePath, path);
            File.Delete(fullPath);
        }
    }
}