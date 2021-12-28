using System;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Abstractions
{
    public interface IMusicRepository : IAsyncRepository<Guid, Music>
    {
        
    }
}