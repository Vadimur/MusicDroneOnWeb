using System;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Repositories
{
    public class FileDescRepository : GenericRepository<Guid, FileDesc>, IFileDescRepository
    {
        public FileDescRepository(MusicDroneDbContext musicDroneDbContext) : base(musicDroneDbContext)
        {
        }
    }
}