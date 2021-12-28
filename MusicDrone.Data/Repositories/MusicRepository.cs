using System;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Repositories
{
    public class MusicRepository : GenericRepository<Guid, Music>, IMusicRepository
    {
        public MusicRepository(MusicDroneDbContext musicDroneDbContext) : base(musicDroneDbContext)
        {
        }
    }
}