using System;
using System.Threading.Tasks;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Abstractions
{
    public interface IRoomRepository : IAsyncRepository<Guid, Room>
    {
    }
}