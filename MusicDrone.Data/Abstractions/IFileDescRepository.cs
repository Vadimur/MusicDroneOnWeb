using System;
using MusicDrone.Data.Models;

namespace MusicDrone.Data.Abstractions
{
    public interface IFileDescRepository : IAsyncRepository<Guid, FileDesc>
    {
        
    }
}