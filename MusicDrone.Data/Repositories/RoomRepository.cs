using System;
using System.Threading.Tasks;
using MusicDrone.Data.Abstractions;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Repositories
{
    public class RoomRepository : GenericRepository<Guid, Room>, IRoomRepository
    {
        public RoomRepository(MusicDroneDbContext musicDroneDbContext) : base(musicDroneDbContext)
        {
        }
    }
}